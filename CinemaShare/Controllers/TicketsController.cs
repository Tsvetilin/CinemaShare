using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private const int ticketsOnPage = 3;
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

        ///<summary>
        /// Redirects to a new TicketIndex view model 
        ///</summary>
        [Authorize]
        public async Task<IActionResult> Index(int id = 1)
        {
            var user = await userManager.GetUserAsync(User);
            var tickets = projectionTicketBusiness.GetForUser(user.Id, mapper.MapToTicketCardViewModel).ToList();
            int pageCount = (int)Math.Ceiling((double)tickets.Count() / ticketsOnPage);
            if (id > pageCount || id < 1)
            {
                id = 1;
            }

            TicketsIndexViewModel viewModel = new TicketsIndexViewModel
            {
                PagesCount = pageCount,
                CurrentPage = id,
                Tickets = tickets,
            };

            return View(viewModel);
        }

        ///<summary>
        /// Searchs projection by a selected ID,
        /// gets the projection data and creates new ReservationTicketInputModel
        /// for reserving tickets where seats are empty
        ///</summary>
        ///<returns>ReservationTicketInputModel view</returns>
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
                ChildrenPrice = projection.TicketPrices.ChildrenPrice,
                StudentPrice = projection.TicketPrices.StudentPrice,
                AdultPrice = projection.TicketPrices.AdultPrice,
            };
            return View(inputModel);
        }

        ///<summary>
        /// Search projection by a selected ID,
        /// gets the projection data and a ReserveTicketInputModel 
        /// for reserving tickets where seats are empty, 
        /// checks for conflicts with taken seats and concurrency reservations
        ///</summary>
        ///<returns>To Projections reservations page</returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Reserve(string id, ReserveTicketInputModel input)
        {
            var ticketsInput = input.TicketInputModels.Select(x => x.Value);
            var projection = await filmProjectionBusiness.Get(id);
            if (projection == null)
            {
                return RedirectToAction("Index", "Projections");
            }

            //If same seats are added add error
            if (ticketsInput.Any(ticket => ticketsInput.Count(others => others.Seat == ticket.Seat) != 1))
            {
                ModelState.AddModelError("Duplicate", "Can't buy same seats more than once");
            }

            //Check for concurrency reservation conflicts
            var soldSeats = projection.ProjectionTickets.Select(x => x.Seat).ToList();
            var availableSeats = Enumerable.Range(1, projection.TotalTickets).Where(x => !soldSeats.Contains(x));
            if (ticketsInput.Any(ticket => !availableSeats.Contains(ticket.Seat)))
            {
                ModelState.AddModelError("Reserved", "Someone got ahead of you and reserved one of your tickets.");
            }

            if (!ModelState.IsValid)
            {

                var orderedAvailableSeats = availableSeats.OrderBy(x => x).ToDictionary(x => x.ToString());
                input.AvailableSeats = new SelectList(orderedAvailableSeats, "Key", "Value");
                return this.View(input);
            }

            var userId = userManager.GetUserId(User);
            DateTime timeStamp = DateTime.UtcNow;
            var tickets = ticketsInput.Select(ticket => mapper.MapToProjectionTicket(userId, ticket, projection, timeStamp));
            await projectionTicketBusiness.AddMultipleAsync(tickets);
            return RedirectToAction("Index", "Tickets");
        }

        ///<summary>
        /// Updates data for a ticket by selected ID
        ///</summary>
        /// <returns>New UpdateTicketInputModel view</returns>
        [Authorize]
        public async Task<IActionResult> Update(string id)
        {
            var user = await userManager.GetUserAsync(User);
            var ticket = await projectionTicketBusiness.GetAsync(id);

            if (ticket == null || ticket.Projection.Date<DateTime.UtcNow)
            {
                return this.RedirectToAction("Index", "Tickets");
            }

            var soldSeats = ticket.Projection.ProjectionTickets.Select(x => x.Seat).ToList();
            var availableSeats = Enumerable.Range(1, ticket.Projection.TotalTickets).Where(x => !soldSeats.Contains(x));
            availableSeats = availableSeats.Append(ticket.Seat);
            var orderedAvailableSeats = availableSeats.OrderBy(x => x).ToDictionary(x => x.ToString());
            UpdateTicketInputModel inputModel = new UpdateTicketInputModel
            {
                Ticket = new TicketInputModel { Seat = ticket.Seat, TicketType = ticket.Type },
                AvailableSeats = new SelectList(orderedAvailableSeats, "Key", "Value"),
                ChildrenPrice = ticket.Projection.TicketPrices.ChildrenPrice,
                StudentPrice = ticket.Projection.TicketPrices.StudentPrice,
                AdultPrice = ticket.Projection.TicketPrices.AdultPrice,
            };
            return View(inputModel);
        }

        ///<summary>
        /// Redirect to a new cinema view model if seаrched string is valid 
        /// or shows an error
        ///</summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Update(string id, UpdateTicketInputModel input)
        {
            var ticket = await projectionTicketBusiness.GetAsync(id);
            var projection = ticket.Projection;

            if (ticket == null || ticket.Projection.Date < DateTime.UtcNow)
            {
                return this.RedirectToAction("Index", "Tickets");
            }
            if (!ModelState.IsValid)
            {
                return this.View(input);
            }

            var userId = userManager.GetUserId(User);
            DateTime timeStamp = DateTime.UtcNow;
            var updatedTicket = mapper.MapToProjectionTicket(userId, input.Ticket, projection, timeStamp);
            updatedTicket.Id = id;
            await projectionTicketBusiness.UpdateAsync(updatedTicket);
            return this.RedirectToAction("Index", "Tickets", new { Id = id });
        }

        ///<summary>
        /// Deletes ticket by a Selected ID 
        ///</summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var ticket = await projectionTicketBusiness.GetAsync(id);
            if (ticket != null)
            {
                await projectionTicketBusiness.DeleteAsync(id);
            }
            return this.RedirectToAction("Index", "Tickets");
        }
    }
}
