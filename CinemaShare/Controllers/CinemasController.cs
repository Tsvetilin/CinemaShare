using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business;
using CinemaShare.Common.Mapping;
using CinemaShare.Models.ViewModels;
using Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace CinemaShare.Controllers
{
    public class CinemasController : Controller
    {
        private readonly ICinemaBusiness cinemaBusiness;
        private readonly IMapper mapper;
        private const int cinemasOnPage = 3;
        public CinemasController( ICinemaBusiness cinemaBusiness,
                                   IMapper mapper)
        {
            this.cinemaBusiness = cinemaBusiness;
            this.mapper = mapper;
        }

        public IActionResult Index(int id = 1)
        {
            List<CinemaCardViewModel> cinemas = new List<CinemaCardViewModel>();
            int pageCount = (int)Math.Ceiling((double)cinemaBusiness.CountAllCinemas() / cinemasOnPage);

            cinemas = cinemaBusiness.GetPageItems(id, cinemasOnPage, mapper.MapToCinemaCardViewModel).ToList();
            if (id > pageCount || id < 1)
            {
                id = 1;
            }
            CinemasIndexViewModel viewModel = new CinemasIndexViewModel
            {
                PagesCount = pageCount,
                CurrentPage = id,
                Cinemas = cinemas,
            };

            return View(viewModel);
        }

    }
}