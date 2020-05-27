using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaShare.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AutoValidateAntiforgeryToken]
    [Authorize(Roles = "Admin")]
    public class CinemasController : Controller
    {
        private readonly ICinemaBusiness cinemaBusiness;

        public CinemasController(ICinemaBusiness cinemaBusiness)
        {
            this.cinemaBusiness = cinemaBusiness;
        }

        ///<summary>
        /// Universal method for redirection of pages
        ///</summary>
        ///<returns>Film data model</returns>
        public IActionResult Index()
        {
            var model = cinemaBusiness.GetAll();
            return View(model);
        }

        ///<summary>
        /// Shows details for cinema searched by ID
        ///</summary>
        ///<returns>Film data view</returns>
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cinema = await cinemaBusiness.GetAsync(id);
            if (cinema == null)
            {
                return NotFound();
            }

            return View(cinema);
        }

        ///<summary>
        /// Edits cinema data by selected ID
        ///</summary>
        ///<returns>Cinema view</returns>
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cinema = await cinemaBusiness.GetAsync(id);
            if (cinema == null)
            {
                return NotFound();
            }

            return View(cinema);
        }

        ///<summary>
        /// Edits only selected film data
        ///</summary>
        ///<returns>Cinema view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,City,Country")] Cinema cinema)
        {
            var cinemaInContext = await cinemaBusiness.GetAsync(id);

            if (id != cinema.Id || cinemaInContext == null) 
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                cinema.ManagerId = cinemaInContext.ManagerId;
                var ticketUrlPattern = Url.ActionLink("Index", "Tickets");
                await cinemaBusiness.UpdateAsync(cinema,ticketUrlPattern);
                return RedirectToAction(nameof(Details), new { id });
            }

            return View(cinema);
        }

        ///<summary>
        /// Deletes cinema by ID and
        /// and makes a redirection to Projections
        ///</summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var projectionUrlPattern = Url.ActionLink("Index", "Projections");
            await cinemaBusiness.DeleteAsync(id,projectionUrlPattern);
            return RedirectToAction(nameof(Index));
        }
    }
}
