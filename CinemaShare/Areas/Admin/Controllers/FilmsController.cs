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
        private readonly IFilmReviewBusiness filmReviewBusiness;

        public FilmsController(IFilmDataBusiness filmDataBusiness, IFilmReviewBusiness filmReviewBusiness)
        {
            this.filmDataBusiness = filmDataBusiness;
            this.filmReviewBusiness = filmReviewBusiness;
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
        public async Task<IActionResult> Edit(string id, [Bind("FilmId,Title,Poster,Description,Director,Cast,Runtime,ReleaseDate,TargetAudience")] FilmData filmData)
        {
            var dataInContext = await filmDataBusiness.GetAsync(id);
            if (id != filmData.FilmId || dataInContext==null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await filmDataBusiness.Update(filmData);
                return RedirectToAction(nameof(Details), new { id });
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(string id , string filmId)
        {
            await filmReviewBusiness.Delete(id);
            return RedirectToAction(nameof(Details), new { id = filmId });
        }
    }
}