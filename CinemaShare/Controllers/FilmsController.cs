using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Business;
using CinemaShare.Models;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Schema;

namespace CinemaShare.Controllers
{
    public class FilmsController : Controller
    {
        [BindProperty]
        private FilmReviewInputModel FilmReviewInput { get; set; }

        private readonly IFilmDataBusiness filmBusiness;
        private readonly IFilmReviewBusiness reviewBusiness;
        private readonly UserManager<CinemaUser> userManager;
        private const int filmsOnPage = 3;

        public FilmsController(IFilmDataBusiness filmBusiness, IFilmReviewBusiness reviewBusiness, UserManager<CinemaUser> userManager)
        {
            this.filmBusiness = filmBusiness;
            this.reviewBusiness = reviewBusiness;
            this.userManager = userManager;
        }

        public IActionResult Index(int id = 1, string sort = "")
        {
            var allFilms = filmBusiness.GetAll();
            int pageCount = (int)Math.Ceiling((double)allFilms.Count() / filmsOnPage);
            if (id > pageCount)
            {
                id = 1;
            }
            FilmsIndexViewModel viewModel = new FilmsIndexViewModel
            {
                Films = allFilms.Select(film=>MapToViewModel(film)).ToList(),
                PagesCount = pageCount,
                CurrentPage = id
            };
            if (sort == "Name")
            {
                allFilms = allFilms.OrderBy(x => x.Title);
            }
            else if (sort == "Year")
            {
                allFilms = allFilms.OrderByDescending(x => x.ReleaseDate);
            }
            else if (sort == "Rating")
            {
                allFilms = allFilms.OrderByDescending(x => x.Film.Rating);
            }
            viewModel.Films = allFilms.Skip(filmsOnPage * (id - 1)).Take(filmsOnPage).
                                       Select(filmData=>MapToViewModel(filmData)).ToList();
            return View(viewModel);
        }

        [Authorize]
        public IActionResult Add()
        {
            return this.View();
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add(FilmInputModel input)
        {
            await filmBusiness.Add(new FilmData());
            return this.View();
        }

        public async Task<IActionResult> Detail(string id = null)
        {
            var film = await filmBusiness.Get(id);
            if (film == null)
            {
                this.NotFound();
            }

            FilmDataViewModel viewModel = MapToViewModel(MapToViewModel(film), film);
            return this.View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddReview([Bind("Content")]FilmReviewInputModel Input, string id)
        {
            var user = userManager.GetUserAsync(User).GetAwaiter().GetResult();
            if(ModelState.IsValid && id!=null || user!=null)
            {
                await reviewBusiness.Add(new FilmReview
                {
                    Content = Input.Content,
                    CreatedOn = DateTime.UtcNow,
                    FilmId = id,
                    UserId = user.Id
                });

            }

            return this.Redirect(Request.Headers["Referer"].ToString());
        }

        private ExtendedFilmCardViewModel MapToViewModel(FilmData rawFilmData)
        {
            return new ExtendedFilmCardViewModel
            {
                Title = rawFilmData.Title,
                Genres = string.Join(", ", rawFilmData.Genre.Select(a => a.Genre.ToString())),
                Poster = rawFilmData.Poster,
                Rating = rawFilmData.Film.Rating.ToString(),
                Id = rawFilmData.FilmId,
                Cast = rawFilmData.Cast,
                Description = rawFilmData.Description,
                Director = rawFilmData.Director,
                Runtime = rawFilmData.Runtime,
                ReleaseDate = rawFilmData.ReleaseDate,
            };
        }

        private FilmDataViewModel MapToViewModel(ExtendedFilmCardViewModel filmCard, FilmData film)
        {
            FilmDataViewModel viewModel = new FilmDataViewModel();
            PropertyInfo[] props = filmCard.GetType().GetProperties();
            foreach (var prop in props)
            {
                viewModel.GetType().GetProperty(prop.Name).SetValue(viewModel, prop.GetValue(filmCard));
            }
            viewModel.TargetAudience = film.TargetAudience;
            viewModel.FilmProjections = film.Film.FilmProjection.ToList();
            viewModel.FilmReviews = film.Film.FilmReviews.ToList();
            return viewModel;
        }

    }
}