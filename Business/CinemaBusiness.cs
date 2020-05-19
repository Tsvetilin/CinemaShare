using Data;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Business
{
    public class CinemaBusiness : ICinemaBusiness
    {
        private readonly CinemaDbContext context;
        private readonly IEmailSender emailSender;

        public CinemaBusiness(CinemaDbContext context, IEmailSender emailSender)
        {
            this.context = context;
            this.emailSender = emailSender;
        }

        public async Task AddAsync(Cinema cinema)
        {
            await context.Cinemas.AddAsync(cinema);
            await context.SaveChangesAsync();
        }

        public async Task<Cinema> GetAsync(string id)
        {
            return await context.Cinemas.FindAsync(id);
        }

        public IEnumerable<Cinema> GetAll()
        {
            return context.Cinemas.ToList();
        }

        public int CountAllCinemas()
        {
            return context.Cinemas.Count();

        }
        public IEnumerable<TModel> GetPageItems<TModel>(int page, int cinemasOnPage, Func<Cinema, TModel> mapToModelFunc)
        {
            var cinemas = GetAll();
            var selectedCinemas = cinemas.Skip(cinemasOnPage * (page - 1)).Take(cinemasOnPage);
            return selectedCinemas.Select(x => mapToModelFunc(x));
        }

        public async Task<TModel> GetAsync<TModel>(string id, Func<Cinema, TModel> mapToModelFunc)
        {
            var cinema = await context.Cinemas.FindAsync(id);
            return mapToModelFunc(cinema);
        }

        public async Task UpdateAsync(Cinema cinema, string ticketUrlPattern)
        {
            var cinemaInContext = await context.Cinemas.FindAsync(cinema.Id);
            if (cinemaInContext != null)
            {
                var projections = context.FilmProjections.Where(x => x.CinemaId == cinemaInContext.Id).ToList();
                foreach (var projection in projections)
                {
                    var projectionTickets = projection.ProjectionTickets.ToList();
                    foreach (var ticket in projectionTickets)
                    {
                        await emailSender.SendTicketUpdateEmailAsync(ticket.Holder.Email, ticket.Projection, ticketUrlPattern);
                    }
                }
                context.Entry(cinemaInContext).CurrentValues.SetValues(cinema);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(string id, string projectionsUrlPattern)
        {
            var cinemaInContext = await context.Cinemas.FindAsync(id);
            if (cinemaInContext != null)
            {
                var projections = context.FilmProjections.Where(x => x.CinemaId == cinemaInContext.Id).ToList();
                foreach (var projection in projections)
                {
                    var projectionTickets = projection.ProjectionTickets.ToList();
                    foreach (var ticket in projectionTickets)
                    {
                        await emailSender.SendTicketCancelationEmailAsync(ticket.Holder.Email, ticket.Projection, projectionsUrlPattern);
                    }
                }
                context.Cinemas.Remove(cinemaInContext);
                await context.SaveChangesAsync();
            }
        }

        public bool IsAlreadyAdded(string cinemaName)
        {
            return context.Cinemas.Any(x => x.Name.ToLower().Equals(cinemaName.ToLower()));
        }

        public IEnumerable<TModel> GetAllByName<TModel>(string searchString, Func<Cinema, TModel> mapToModelFunc)
        {
            return GetAll().Where(x => x.Name.ToLower().Contains(searchString.ToLower()))
                                                .Select(x => mapToModelFunc(x)).ToList();
        }

        public IEnumerable<TModel> GetAllByCity<TModel>(string searchString, Func<Cinema, TModel> mapToModelFunc)
        {
            return GetAll().Where(x => x.City.ToLower().Contains(searchString.ToLower()))
                                                .Select(x => mapToModelFunc(x)).ToList();
        }

        public IEnumerable<TModel> GetSearchResults<TModel>(string searchString, Func<Cinema, TModel> mapToModelFunc)
        {
            var result = new List<TModel>();
            var nameResults = GetAllByName(searchString, mapToModelFunc);
            var cityResults = GetAllByCity(searchString, mapToModelFunc);
            if(nameResults?.Count()!=0)
            {
                result.AddRange(nameResults);
            }
            if (cityResults?.Count() != 0)
            {
                result.AddRange(cityResults);
            }
            return result;
        }

    }
}
