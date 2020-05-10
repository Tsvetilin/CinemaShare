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
using CinemaShare.Models.ViewModels;

namespace CinemaShare.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFilmDataBusiness filmDataBusiness;
        private readonly IMapper mapper;

        public HomeController(IFilmDataBusiness filmDataBusiness,
                              IMapper mapper)
        {
            this.filmDataBusiness = filmDataBusiness;
            this.mapper = mapper;
        }

        public IActionResult Index()
        {
            HomePageViewModel viewModel = new HomePageViewModel
            {
                TopFilms= filmDataBusiness.GetTopFilms(mapper.MapToFilmCardViewModel).ToList(),
                RecentFilms = filmDataBusiness.GetRecentFilms(mapper.MapToFilmCardViewModel).ToList(),
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
