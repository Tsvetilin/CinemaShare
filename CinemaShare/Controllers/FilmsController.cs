using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business;
using CinemaShare.Models;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Schema;

namespace CinemaShare.Controllers
{
    public class FilmsController : Controller
    {
        private readonly IFilmDataBusiness filmBusiness;
        private readonly IFilmReviewBusiness reviewBusiness;
        private const int filmsOnPage = 3;

        public FilmsController(IFilmDataBusiness filmBusiness, IFilmReviewBusiness reviewBusiness)
        {
            this.filmBusiness = filmBusiness;
            this.reviewBusiness = reviewBusiness;
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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddReview(ReviewInputModel input)
        {
            await reviewBusiness.Add(new FilmReview());
            return this.View();
        }

        public async Task<IActionResult> Detail(string id = null)
        {
            var film = await filmBusiness.Get(id);
            if (film == null)
            {
                this.NotFound();
            }

            var viewModel = MapToViewModel(film);

            return this.View(viewModel);
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
                TargetAudience = rawFilmData.TargetAudience,
            };
        }

    }
}