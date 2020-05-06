using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Business;
using CinemaShare.Common.Mapping;
using CinemaShare.Models;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Schema;

namespace CinemaShare.Controllers
{
    public class FilmsController : Controller
    {
        [BindProperty]
        private FilmReviewInputModel FilmReviewInput { get; set; }

        private readonly IFilmDataBusiness filmDataBusiness;
        private readonly IFilmBusiness filmBusiness;
        private readonly IFilmReviewBusiness reviewBusiness;
        private readonly UserManager<CinemaUser> userManager;
        private readonly IMapper mapper;
        private const int filmsOnPage = 3;

        public FilmsController(IFilmDataBusiness filmDataBusiness,
                               IFilmBusiness filmBusiness,
                               IFilmReviewBusiness reviewBusiness,
                               UserManager<CinemaUser> userManager,
                               IMapper mapper)
        {
            this.filmDataBusiness = filmDataBusiness;
            this.filmBusiness = filmBusiness;
            this.reviewBusiness = reviewBusiness;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        public IActionResult Index(int id = 1, string sort = "")
        {
            var allFilms = filmDataBusiness.GetAll();
            int pageCount = (int)Math.Ceiling((double)allFilms.Count() / filmsOnPage);
            if (id > pageCount)
            {
                id = 1;
            }

            FilmsIndexViewModel viewModel = new FilmsIndexViewModel
            {
                Films = allFilms.Select(film => mapper.MapToExtendedFilmCardViewModel(film)).ToList(),
                PagesCount = pageCount,
                CurrentPage = id
            };

            if (sort == "Name")
            {
                allFilms = allFilms.OrderBy(x => x.Title);
            }
            else if (sort == "Year")
            {
                allFilms = allFilms.OrderByDescending(x => x.ReleaseDate);
            }
            else if (sort == "Rating")
            {
                allFilms = allFilms.OrderByDescending(x => x.Film.Rating);
            }

            viewModel.Films = allFilms.Skip(filmsOnPage * (id - 1)).Take(filmsOnPage).
                                       Select(filmData => mapper.MapToExtendedFilmCardViewModel(filmData)).ToList();
            return View(viewModel);
        }

        [Authorize]
        public IActionResult Add(FilmInputModel input, string id=null)
        {
            return this.View(input);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add(FilmInputModel input)
        {
            if (!TryValidateModel(input))
            {
                return RedirectToAction("Add","Films",input);
            }
            var film = new Film
            {
                UserId = userManager.GetUserId(User)
            };
            await filmBusiness.AddAsync(film);
            var filmData = new FilmData
            {
                FilmId = film.Id,
                Title=input.Title,
            };
            await filmDataBusiness.Add(filmData);

            return this.RedirectToAction("Detail", "Films", new {Id=film.Id});
        }

        public async Task<IActionResult> Detail(string id = null)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Films");
            }

            var film = await filmDataBusiness.Get(id);
            if (film == null)
            {
                this.NotFound();
            }

            FilmDataViewModel viewModel = mapper.MapToFilmDataViewModel(film);
            return this.View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddReview(FilmReviewInputModel input, string id)
        {
            var user = userManager.GetUserAsync(User).GetAwaiter().GetResult();
            if (ModelState.IsValid && id != null && user != null)
            {
                await reviewBusiness.Add(new FilmReview
                {
                    Content = input.Content,
                    CreatedOn = DateTime.UtcNow,
                    FilmId = id,
                    UserId = user.Id
                });
            }

            return RedirectToAction("Detail", "Films", new { Id = id });
            //return this.Redirect(Request.Headers["Referer"].ToString());
        }
    }
}