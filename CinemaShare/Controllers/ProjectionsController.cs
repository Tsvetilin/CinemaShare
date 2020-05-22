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
using Microsoft.AspNetCore.Mvc.Rendering;

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
            var pageCount = (int)Math.Ceiling((double)filmProjectionBusiness.CountAllProjections() / projectionsOnPage);
            if (id > pageCount || id < 1)
            {
                id = 1;
            }
            var projections = filmProjectionBusiness.GetPageItems(id, projectionsOnPage, mapper.MapToProjectionCardViewModel)
                                                    .ToList();
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
            if(id==null)
            {
                return this.RedirectToAction("Index", "Projections");
            }
            var viewModel = await filmProjectionBusiness.GetAsync(id, mapper.MapToProjectionDataViewModel);
            if (viewModel == null)
            {
                return NotFound();
            }
            return View(viewModel);
        }

        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> Add()
        {
            var user = await userManager.GetUserAsync(User);
            var cinema = user?.Cinema;
            if (cinema != null)
            {
                var model = new ProjectionInputModel();
                var films = filmDataBusiness.GetAll().ToList().Select(x => x.Title).OrderBy(x=>x).ToDictionary(x => x);
                model.AvailableFilms = new SelectList(films, "Key", "Value");
                model.Date = DateTime.UtcNow.Date;
                return this.View(model);
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

            if(input.Date<DateTime.UtcNow)
            {
                ModelState.AddModelError("Date", "Projection can start at least one hour from now!");
            }

            if (!ModelState.IsValid)
            {
                var films = filmDataBusiness.GetAll().ToList().Select(x => x.Title).OrderBy(x => x).ToDictionary(x => x);
                input.AvailableFilms = new SelectList(films, "Key", "Value");
                return this.View(input);
            }

            var projection = mapper.MapToFilmProjection(input, film, cinema);
            await filmProjectionBusiness.Add(projection);
            return this.RedirectToAction("Detail", "Projections", new { projection.Id });
        }

        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> Update(string id)
        {
            var user = await userManager.GetUserAsync(User);
            var cinema = user?.Cinema;
            var projection = await filmProjectionBusiness.Get(id);

            if (cinema == null || projection == null || projection?.CinemaId!=cinema.Id || projection?.Date<DateTime.UtcNow)
            {
                return this.RedirectToAction("Index", "Projections");
            }
            var films = filmDataBusiness.GetAll().ToList().Select(x => x.Title).OrderBy(x => x).ToDictionary(x => x);
            var inputModel = mapper.MapToProjectionInputModel(projection);
            inputModel.AvailableFilms = new SelectList(films, "Key", "Value");
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

            if (cinema == null || projection == null || projection?.CinemaId != cinema.Id || projection?.Date < DateTime.UtcNow)
            {
                return this.RedirectToAction("Index", "Projections");
            }

            var film = filmDataBusiness.GetByName(input.FilmTitle);
            if (film?.FilmId == null)
            {
                ModelState.AddModelError("Found", "Film not found!");
            }

            if (input.Date < DateTime.UtcNow)
            {
                ModelState.AddModelError("Date", "Projection can start at least one hour from now!");
            }

            if (!ModelState.IsValid)
            {
                return this.View(input);
            }
            
            var projectionUrlPattern = Url.ActionLink("Index", "Projections");
            var ticketUrlPattern = Url.ActionLink("Index", "Tickets");
            var updatedProjection = mapper.MapToFilmProjection(input, film, cinema);
            updatedProjection.Id = id;
            await filmProjectionBusiness.Update(updatedProjection, projectionUrlPattern, ticketUrlPattern);
            return this.RedirectToAction("Detail", "Projections", new { projection.Id });
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
                var projectionUrlPattern = Url.ActionLink("Index", "Projections");
                await filmProjectionBusiness.Delete(id, projectionUrlPattern);
            }
            return this.RedirectToAction("Manage", "Cinemas",new { cinema?.Id });
        }
    }
}