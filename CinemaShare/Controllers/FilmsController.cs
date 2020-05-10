using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Text;
using Business;
using CinemaShare.Common.Mapping;
using CinemaShare.Models;
using CinemaShare.Models.JsonModels;
using Data.Enums;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using CinemaShare.Models.ViewModels;

namespace CinemaShare.Controllers
{
    public class FilmsController : Controller
    {
        private readonly IFilmDataBusiness filmDataBusiness;
        private readonly IFilmBusiness filmBusiness;
        private readonly IFilmReviewBusiness reviewBusiness;
        private readonly UserManager<CinemaUser> userManager;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly IFilmFetchAPI filmFetchApi;
        private const int filmsOnPage = 3;

        public FilmsController(IFilmDataBusiness filmDataBusiness,
                               IFilmBusiness filmBusiness,
                               IFilmReviewBusiness reviewBusiness,
                               UserManager<CinemaUser> userManager,
                               IMapper mapper,
                               IConfiguration configuration,
                               IFilmFetchAPI filmFetchApi)
        {
            this.filmDataBusiness = filmDataBusiness;
            this.filmBusiness = filmBusiness;
            this.reviewBusiness = reviewBusiness;
            this.userManager = userManager;
            this.mapper = mapper;
            this.configuration = configuration;
            this.filmFetchApi = filmFetchApi;
        }

        public IActionResult Index(int id = 1, string sort = "", string search = "")
        {
            int pageCount = 1;
            List<ExtendedFilmCardViewModel> films = new List<ExtendedFilmCardViewModel>();
            FilmsIndexViewModel viewModel = new FilmsIndexViewModel();
            if (!String.IsNullOrEmpty(search))
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
                ModelState.AddModelError("found", "Film not found!");
            }
            pageCount = (int)Math.Ceiling((double)filmDataBusiness.CountAllFilms() / filmsOnPage);

            if (sort == "Name")
            {
                films = filmDataBusiness.GetFilmsOnPageByName(id, filmsOnPage,
                                                              mapper.MapToExtendedFilmCardViewModel).ToList();
            }
            else if (sort == "Year")
            {
                films = filmDataBusiness.GetFilmsOnPageByYear(id, filmsOnPage,
                                                              mapper.MapToExtendedFilmCardViewModel).ToList();
            }
            else if (sort == "Rating")
            {
                films = filmDataBusiness.GetFilmsOnPageByRating(id, filmsOnPage,
                                                                mapper.MapToExtendedFilmCardViewModel).ToList();
            }
            else
            {
                films = filmDataBusiness.GetPageItems(id, filmsOnPage,
                                                                mapper.MapToExtendedFilmCardViewModel).ToList();
            }
            if (id > pageCount || id < 1)
            {
                id = 1;
            }
            viewModel = new FilmsIndexViewModel
            {
                PagesCount = pageCount,
                CurrentPage = id,
                Films = films
            };

            return View(viewModel);
        }

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

        [Authorize]
        public IActionResult Add(string id = null)
        {
            if (id == null ? false : TempData[id] != null)
            {
                var serializedFilm = TempData[id];
                var inputModel = JsonConvert.DeserializeObject<FilmInputModel>(serializedFilm.ToString());
                if (inputModel.Error != null)
                {
                    ModelState.Clear();
                    ModelState.AddModelError("found", "Film not found!");
                }
                return this.View(inputModel);
            }

            return this.View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add(FilmInputModel input)
        {
            if (filmDataBusiness.IsAlreadyAdded(input.Title))
            {
                ModelState.AddModelError("Added", "Film already added!");
            }

            if (!ModelState.IsValid)
            {
                return this.View();
            }

            var userId = userManager.GetUserId(User);
            var film = new Film
            {
                AddedByUserId = userId,
                Rating=input.Rating,
                Ratings= new List<FilmRating> { new FilmRating { Rating=input.Rating, UserId= userId} }
            };
            await filmBusiness.AddAsync(film);
            await filmDataBusiness.AddAsync(input, film, mapper.MapToFilmData);

            return this.RedirectToAction("Detail", "Films", new { Id = film.Id });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddReview(FilmReviewInputModel input, string id)
        {
            var user = await userManager.GetUserAsync(User);
            if (ModelState.IsValid && id != null && user != null)
            {
                await reviewBusiness.Add(new FilmReview
                {
                    Content = input.Content,
                    CreatedOn = DateTime.UtcNow,
                    FilmId = id,
                    UserId = user.Id
                });
            }

            return RedirectToAction("Detail", "Films", new { Id = id });
            //return this.Redirect(Request.Headers["Referer"].ToString());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> FetchFilm(string title = null)
        {
            if (title == null)
            {
                return RedirectToAction("Add", "Films");
            }
            string apiKey = configuration.GetSection("OMDb").Value;
            var id = Guid.NewGuid().ToString();
            TempData[id] = await filmFetchApi.FetchFilmAsync<FilmJsonModel, FilmInputModel>
                (apiKey, title, mapper.MapToFilmInputModel);

            return this.RedirectToAction("Add", "Films", new { Id = id });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RateFilm(string id, int rating)
        {
            //TODO: improve conditions
            if (rating > 0 && rating < 6 && id!=null)
            {
                await filmBusiness.RateAsync(id,userManager.GetUserId(User),rating);
            }
            return RedirectToAction("Detail","Films",new {Id=id});
        }
    }
}