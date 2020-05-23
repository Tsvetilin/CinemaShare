using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CinemaShare.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AutoValidateAntiforgeryToken]
    [Authorize(Roles = "Admin")]
    public class ProjectionsController : Controller
    {
        private readonly IFilmProjectionBusiness filmProjectionBusiness;

        public ProjectionsController(IFilmProjectionBusiness filmProjectionBusiness)
        {
            this.filmProjectionBusiness = filmProjectionBusiness;
        }

        public IActionResult Index()
        {
            var projecitons = filmProjectionBusiness.GetAll().ToList();
            return View(projecitons);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projeciton = await filmProjectionBusiness.Get(id);
            if (projeciton == null)
            {
                return NotFound();
            }

            return View(projeciton);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projection = await filmProjectionBusiness.Get(id);
            if (projection == null)
            {
                return NotFound();
            }

            return View(projection);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Date,ProjecitonType,TotalTickets,Film")] FilmProjection projection)
        {
            var projectionInContext = await filmProjectionBusiness.Get(id);

            if (id != projection.Id || projectionInContext == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                projection.Id = projectionInContext.Id;
                var ticketUrlPattern = Url.ActionLink("Index", "Tickets");
                var projecitonsUrlPattern = Url.ActionLink("Index", "Projections");
                await filmProjectionBusiness.Update(projection,projecitonsUrlPattern ,ticketUrlPattern);
                return RedirectToAction(nameof(Index));
            }

            return View(projection);
        }

      /*  [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var projectionUrlPattern = Url.ActionLink("Index", "Projections");
            await cinemaBusiness.DeleteAsync(id, projectionUrlPattern);
            return RedirectToAction(nameof(Index));
        }*/
    }
}