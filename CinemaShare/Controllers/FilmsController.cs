﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Business;
using CinemaShare.Common.Mapping;
using CinemaShare.Models.JsonModels;
using CinemaShare.Models.ViewModels;
using CinemaShare.Models.InputModels;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace CinemaShare.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class FilmsController : Controller
    {
        private readonly IFilmDataBusiness filmDataBusiness;
        private readonly IFilmBusiness filmBusiness;
        private readonly IFilmReviewBusiness reviewBusiness;
        private readonly UserManager<CinemaUser> userManager;
        private readonly IMapper mapper;
        private readonly IFilmFetchAPI filmFetchApi;
        private readonly ICloudinaryAPI cloudinaryAPI;
        private const int filmsOnPage = 3;

        public FilmsController(IFilmDataBusiness filmDataBusiness,
                               IFilmBusiness filmBusiness,
                               IFilmReviewBusiness reviewBusiness,
                               UserManager<CinemaUser> userManager,
                               IMapper mapper,
                               IFilmFetchAPI filmFetchApi,
                               ICloudinaryAPI cloudinaryAPI)
        {
            this.filmDataBusiness = filmDataBusiness;
            this.filmBusiness = filmBusiness;
            this.reviewBusiness = reviewBusiness;
            this.userManager = userManager;
            this.mapper = mapper;
            this.filmFetchApi = filmFetchApi;
            this.cloudinaryAPI = cloudinaryAPI;
        }

        ///<summary>
        /// Default films page for listing and searching, with sorting options
        ///</summary>
        public IActionResult Index(int id = 1, string sort = "", string search = "")
        {
            int pageCount = 1;
            List<ExtendedFilmCardViewModel> films = new List<ExtendedFilmCardViewModel>();

            if (!string.IsNullOrWhiteSpace(search))
            {
                films = filmDataBusiness.GetAllByName(search, mapper.MapToExtendedFilmCardViewModel).ToList();
                if (films.Count != 0)
                {
                    return View(new FilmsIndexViewModel
                    {
                        PagesCount = 1,
                        CurrentPage = 1,
                        Films = films
                    });
                }
                ModelState.AddModelError("Found", "Film not found!");
            }

            pageCount = (int)Math.Ceiling((double)filmDataBusiness.CountAllFilms() / filmsOnPage);

            if (id > pageCount || id < 1)
            {
                id = 1;
            }
            films = filmDataBusiness.GetPageItems(id, filmsOnPage, sort,
                                                                mapper.MapToExtendedFilmCardViewModel).ToList();
            var viewModel = new FilmsIndexViewModel
            {
                PagesCount = pageCount,
                CurrentPage = id,
                Films = films
            };

            return View(viewModel);
        }

        ///<summary>
        /// Shows details page about a film by selected ID
        ///</summary>
        public async Task<IActionResult> Detail(string id = null)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Films");
            }

            var viewModel = await filmDataBusiness.GetAsync(id, mapper.MapToFilmDataViewModel);
            if (viewModel?.Id == null)
            {
                return this.NotFound();
            }

            return this.View(viewModel);
        }

        ///<summary>
        /// Gets film data as Json
        ///</summary>
        [Authorize]
        public IActionResult Add(string id = null)
        {
            //Check if there is TempData record for previous fetching
            if (id == null ? false : TempData[id] != null)
            {
                var serializedFilm = TempData[id];
                var inputModel = JsonConvert.DeserializeObject<FilmInputModel>(serializedFilm.ToString());
                if (inputModel.Error != null)
                {
                    ModelState.Clear();
                    ModelState.AddModelError("Error", inputModel.Error);
                }
                return this.View(inputModel);
            }

            return this.View();
        }

        ///<summary>
        /// Adds a new film to the database
        ///</summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add(FilmInputModel input)
        {
            if (filmDataBusiness.IsAlreadyAdded(input.Title))
            {
                ModelState.AddModelError("Added", "Film already added!");
            }

            var userId = userManager.GetUserId(User);

            if (Guid.TryParse(input.Poster, out _))
            {
                if (input.PosterUpload != null)
                {
                    var fileName = $"_{input.Title}_Poster";

                    IFormFile file = input.PosterUpload;

                    using var stream = new MemoryStream();
                    await file.CopyToAsync(stream);
                    var posterUrl = await cloudinaryAPI.UploadImageAsync(stream, fileName);
                    input.Poster = posterUrl;
                }
                else
                {
                    ModelState.AddModelError("Poster", "Poster is required! Either add image url or upload one.");
                }
            }

            if (!ModelState.IsValid || userId == null)
            {
                return this.View();
            }

            var film = new Film
            {
                AddedByUserId = userId,
                Rating = input.Rating,
                Ratings = new List<FilmRating> { new FilmRating { Rating = input.Rating, UserId = userId } }
            };

            await filmBusiness.AddAsync(film);
            await filmDataBusiness.AddAsync(input, film, mapper.MapToFilmData);

            return this.RedirectToAction("Detail", "Films", new { film.Id });
        }

        ///<summary>
        /// Adds review to a selected film by user
        ///</summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddReview(FilmReviewInputModel input, string id)
        {
            var userId = userManager.GetUserId(User);
            if (ModelState.IsValid && id != null && userId != null)
            {
                await reviewBusiness.AddAsync(new FilmReview
                {
                    Content = input.Content,
                    CreatedOn = DateTime.UtcNow,
                    FilmId = id,
                    UserId = userId
                });
            }

            return RedirectToAction("Detail", "Films", new { Id = id });
        }

        ///<summary>
        /// Fetches a film by title
        ///</summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> FetchFilm(string title = null)
        {
            if (title == null)
            {
                return RedirectToAction("Add", "Films");
            }

            var tempDataId = Guid.NewGuid().ToString();
            if (filmDataBusiness.IsAlreadyAdded(title))
            {
                var inputModel = new FilmInputModel
                {
                    Error = "Film already added!"
                };
                TempData[tempDataId] = JsonConvert.SerializeObject(inputModel);
            }
            else
            {
                TempData[tempDataId] = await filmFetchApi.FetchFilmAsync<FilmJsonModel, FilmInputModel>
                    (title, mapper.MapToFilmInputModel);
            }

            return this.RedirectToAction("Add", "Films", new { Id = tempDataId });
        }

        ///<summary>
        /// Rates a selected film by user
        ///</summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RateFilm(string id, int rating)
        {
            if (rating > 0 && rating < 6 && id != null)
            {
                await filmBusiness.RateAsync(id, userManager.GetUserId(User), rating);
            }
            return RedirectToAction("Detail", "Films", new { Id = id });
        }

        ///<summary>
        /// Shows edit page for selected film
        ///</summary>
        [Authorize]
        public async Task<IActionResult> Update(string id)
        {
            var user = await userManager.GetUserAsync(User);
            var film = await filmBusiness.GetAsync(id);

            // Check if user is signed in, film exists and the user is the one who added it
            if (user?.Id == null ? false : user.Id == film?.AddedByUserId)
            {
                var inputModel = mapper.MapToFilmUpdateInputModel(film.FilmData);
                return this.View(inputModel);
            }
            else if (film != null)
            {
                return RedirectToAction("Detail", "Films", new { Id = id });
            }
            return RedirectToAction("Index", "Films");
        }

        ///<summary>
        /// Updates film data by selected ID 
        ///</summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Update(FilmUpdateInputModel input, string id)
        {
            var userId = userManager.GetUserId(User);
            var film = await filmBusiness.GetAsync(id);

            //Check if user is signed in, film exists and the user is the one who added it
            if (userId == null ? false : userId == film?.AddedByUserId)
            {
                //Check if the poster url is guid, which means that there should be uploaded poster 
                if (Guid.TryParse(input.Poster, out _))
                {
                    if (input.PosterUpload != null)
                    {
                        var fileName = $"_{film.FilmData.Title}_Poster";

                        IFormFile file = input.PosterUpload;

                        using var stream = new MemoryStream();
                        await file.CopyToAsync(stream);
                        var posterUrl = await cloudinaryAPI.UploadImageAsync(stream, fileName);
                        input.Poster = posterUrl;

                    }
                    else
                    {
                        ModelState.AddModelError("Poster", "Poster is required! Either add image url or upload one.");
                    }
                }

                if (!ModelState.IsValid)
                {
                    return this.View(input);
                }

                var data = mapper.MapToFilmData(input);
                data.FilmId = film.Id;
                data.Title = film.FilmData.Title;
                await filmDataBusiness.UpdateAsync(data);
                return RedirectToAction("Detail", "Films", new { Id = id });
            }

            return RedirectToAction("Index", "Films");
        }

        ///<summary>
        /// Selects a film by ID and adds it to user's watchlist
        ///</summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddToWatchList(string id)
        {
            var film = await filmDataBusiness.GetAsync(id);
            var userId = userManager.GetUserId(User);
            if (film != null && userId != null)
            {
                await filmBusiness.AddToWatchListAsync(userId, film.Film);
                return RedirectToAction("Detail", "Films", new { Id = id });
            }
            return RedirectToAction("Index", "Films");
        }


        ///<summary>
        /// Removes film from user's watchlist by selected ID
        ///</summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RemoveFromWatchList(string id)
        {
            var film = await filmDataBusiness.GetAsync(id);
            var userId = userManager.GetUserId(User);
            if (film != null && userId != null)
            {
                await filmBusiness.RemoveFromWatchListAsync(userId, film.Film);
                return RedirectToAction("Detail", "Films", new { Id = id });
            }
            return RedirectToAction("Index", "Films");
        }

        ///<summary>
        /// Shows user's films watchlist
        ///</summary>
        [Authorize]
        public IActionResult WatchList()
        {
            var userId = userManager.GetUserId(User);
            var films = filmBusiness.GetWatchList(userId, mapper.MapToExtendedFilmCardViewModel).ToList();
            return this.View(new FilmsIndexViewModel { Films = films });
        }

    }
}
