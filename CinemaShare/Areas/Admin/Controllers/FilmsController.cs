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
    public class FilmsController : Controller
    {
        private readonly IFilmDataBusiness filmDataBusiness;
        private readonly IFilmReviewBusiness filmReviewBusiness;

        public FilmsController(IFilmDataBusiness filmDataBusiness, IFilmReviewBusiness filmReviewBusiness)
        {
            this.filmDataBusiness = filmDataBusiness;
            this.filmReviewBusiness = filmReviewBusiness;
        }

        ///<summary>
        /// Films default page for listing
        ///</summary>
        public IActionResult Index()
        {
            var model = filmDataBusiness.GetAll();
            return View(model);
        }
        
        ///<summary>
        /// Shows details for film searched by ID
        ///</summary>
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

        ///<summary>
        /// Shows edit page for the film
        ///</summary>
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

        ///<summary>
        /// Edits selected film's data
        ///</summary>
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
                await filmDataBusiness.UpdateAsync(filmData);
                return RedirectToAction(nameof(Details), new { id });
            }

            return View(filmData);
        }

        ///<summary>
        /// Deletes film data by selected ID
        ///</summary>
        ///<returns>Film data view</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await filmDataBusiness.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        ///<summary>
        /// Deletes film review and
        /// sets new film ID
        ///</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(string id , string filmId)
        {
            await filmReviewBusiness.DeleteAsync(id);
            return RedirectToAction(nameof(Details), new { id = filmId });
        }
    }
}
