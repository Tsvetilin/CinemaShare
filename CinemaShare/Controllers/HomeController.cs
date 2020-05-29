using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Business;
using CinemaShare.Common.Mapping;
using CinemaShare.Models.ViewModels;

namespace CinemaShare.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFilmDataBusiness filmDataBusiness;
        private readonly IMapper mapper;

        public HomeController(IFilmDataBusiness filmDataBusiness,
                              IMapper mapper)
        {
            this.filmDataBusiness = filmDataBusiness;
            this.mapper = mapper;
        }

        ///<summary>
        /// Default website page
        ///</summary>
        public IActionResult Index()
        {
            HomePageViewModel viewModel = new HomePageViewModel
            {
                TopFilms = filmDataBusiness.GetTopFilms(mapper.MapToFilmCardViewModel).ToList(),
                RecentFilms = filmDataBusiness.GetRecentFilms(mapper.MapToFilmCardViewModel).ToList(),
            };

            return View(viewModel);
        }

        /// <summary>
        /// Website's privacy policy page
        /// </summary>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Website's default error page
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        ///<summary>
        /// Website's default status code error handler page
        ///</summary>
        public IActionResult StatusError(int code = 404)
        {
            return this.View(code);
        }
    }
}
