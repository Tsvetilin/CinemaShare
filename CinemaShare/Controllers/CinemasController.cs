using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business;
using CinemaShare.Common.Mapping;
using CinemaShare.Models;
using CinemaShare.Models.ViewModels;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CinemaShare.Controllers
{
    public class CinemasController : Controller
    {
        private readonly ICinemaBusiness cinemaBusiness;
        private readonly IMapper mapper;
        private readonly UserManager<CinemaUser> userManager;
        private const int cinemasOnPage = 3;
        public CinemasController(ICinemaBusiness cinemaBusiness,
                                 IMapper mapper,
                                 UserManager<CinemaUser> userManager)
        {
            this.cinemaBusiness = cinemaBusiness;
            this.mapper = mapper;
            this.userManager = userManager;
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
        public async Task<IActionResult> Detail(string id = null)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Cinemas");
            }

            var viewModel = await cinemaBusiness.GetAsync(id, mapper.MapToCinemaDataViewModel);
            if (viewModel?.Id == null)
            {
                return this.NotFound();
            }

            return this.View(viewModel);
        }

       /* [Authorize]
        public IActionResult Add(string id = null)
        {
            if (id == null ? false : TempData[id] != null)
            {
                var serializedFilm = TempData[id];
                var inputModel = JsonConvert.DeserializeObject<FilmInputModel>(serializedFilm.ToString());
                if (inputModel.Error != null)
                {
                    ModelState.Clear();
                    ModelState.AddModelError("found", "Film not found!");
                }
                return this.View(inputModel);
            }

            return this.View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add(FilmInputModel input)
        {
            if (filmDataBusiness.IsAlreadyAdded(input.Title))
            {
                ModelState.AddModelError("Added", "Film already added!");
            }

            if (!ModelState.IsValid)
            {
                return this.View();
            }

            var film = new Film
            {
                UserId = userManager.GetUserId(User)
            };
            await filmBusiness.AddAsync(film);
            await filmDataBusiness.AddAsync(input, film, mapper.MapToFilmData);

            return this.RedirectToAction("Detail", "Films", new { Id = film.Id });
        }*/

    }
}