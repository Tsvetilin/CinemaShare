using Data;
using Data.Enums;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Business
{
    public class FilmProjectionBusiness : IFilmProjectionBusiness
    {
        private readonly CinemaDbContext context;
        private readonly IEmailSender emailSender;

        public FilmProjectionBusiness(CinemaDbContext context, IEmailSender emailSender)
        {
            this.context = context;
            this.emailSender = emailSender;
        }

        /// <summary>
        /// Adds new projection
        /// </summary>
        /// <param name="filmProjection">New projection object</param>
        /// <returns>The results from searching</returns>
        public async Task Add(FilmProjection filmProjection)
        {
            await context.FilmProjections.AddAsync(filmProjection);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Finds a projection by ID
        /// </summary>
        /// <param name="id">Projection id</param>
        /// <returns>Projection</returns>
        public async Task<FilmProjection> Get(string id)
        {
            return await context.FilmProjections.FindAsync(id);
        }
        /// <summary>
        /// Gets all projections' count
        /// </summary>
        /// <returns>Count</returns>
        public int CountAllProjections()
        {
            return context.FilmProjections.Count();

        }
        
        /// <summary>
        /// Gets all projections on the selected page
        /// </summary>
        /// <param name="page">Number of the page</param>
        /// <param name="projectionsOnPage">Projections Count</param>
        /// <returns>List of Projections</returns>
        public IEnumerable<TModel> GetPageItems<TModel>(int page, int projectionsOnPage, Func<FilmProjection, TModel> mapToModelFunc)
        {
            var projections = GetAll().OrderByDescending(x=>x.Date);
            var selectedCinemas = projections.Skip(projectionsOnPage * (page - 1)).Take(projectionsOnPage);
            return selectedCinemas.Select(x => mapToModelFunc(x)).ToList();
        }

        /// <summary>
        /// Gets projection by ID
        /// </summary>
        /// <param name="id">Projection ID</param>
        /// <param name="projectionsOnPage">Projections Count</param>
        /// <returns>Projection</returns>
        public async Task<TModel> GetAsync<TModel>(string id, Func<FilmProjection, TModel> mapToModelFunc)
        {
            var projection = await Get(id);
            return mapToModelFunc(projection);
        }

        /// <summary>
        /// Gets all projections
        /// </summary>
        /// <returns>List of all Projections</returns>
        public IEnumerable<FilmProjection> GetAll()
        {
            return context.FilmProjections.ToList();
        }

       /// <summary>
        /// Gets all projections from selected cinema
        /// </summary>
        /// <param name="page">Id of the cinema</param>
        /// <param name="projectionsOnPage">Projections Count</param>
        /// <returns>List of Projections</returns>
        public IEnumerable<TModel> GetAllByCinemaId<TModel>(string cinemaId, Func<FilmProjection, TModel> mapToModelFunc)
        {
            return context.FilmProjections.Where(x => x.CinemaId == cinemaId).ToList().Select(x => mapToModelFunc(x)).ToList();
        }

        /// <summary>
        /// Gets projection as TModel
        /// </summary>
        /// <returns>List of Projections</returns>
        public IEnumerable<TModel> GetAll<TModel>(Func<FilmProjection, TModel> mapToModelFunc)
        {
            return GetAll().Select(x => mapToModelFunc(x)).ToList();
        }
        /// <summary>
        /// Updates information for a selected projection
        /// and send email to infrom about the update
        /// </summary>
        /// <param name="projection">New projection object</param>
        /// <returns></returns>
        public async Task Update(FilmProjection projection, string projectionsUrlPattern, string ticketsUrlPattern)
        {
            var projectionInContext = await context.FilmProjections.FindAsync(projection.Id);
            if (projectionInContext != null)
            {
                foreach (var ticket in context.ProjectionTickets.Where(x => x.ProjectionId == projection.Id).ToList())
                {
                    if (ticket.Seat > projection.TotalTickets)
                    {
                        await emailSender.SendTicketCancelationEmailAsync(ticket.Holder.Email, projectionInContext, projectionsUrlPattern);
                        context.ProjectionTickets.Remove(ticket);
                    }
                    else
                    {
                        switch (ticket.Type)
                        {
                            case TicketType.Adult: ticket.Price = projection.TicketPrices.AdultPrice; break;
                            case TicketType.Children: ticket.Price = projection.TicketPrices.ChildrenPrice; break;
                            case TicketType.Student: ticket.Price = projection.TicketPrices.StudentPrice; break;
                        }

                        await emailSender.SendTicketUpdateEmailAsync(ticket.Holder.Email, projectionInContext, ticketsUrlPattern);
                    }
                }
                context.Entry(projectionInContext).CurrentValues.SetValues(projection);
                projectionInContext.TicketPrices.AdultPrice = projection.TicketPrices.AdultPrice;
                projectionInContext.TicketPrices.StudentPrice = projection.TicketPrices.StudentPrice;
                projectionInContext.TicketPrices.ChildrenPrice = projection.TicketPrices.ChildrenPrice;
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Deletes a projection from the database and
        /// sends email for reservations cancellation
        /// </summary>
        /// <param name="id">ID of the projection</param>
        /// <param name="projectionsOnPage">Projections Count</param>
        /// <returns></returns>
        public async Task Delete(string id, string projectionsUrlPattern)
        {
            var projectionInContext = await context.FilmProjections.FindAsync(id);
            if (projectionInContext != null)
            {
                foreach (var ticket in context.ProjectionTickets.Where(x => x.ProjectionId == projectionInContext.Id).ToList())
                {
                    context.ProjectionTickets.Remove(ticket);
                    await emailSender.SendTicketCancelationEmailAsync(ticket.Holder.Email, projectionInContext, projectionsUrlPattern);
                }
                context.FilmProjections.Remove(projectionInContext);
                await context.SaveChangesAsync();
            }
        }
    }
}
