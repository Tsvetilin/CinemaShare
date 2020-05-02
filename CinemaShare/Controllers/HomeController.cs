using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CinemaShare.Models;
using Business;
using Data.Models;
using Data.Enums;

namespace CinemaShare.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IFilmBusiness filmsBusiness;
        private readonly IFilmDataBusiness filmDataBusiness;

        public HomeController(ILogger<HomeController> logger,
                              IFilmBusiness filmsBusiness,
                              IFilmDataBusiness filmDataBusiness)
        {
            _logger = logger;
            this.filmsBusiness = filmsBusiness;
            this.filmDataBusiness = filmDataBusiness;
        }

        public IActionResult Index()
        {
            var rawAllFilms = filmDataBusiness.GetAll();
            var data = rawAllFilms.ToList()[0].Film;
            //var rawTopFilms =  rawAllFilms?.OrderBy(x => x.Rating)?.Take(10)?.Reverse();
            //var rawRecentFilms = rawAllFilms?.OrderBy(x => x.FilmData.ReleaseDate).Take(4)?.Reverse();

            HomePageViewModel viewModel = new HomePageViewModel
            {
                TopFilms = MapToViewModel(rawAllFilms).ToList(),
                RecentFilms = MapToViewModel(rawAllFilms).ToList(),
            };
            
            return View(viewModel);
        }

        private IEnumerable<FilmCardViewModel> MapToViewModel(IEnumerable<FilmData> rawFilms)
        {
            return rawFilms.Select(x => new FilmCardViewModel
            {
                Title = x.Title,
                Genres = string.Join(", ", x.Genre.Select(a => a.Genre.ToString())),
                Poster = x.Poster,
                Rating = "5"
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
