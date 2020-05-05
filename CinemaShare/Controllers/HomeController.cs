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
using Data;
using CinemaShare.Common.Mapping;

namespace CinemaShare.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IFilmBusiness filmsBusiness;
        private readonly IFilmDataBusiness filmDataBusiness;
        private readonly IMapper mapper;

        public HomeController(ILogger<HomeController> logger,
                              IFilmBusiness filmsBusiness,
                              IFilmDataBusiness filmDataBusiness,
                              IMapper mapper)
        {
            _logger = logger;
            this.filmsBusiness = filmsBusiness;
            this.filmDataBusiness = filmDataBusiness;
            this.mapper = mapper;
        }

        public IActionResult Index(int? page)
        {
            var rawAllFilms = filmDataBusiness.GetAll();
            var rawTopFilms =  rawAllFilms?.OrderByDescending(x => x.Film.Rating)?.Take(10);
            var rawRecentFilms = rawAllFilms?.OrderByDescending(x => x.ReleaseDate).Take(4);

            HomePageViewModel viewModel = new HomePageViewModel
            {
                TopFilms = mapper.MapToFilmCardViewModel(rawTopFilms).ToList(),
                RecentFilms = mapper.MapToFilmCardViewModel(rawRecentFilms).ToList(),
            };
            
            return View(viewModel);
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
