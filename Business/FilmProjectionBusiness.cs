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

        public async Task Add(FilmProjection filmProjection)
        {
            await context.FilmProjections.AddAsync(filmProjection);
            await context.SaveChangesAsync();
        }

        public async Task<FilmProjection> Get(string id)
        {
            return await context.FilmProjections.FindAsync(id);
        }

        public int CountAllProjections()
        {
            return context.FilmProjections.Count();

        }
        public IEnumerable<TModel> GetPageItems<TModel>(int page, int projectionsOnPage, Func<FilmProjection, TModel> mapToModelFunc)
        {
            var projections = GetAll().OrderByDescending(x=>x.Date);
            var selectedCinemas = projections.Skip(projectionsOnPage * (page - 1)).Take(projectionsOnPage);
            return selectedCinemas.Select(x => mapToModelFunc(x)).ToList();
        }

        public async Task<TModel> GetAsync<TModel>(string id, Func<FilmProjection, TModel> mapToModelFunc)
        {
            var projection = await Get(id);
            return mapToModelFunc(projection);
        }

        public IEnumerable<FilmProjection> GetAll()
        {
            return context.FilmProjections.ToList();
        }

        public IEnumerable<TModel> GetAllByCinemaId<TModel>(string cinemaId, Func<FilmProjection, TModel> mapToModelFunc)
        {
            return context.FilmProjections.Where(x => x.CinemaId == cinemaId).ToList().Select(x => mapToModelFunc(x)).ToList();
        }

        public IEnumerable<TModel> GetAll<TModel>(Func<FilmProjection, TModel> mapToModelFunc)
        {
            return GetAll().Select(x => mapToModelFunc(x)).ToList();
        }

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
