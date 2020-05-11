﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business;
using CinemaShare.Common.Mapping;
using CinemaShare.Models.InputModels;
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

        public IActionResult Index()
        {
            var allProjectionsList = filmProjectionBusiness.GetAll(mapper.MapToProjectionViewModel).ToList();
            return View(allProjectionsList);
        }

        public IActionResult Detail(string id)
        {
            var viewModel = filmProjectionBusiness.GetAsync(id, mapper.MapToProjectionViewModel);
            if (viewModel == null)
            {
                return this.RedirectToAction("Index", "Projections");
            }
            return View(viewModel);
        }

        [Authorize]
        public async Task<IActionResult> AddProjection()
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
        public async Task<IActionResult> AddProjection(ProjectionInputModel input)
        {
            var user = await userManager.GetUserAsync(User);
            var cinema = user?.Cinema;
            if (cinema == null)
            {
                return this.RedirectToAction("Index", "Projections");
            }

            var film = await filmDataBusiness.GetByNameAsync(input.FilmTitle);
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
        public async Task<IActionResult> UpdateProjection(string id)
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
        public async Task<IActionResult> UpdateProjection(string id, ProjectionInputModel input)
        {
            var user = await userManager.GetUserAsync(User);
            var cinema = user?.Cinema;
            var projection = await filmProjectionBusiness.Get(id);

            if (cinema == null || projection == null || projection?.CinemaId != cinema.Id)
            {
                return this.RedirectToAction("Index", "Projections");
            }

            var film = await filmDataBusiness.GetByNameAsync(input.FilmTitle);
            if (film?.FilmId == null)
            {
                ModelState.AddModelError("Found", "Film not found!");
            }

            if (!ModelState.IsValid)
            {
                return this.View();
            }

            var updatedProjection = mapper.MapToFilmProjection(input, film, cinema);
            updatedProjection.Id = projection.Id;
            await filmProjectionBusiness.Update(projection);
            return this.RedirectToAction("Detail", "Projections", new { Id = projection.Id });
        }

        [Authorize(Roles ="Manager, Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteProjection(string id)
        {
            var user = await userManager.GetUserAsync(User);
            var cinema = user?.Cinema;
            if (cinema != null)
            {
                await filmProjectionBusiness.Delete(id);
            }
            return this.RedirectToAction("Index", "Projections");
        }
    }
}