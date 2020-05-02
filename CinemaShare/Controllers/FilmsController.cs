using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business;
using CinemaShare.Models;
using Microsoft.AspNetCore.Mvc;

namespace CinemaShare.Controllers
{
    public class FilmsController : Controller
    {
        private readonly IFilmBusiness filmBusiness;

        public FilmsController(IFilmBusiness filmBusiness)
        {
            this.filmBusiness = filmBusiness;
        }
        public IActionResult Index()
        {
            FilmsIndexViewModel viewModel = new FilmsIndexViewModel
            {
                Films = new List<FilmDataViewModel>(),
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Add()
        {
            return this.View();
        }
    }
}