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
        private const int filmsOnPage = 1;

        public FilmsController(IFilmDataBusiness filmBusiness)
        {
            this.filmBusiness = filmBusiness;
        }
        public IActionResult Index(int id=1)
        {
            var allFilms = filmBusiness.GetAll();
            int pageCount = (int)Math.Ceiling((double)allFilms.Count() / filmsOnPage);
            if (id >pageCount)
            {
                id = 1;
            }
            FilmsIndexViewModel viewModel = new FilmsIndexViewModel
            {
                Films = MapToViewModel(allFilms.Skip(filmsOnPage * (id-1)).Take(filmsOnPage)).ToList(),
                PagesCount = pageCount,
                CurrentPage = id
            };
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
                Redirect = "\"/films/" + x.FilmId + "\"",
                Cast = x.Cast,
                Description = x.Description,
                Director = x.Director,
                Runtime = x.Runtime,
                ReleaseDate = x.ReleaseDate
            }) ;
        }
    }
}