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

namespace CinemaShare.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IFilmBusiness filmsBusiness;

        public HomeController(ILogger<HomeController> logger, IFilmBusiness filmsBusiness)
        {
            _logger = logger;
            this.filmsBusiness = filmsBusiness;
        }

        public async Task<IActionResult> Index()
        {
            var rawTopFilms =  filmsBusiness.GetTopFilms();
            var rawRecentFilms =  filmsBusiness.GetRecentFilms();

            HomePageViewModel viewModel = new HomePageViewModel
            {
                TopFilms = MapToViewModel(rawTopFilms).ToList(),
                RecentFilms = MapToViewModel(rawRecentFilms).ToList(),
            };

            return View(viewModel);
        }

        private IEnumerable<FilmCardViewModel> MapToViewModel(IEnumerable<Film> rawFilms)
        {
            return rawFilms.Select(x => new FilmCardViewModel
            {
                Title = x.FilmData.Title,
                Genres = string.Join(", ", x.FilmData.Genre.Select(a => a.Genre.ToString())),
                Poster = x.FilmData.Poster,
                Rating = x.Rating.ToString()
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
