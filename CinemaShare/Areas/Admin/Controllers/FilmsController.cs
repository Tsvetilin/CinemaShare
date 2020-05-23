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
    public class FilmsController : Controller
    {
        private readonly IFilmDataBusiness filmDataBusiness;

        public FilmsController(IFilmDataBusiness filmDataBusiness)
        {
            this.filmDataBusiness = filmDataBusiness;
        }

        public IActionResult Index()
        {
            var model = filmDataBusiness.GetAll();
            return View(model);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cinema = await filmDataBusiness.GetAsync(id);
            if (cinema == null)
            {
                return NotFound();
            }

            return View(cinema);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filmData = await filmDataBusiness.GetAsync(id);
            if (filmData == null)
            {
                return NotFound();
            }

            return View(filmData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Title,Poster,Description,Director,Cast,Runtime,ReleaseDate,TargetAudience")] FilmData filmData)
        {
            var dataInContext = await filmDataBusiness.GetAsync(id);
            if (id != filmData.FilmId || dataInContext==null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                filmData.FilmId = dataInContext.FilmId;
                await filmDataBusiness.Update(filmData);
                return RedirectToAction(nameof(Index));
            }

            return View(filmData);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await filmDataBusiness.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}