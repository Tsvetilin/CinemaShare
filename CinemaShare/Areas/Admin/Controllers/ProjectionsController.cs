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
        private readonly IFilmDataBusiness filmDataBusiness;

        public ProjectionsController(IFilmProjectionBusiness filmProjectionBusiness,
                                     IFilmDataBusiness filmDataBusiness)
        {
            this.filmProjectionBusiness = filmProjectionBusiness;
            this.filmDataBusiness = filmDataBusiness;
        }

        /// <summary>
        /// Default projections page listing
        /// </summary>
        public IActionResult Index()
        {
            var projecitons = filmProjectionBusiness.GetAll().ToList();
            return View(projecitons);
        }

        /// <summary>
        /// Shows details about projection by ID
        /// </summary>
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projeciton = await filmProjectionBusiness.GetAsync(id);
            if (projeciton == null)
            {
                return NotFound();
            }

            return View(projeciton);
        }

        /// <summary>
        /// Shows edit page for the projection
        /// </summary>
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projection = await filmProjectionBusiness.GetAsync(id);
            if (projection == null)
            {
                return NotFound();
            }

            var films = filmDataBusiness.GetAll().ToList().OrderBy(x => x.Title).
                                                  ToDictionary(x => x.FilmId, x => x.Title);
            ViewBag.SelectListOfFilms = new SelectList(films, "Key", "Value");
            return View(projection);
        }

        /// <summary>
        /// Edits selected film's data
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id,
                                  [Bind("Id,Date,ProjecitonType,TotalTickets,FilmId, TicketPrices")] FilmProjection projection)
        {
            var projectionInContext = await filmProjectionBusiness.GetAsync(id);
            if (id != projection.Id || projectionInContext == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var films = filmDataBusiness.GetAll().ToList().OrderBy(x => x.Title).
                                                  ToDictionary(x => x.FilmId, x => x.Title);
                ViewBag.SelectListOfFilms = new SelectList(films, "Key", "Value");
                return View(projection);
            }

            var ticketUrlPattern = Url.ActionLink("Index", "Tickets");
            var projecitonsUrlPattern = Url.ActionLink("Index", "Projections");
            projection.CinemaId = projectionInContext.CinemaId;
            await filmProjectionBusiness.UpdateAsync(projection, projecitonsUrlPattern, ticketUrlPattern);
            return RedirectToAction(nameof(Details), new { id });
        }

        /// <summary>
        /// Deletes projection by ID and redirects the page to projections
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var projectionUrlPattern = Url.ActionLink("Index", "Projections");
            await filmProjectionBusiness.DeleteAsync(id, projectionUrlPattern);
            return RedirectToAction(nameof(Index));
        }
    }
}
