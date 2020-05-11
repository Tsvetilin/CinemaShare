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

        public IActionResult Index(int id = 1, string search = "")
        {
            int pageCount = 1;
            List<CinemaCardViewModel> cinemas = new List<CinemaCardViewModel>();
            if (!String.IsNullOrEmpty(search))
            {
                List<CinemaCardViewModel> cinemasName = cinemaBusiness.GetAllByName(search,
                                                        mapper.MapToCinemaCardViewModel).ToList();
                List<CinemaCardViewModel> cinemasCity = cinemaBusiness.GetAllByCity(search,
                                                        mapper.MapToCinemaCardViewModel).ToList();
                if (cinemasName.Count != 0 || cinemasCity.Count != 0)
                {
                    if (cinemasName.Count != 0)
                    {
                        cinemas.AddRange(cinemasName);
                    }
                    if (cinemasCity.Count != 0)
                    {
                        cinemas.AddRange(cinemasCity);
                    }
                    return View(new CinemasIndexViewModel
                    {
                        PagesCount = 1,
                        CurrentPage = 1,
                        Cinemas = cinemas
                    });
                }
                ModelState.AddModelError("Found", "Cinema not found!");
            }

            if (id > pageCount || id < 1)
            {
                id = 1;
            }
            pageCount = (int)Math.Ceiling((double)cinemaBusiness.CountAllCinemas() / cinemasOnPage);
            cinemas = cinemaBusiness.GetPageItems(id, cinemasOnPage, mapper.MapToCinemaCardViewModel).ToList();
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

        [Authorize(Roles = "Manager, Admin")]
        public IActionResult Add(string id = null)
        {
            if (userManager.GetUserAsync(User).GetAwaiter().GetResult()?.Cinema != null)
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
            return this.RedirectToAction("Detail", "Cinemas", new { Id = cinema.Id });
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
                await cinemaBusiness.UpdateAsync(data);
                return RedirectToAction("Detail", "Cinemas", new { Id = id });
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
                await cinemaBusiness.DeleteAsync(id);
            }
            return RedirectToAction("Index", "Cinemas");
        }
    }
}