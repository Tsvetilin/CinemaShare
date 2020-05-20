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
    [AutoValidateAntiforgeryToken]
    public class CinemasController : Controller
    {
        private readonly ICinemaBusiness cinemaBusiness;
        private readonly IFilmProjectionBusiness filmProjectionBusiness;
        private readonly IMapper mapper;
        private readonly UserManager<CinemaUser> userManager;
        private const int cinemasOnPage = 3;
        public CinemasController(ICinemaBusiness cinemaBusiness,
                                 IFilmProjectionBusiness filmProjectionBusiness,
                                 IMapper mapper,
                                 UserManager<CinemaUser> userManager)
        {
            this.cinemaBusiness = cinemaBusiness;
            this.filmProjectionBusiness = filmProjectionBusiness;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        public IActionResult Index(int id = 1, string search = "")
        {
            if (!String.IsNullOrEmpty(search))
            {
                var searchResult = cinemaBusiness.GetSearchResults(search, mapper.MapToCinemaCardViewModel).ToList();
                if (searchResult.Count != 0 )
                {
                    return View(new CinemasIndexViewModel
                    {
                        PagesCount = 1,
                        CurrentPage = 1,
                        Cinemas = searchResult
                    });
                }
                ModelState.AddModelError("Found", "Cinema not found!");
            }

            int pageCount = (int)Math.Ceiling((double)cinemaBusiness.CountAllCinemas() / cinemasOnPage);
            if (id > pageCount || id < 1)
            {
                id = 1;
            }
            var  cinemas = cinemaBusiness.GetPageItems(id, cinemasOnPage, mapper.MapToCinemaCardViewModel).ToList();
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

            var projections = filmProjectionBusiness.GetAllByCinemaId(id, mapper.MapToProjectionCardViewModel);
            viewModel.FilmProjections = projections.ToList();
            return this.View(viewModel);
        }

        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> Add()
        {
            var user = await userManager.GetUserAsync(User);
            if (user?.Cinema != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return this.View();
        }

        [Authorize(Roles = "Manager, Admin")]
        [HttpPost]
        public async Task<IActionResult> Add(CinemaInputModel input)
        {
            if (cinemaBusiness.IsAlreadyAdded(input.Name))
            {
                ModelState.AddModelError("Added", "Cinema already added!");
            }

            var user = await userManager.GetUserAsync(User);
            if (user?.Cinema != null)
            {
                ModelState.AddModelError("Added", "You already manage your cinema.");
            }

            if (!ModelState.IsValid)
            {
                return this.View();
            }

            var cinema = new Cinema
            {
                Name = input.Name,
                Country = input.Country,
                City = input.City,
                ManagerId = userManager.GetUserId(User)
            };
            await cinemaBusiness.AddAsync(cinema);
            return this.RedirectToAction("Manage", "Cinemas", new { cinema.Id });
        }

        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> Update(string id)
        {
            var user = await userManager.GetUserAsync(User);
            var cinema = await cinemaBusiness.GetAsync(id);
            if (user?.Id == cinema?.ManagerId)
            {
                var inputModel = mapper.MapToCinemaUpdateInputModel(cinema);
                return this.View(inputModel);
            }
            else if (cinema != null)
            {
                return RedirectToAction("Detail", "Cinemas", new { Id = id });
            }
            return RedirectToAction("Index", "Cinemas");
        }

        [Authorize(Roles = "Manager, Admin")]
        [HttpPost]
        public async Task<IActionResult> Update(CinemaInputModel input, string id)
        {
            var user = await userManager.GetUserAsync(User);
            var cinema = await cinemaBusiness.GetAsync(id);
            if (user?.Id == cinema?.ManagerId)
            {
                if (!ModelState.IsValid)
                {
                    return this.View(input);
                }

                var data = mapper.MapToCinemaData(input);
                data.Id = cinema.Id;
                data.ManagerId = cinema.ManagerId;
                var ticketUrlPattern = Url.ActionLink("Index", "Tickets");
                await cinemaBusiness.UpdateAsync(data,ticketUrlPattern);
                return RedirectToAction("Manage", "Cinemas", new { Id = id });
            }

            return RedirectToAction("Index", "Cinemas");
        }

        [Authorize(Roles = "Manager, Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await userManager.GetUserAsync(User);
            var cinema = await cinemaBusiness.GetAsync(id);
            if (user?.Id == cinema?.ManagerId)
            {
                var projectionUrlPattern = Url.ActionLink("Index", "Projections");
                await cinemaBusiness.DeleteAsync(id,projectionUrlPattern);
            }
            return RedirectToAction("Index", "Cinemas");
        }

        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> Manage(string id)
        {
            var user = await userManager.GetUserAsync(User);
            var cinema = await cinemaBusiness.GetAsync(id);
            if (user?.Id == cinema?.ManagerId)
            {
                var projections = filmProjectionBusiness.GetAllByCinemaId(id, mapper.MapToProjectionCardViewModel);
                var viewModel = mapper.MapToCinemaDataViewModel(cinema);
                viewModel.FilmProjections = projections.ToList();
                return this.View(viewModel);
            }
            return RedirectToAction("Index", "Cinemas");
        }
    }
}