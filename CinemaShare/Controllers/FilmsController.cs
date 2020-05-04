using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business;
using CinemaShare.Models;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Schema;

namespace CinemaShare.Controllers
{
    public class FilmsController : Controller
    {
        private readonly IFilmDataBusiness filmBusiness;
        private const int filmsOnPage = 3;

        public FilmsController(IFilmDataBusiness filmBusiness)
        {
            this.filmBusiness = filmBusiness;
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
                Films = MapToViewModel(allFilms).ToList(),
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
            viewModel.Films = MapToViewModel(allFilms.Skip(filmsOnPage * (id - 1)).Take(filmsOnPage)).ToList();
            return View(viewModel);
        }

        public async Task<IActionResult> Add()
        {
            return this.View();
        }
        private IEnumerable<ExtendedFilmCardViewModel> MapToViewModel(IEnumerable<FilmData> rawFilms)
        {
            return rawFilms.Select(x => new ExtendedFilmCardViewModel
            {
                Title = x.Title,
                Genres = string.Join(", ", x.Genre.Select(a => a.Genre.ToString())),
                Poster = x.Poster,
                Rating = x.Film.Rating.ToString(),
                Id =  x.FilmId,
                Cast = x.Cast,
                Description = x.Description,
                Director = x.Director,
                Runtime = x.Runtime,
                ReleaseDate = x.ReleaseDate
            });
        }

        public async Task<IActionResult> Detail(string id=null)
        {
            var film = await filmBusiness.Get(id);
            if (film==null)
            {
                this.NotFound();
            }

            var viewModel = new ExtendedFilmCardViewModel()
            {
                Title = film.Title,
                Genres = string.Join(", ", film.Genre.Select(a => a.Genre.ToString())),
                Poster = film.Poster,
                Rating = film.Film.Rating.ToString(),
                Id = film.FilmId,
                Cast = film.Cast,
                Description = film.Description,
                Director = film.Director,
                Runtime = film.Runtime,
                ReleaseDate = film.ReleaseDate,
                TargetAudience = film.TargetAudience,
               /* Image1 = film.Image1,
                Image2 = film.Image2,
                Image3 = film.Image3,*/
            };

            return this.View(viewModel);
        }
    }
}