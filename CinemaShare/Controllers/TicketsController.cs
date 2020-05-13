using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business;
using CinemaShare.Common.Mapping;
using CinemaShare.Models.InputModels;
using CinemaShare.Models.ViewModels;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CinemaShare.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class TicketsController : Controller
    {
        private readonly IFilmProjectionBusiness filmProjectionBusiness;
        private readonly IProjectionTicketBusiness projectionTicketBusiness;
        private readonly IMapper mapper;
        private readonly UserManager<CinemaUser> userManager;

        public TicketsController(IFilmProjectionBusiness filmProjectionBusiness,
                                 IProjectionTicketBusiness projectionTicketBusiness,
                                 IMapper mapper,
                                 UserManager<CinemaUser> userManager)
        {
            this.filmProjectionBusiness = filmProjectionBusiness;
            this.projectionTicketBusiness = projectionTicketBusiness;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(User);
            var tickets = projectionTicketBusiness.GetForUser(user.Id);
            return View(new TicketsIndexViewModel { });
        }

        [Authorize]
        public async Task<IActionResult> Reserve(string id)
        {
            var projection = await filmProjectionBusiness.Get(id);
            if (projection == null)
            {
                return RedirectToAction("Index", "Projections");
            }
            var soldSeats = projection.ProjectionTickets.Select(x => x.Seat).ToList();
            var availableSeats = Enumerable.Range(1, projection.TotalTickets).Where(x => !soldSeats.Contains(x));
            var orderedAvailableSeats = availableSeats.OrderBy(x => x).ToDictionary(x => x.ToString());
            ReserveTicketInputModel inputModel = new ReserveTicketInputModel
            {
                AvailableSeats = new SelectList(orderedAvailableSeats, "Key", "Value"),
                ChildrenPrice= projection.TicketPrices.ChildrenPrice,
                StudentPrice = projection.TicketPrices.StudentPrice,
                AdultPrice = projection.TicketPrices.AdultPrice,
            };
            return View(inputModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Reserve(string id, ReserveTicketInputModel input)
        {
            var ticketsInput = input.TicketInputModels.Select(x=>x.Value);
            var projection = await filmProjectionBusiness.Get(id);
            if (projection == null)
            {
                return RedirectToAction("Index", "Projections");
            }

            //If same seats add error
            if (ticketsInput.Any(ticket=> ticketsInput.Count(others => others.Seat == ticket.Seat) != 1))
            {
                ModelState.AddModelError("Duplicate", "Can't buy same seats more than once");
            }
            if (!ModelState.IsValid)
            {
                return this.View(input);
            }

            var userId = userManager.GetUserId(User);
            DateTime timeStamp = DateTime.UtcNow;
            var tickets = ticketsInput.Select(ticket => mapper.MapToProjectionTicket(userId, ticket, projection,timeStamp));
            await projectionTicketBusiness.AddMultipleAsync(tickets);
            return RedirectToAction("Index", "Tickets");
        }

        [Authorize]
        public async Task<IActionResult> Detail(string id)
        {
            return View(new TicketDataViewModel { });
        }
    }
}