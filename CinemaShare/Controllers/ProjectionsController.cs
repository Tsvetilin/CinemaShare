using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business;
using CinemaShare.Common.Mapping;
using CinemaShare.Models.InputModels;
using CinemaShare.Models.ViewModels;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CinemaShare.Controllers
{
    public class ProjectionsController : Controller
    {
        private readonly IFilmProjectionBusiness filmProjectionBusiness;
        private readonly IFilmDataBusiness filmDataBusiness;
        private readonly UserManager<CinemaUser> userManager;
        private readonly IMapper mapper;
        private const int projectionsOnPage = 3;

        public ProjectionsController(IFilmProjectionBusiness filmProjectionBusiness,
                                     IFilmDataBusiness filmDataBusiness,
                                     UserManager<CinemaUser> userManager,
                                     IMapper mapper)
        {
            this.filmProjectionBusiness = filmProjectionBusiness;
            this.filmDataBusiness = filmDataBusiness;
            this.userManager = userManager;
            this.mapper = mapper;
        }

       public IActionResult Index(int id=1)
        {
            var allProjectionsList = filmProjectionBusiness.GetAll(mapper.MapToProjectionCardViewModel).ToList();
            var pageCount = (int)Math.Ceiling((double)filmProjectionBusiness.CountAllProjections() / projectionsOnPage);
            if (id > pageCount || id < 1)
            {
                id = 1;
            }
            var projections = filmProjectionBusiness.GetPageItems(id, projectionsOnPage, mapper.MapToProjectionCardViewModel).ToList();
            ProjectionIndexViewModel viewModel = new ProjectionIndexViewModel
            {
                PagesCount = pageCount,
                CurrentPage = id,
                Projectitons = projections
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Detail(string id)
        {
            var viewModel = await filmProjectionBusiness.GetAsync(id, mapper.MapToProjectionDataViewModel);
            if (viewModel == null)
            {
                return this.RedirectToAction("Index", "Projections");
            }
            return View(viewModel);
        }

        [Authorize]
        public async Task<IActionResult> Add()
        {
            var user = await userManager.GetUserAsync(User);
            var cinema = user?.Cinema;
            if (cinema != null)
            {
                return this.View();
            }
            return this.RedirectToAction("Index", "Projections");
        }

        [Authorize(Roles = "Manager, Admin")]
        [HttpPost]
        public async Task<IActionResult> Add(ProjectionInputModel input)
        {
            var user = await userManager.GetUserAsync(User);
            var cinema = user?.Cinema;
            if (cinema == null)
            {
                return this.RedirectToAction("Index", "Projections");
            }

            var film = filmDataBusiness.GetByName(input.FilmTitle);
            if (film?.FilmId == null)
            {
                ModelState.AddModelError("Found", "Film not found!");
            }

            if (!ModelState.IsValid)
            {
                return this.View();
            }

            var projection = mapper.MapToFilmProjection(input, film, cinema);
            await filmProjectionBusiness.Add(projection);
            return this.RedirectToAction("Detail", "Projections", new { Id = projection.Id });
        }

        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> Update(string id)
        {
            var user = await userManager.GetUserAsync(User);
            var cinema = user?.Cinema;
            var projection = await filmProjectionBusiness.Get(id);

            if (cinema == null || projection == null || projection?.CinemaId!=cinema.Id)
            {
                return this.RedirectToAction("Index", "Projections");
            }

            var inputModel = mapper.MapToProjectionInputModel(projection);
            return View(inputModel);
        }

        [Authorize(Roles = "Manager, Admin")]
        [HttpPost]
        public async Task<IActionResult> Update(string id, ProjectionInputModel input)
        {
            //TODO: remove tickets with seats > number of tickets && update ticket prices
            var user = await userManager.GetUserAsync(User);
            var cinema = user?.Cinema;
            var projection = await filmProjectionBusiness.Get(id);

            if (cinema == null || projection == null || projection?.CinemaId != cinema.Id)
            {
                return this.RedirectToAction("Index", "Projections");
            }

            var film = filmDataBusiness.GetByName(input.FilmTitle);
            if (film?.FilmId == null)
            {
                ModelState.AddModelError("Found", "Film not found!");
            }

            if (!ModelState.IsValid)
            {
                return this.View();
            }

            var updatedProjection = mapper.MapToFilmProjection(input, film, cinema);
            updatedProjection.Id = id;
            await filmProjectionBusiness.Update(updatedProjection);
            return this.RedirectToAction("Detail", "Projections", new { Id = projection.Id });
        }

        [Authorize(Roles ="Manager, Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            //TODO: see if all tickets are removed
            var user = await userManager.GetUserAsync(User);
            var cinema = user?.Cinema;
            if (cinema != null)
            {
                await filmProjectionBusiness.Delete(id);
            }
            return this.RedirectToAction("Manage", "Cinemas",new { Id = cinema?.Id });
        }
    }
}