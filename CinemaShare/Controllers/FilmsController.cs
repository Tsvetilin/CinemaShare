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

namespace CinemaShare.Controllers
{
    public class FilmsController : Controller
    {
        [BindProperty]
        private FilmReviewInputModel FilmReviewInput { get; set; }

        private readonly IFilmDataBusiness filmDataBusiness;
        private readonly IFilmBusiness filmBusiness;
        private readonly IFilmReviewBusiness reviewBusiness;
        private readonly UserManager<CinemaUser> userManager;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private const int filmsOnPage = 3;

        public FilmsController(IFilmDataBusiness filmDataBusiness,
                               IFilmBusiness filmBusiness,
                               IFilmReviewBusiness reviewBusiness,
                               UserManager<CinemaUser> userManager,
                               IMapper mapper, IConfiguration configuration)
        {
            this.filmDataBusiness = filmDataBusiness;
            this.filmBusiness = filmBusiness;
            this.reviewBusiness = reviewBusiness;
            this.userManager = userManager;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        public IActionResult Index(int id = 1, string sort = "")
        {
            int pageCount = (int)Math.Ceiling((double)filmDataBusiness.CountAllFilms() / filmsOnPage);
            if (id > pageCount || id < 1)
            {
                id = 1;
            }

            FilmsIndexViewModel viewModel = new FilmsIndexViewModel
            {
                PagesCount = pageCount,
                CurrentPage = id
            };

            List<ExtendedFilmCardViewModel> films = new List<ExtendedFilmCardViewModel>();
            if (sort == "Name")
            {
                films = filmDataBusiness.GetFilmsOnPageByName(id, filmsOnPage).
                        Select(x => mapper.MapToExtendedFilmCardViewModel(x)).ToList();
            }
            else if (sort == "Year")
            {
                films = filmDataBusiness.GetFilmsOnPageByYear(id, filmsOnPage).
                        Select(x => mapper.MapToExtendedFilmCardViewModel(x)).ToList();
            }
            else if (sort == "Rating")
            {
                films = filmDataBusiness.GetFilmsOnPageByRating(id, filmsOnPage).
                        Select(x => mapper.MapToExtendedFilmCardViewModel(x)).ToList();
            }
            else
            {
                films = filmDataBusiness.GetPageItems(id, filmsOnPage).
                        Select(x => mapper.MapToExtendedFilmCardViewModel(x)).ToList();
            }

            viewModel.Films = films;

            return View(viewModel);
        }

        [Authorize]
        public IActionResult Add(FilmInputModel input, string id = null)
        {
            if (input == null)
            {
                return this.View(input);
            }
            return this.View(input);
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

            var film = new Film
            {
                UserId = userManager.GetUserId(User)
            };
            await filmBusiness.AddAsync(film);

            var filmData = new FilmData
            {
                FilmId = film.Id,
                Film = film,
                Title = input.Title,
                Description = input.Description,
                Director = input.Director,
                Cast = input.Cast,
                Poster = input.Poster,
                TargetAudience = input.TargetAudience,
                ReleaseDate = input.ReleaseDate,
                Runtime = input.Runtime,
                Genre = input.Genre.Select(x => new GenreType() { Genre = x }).ToList(),
            };
            await filmDataBusiness.Add(filmData);

            return this.RedirectToAction("Detail", "Films", new { Id = film.Id });
        }

        public async Task<IActionResult> Detail(string id = null)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Films");
            }

            var film = await filmDataBusiness.Get(id);
            if (film == null)
            {
                this.NotFound();
            }

            FilmDataViewModel viewModel = mapper.MapToFilmDataViewModel(film);
            return this.View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddReview(FilmReviewInputModel input, string id)
        {
            var user = userManager.GetUserAsync(User).GetAwaiter().GetResult();
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
            HttpClient client = new HttpClient();
            string apiKey = configuration.GetSection("OMDb").Value;
            string url = $"https://www.omdbapi.com/?apikey={apiKey}&t={title}";
            var json = await client.GetStringAsync(url);
            var filmData = JsonConvert.DeserializeObject<FilmJsonModel>(json);
            var filmInputModel = new FilmInputModel
            {
                Title = filmData.Title,
                Director = filmData.Director,
                Cast = filmData.Actors,
                Poster = filmData.Poster,
                Description = filmData.Plot,
                TargetAudience = TargetAudience.All
            };
            foreach (var genre in filmData.Genre.Split(","))
            {
            }
            return this.RedirectToAction("Add", "Films", filmInputModel);
        }
    }
}