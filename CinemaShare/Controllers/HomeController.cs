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
        /// Redirects to home page
        ///</summary>
        public IActionResult Index()
        {
            HomePageViewModel viewModel = new HomePageViewModel
            {
                TopFilms= filmDataBusiness.GetTopFilms(mapper.MapToFilmCardViewModel).ToList(),
                RecentFilms = filmDataBusiness.GetRecentFilms(mapper.MapToFilmCardViewModel).ToList(),
            };
            
            return View(viewModel);
        }

        ///<returns>
        /// View
        ///</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        ///<returns>
        /// Error view model
        ///</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        ///<returns>
        /// Error Status
        ///</returns>
        public IActionResult StatusError(int code = 404)
        {
            return this.View(code);
        }
    }
}
