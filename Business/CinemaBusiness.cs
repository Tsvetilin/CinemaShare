using Data;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Service layer logic
/// </summary>
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

        /// <summary>
        /// Adds cinema to the database
        /// </summary>
        /// <param name="cinema">New cinema object</param>
        public async Task AddAsync(Cinema cinema)
        {
            await context.Cinemas.AddAsync(cinema);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Gets the searched cinema by entered ID 
        /// </summary>
        /// <param name="id">The ID of the cinema to search</param>
        /// <returns>Searched cinema, returns null if not found</returns>
        public async Task<Cinema> GetAsync(string id)
        {
            return await context.Cinemas.FindAsync(id);
        }

        /// <summary>
        /// Gets cinema by ID.
        /// </summary>
        /// <param name = "id">The ID of the cinema</param>
        /// <param name="mapToModelFunc">Method that maps the Cinema to <typeparamref name="TModel"/></param>
        /// <typeparam name="TModel">Model, the Cinema model to be mapped to</typeparam>
        /// <returns>The cinema as a <typeparamref name="TModel"/></returns>
        public async Task<TModel> GetAsync<TModel>(string id, Func<Cinema, TModel> mapToModelFunc)
        {
            var cinema = await context.Cinemas.FindAsync(id);
            return mapToModelFunc(cinema);
        }

        /// <summary>
        /// Gets all the cinemas in the database
        /// </summary>
        /// <returns>All cinemas</returns>
        public IEnumerable<Cinema> GetAll()
        {
            return context.Cinemas.ToList();
        }

        /// <summary>
        /// Gets all the cinemas by searched name
        /// </summary>
        /// <param name="searchString">The name of the cinema</param>
        /// <param name="mapToModelFunc">Method that maps the Cinema to <typeparamref name="TModel"/></param>
        /// <typeparam name="TModel">Model, the Cinema model to be mapped to</typeparam>
        /// <returns>All the cinemas with the searched name mapped to <typeparamref name="TModel"/></returns>
        public IEnumerable<TModel> GetAllByName<TModel>(string searchString, Func<Cinema, TModel> mapToModelFunc)
        {
            return GetAll().Where(x => x.Name.ToLower().Contains(searchString.ToLower()))
                                                .Select(x => mapToModelFunc(x)).ToList();
        }

        /// <summary>
        /// Gets all the cinemas in the searched city
        /// </summary>
        /// <param name="searchString">The name of the city</param>
        /// <param name="mapToModelFunc">Method that maps the Cinema to <typeparamref name="TModel"/></param>
        /// <typeparam name="TModel">Model, the Cinema model to be mapped to</typeparam>
        /// <returns>
        /// A list with all the cinema names from the searched city mapped to <typeparamref name="TModel"/>
        /// </returns>
        public IEnumerable<TModel> GetAllByCity<TModel>(string searchString, Func<Cinema, TModel> mapToModelFunc)
        {
            return GetAll().Where(x => x.City.ToLower().Contains(searchString.ToLower()))
                                                .Select(x => mapToModelFunc(x)).ToList();
        }

        /// <summary>
        /// Gets the results from searching by name or city for a cinema.
        /// </summary>
        /// <param name="searchString">The name of the city or the name of the cinema to search for</param>
        /// <returns>List of matching cinemas mapped to <typeparamref name="TModel"/><returns>
        public IEnumerable<TModel> GetSearchResults<TModel>(string searchString, Func<Cinema, TModel> mapToModelFunc)
        {
            var result = new List<TModel>();
            var nameResults = GetAllByName(searchString, mapToModelFunc);
            var cityResults = GetAllByCity(searchString, mapToModelFunc);
            if (nameResults?.Count() != 0)
            {
                result.AddRange(nameResults);
            }
            if (cityResults?.Count() != 0)
            {
                result.AddRange(cityResults);
            }
            return result;
        }

        /// <summary>
        /// Gets all the cinema from particular page.
        /// </summary>
        /// <param name = "page">The number of the page</param>
        /// <param name = "cinemasOnPage">The number of the cinemas on the searched page</param>
        /// <param name="mapToModelFunc">Method that maps the Cinema to <typeparamref name="TModel"/></param>
        /// <typeparam name="TModel">Model, the Cinema model to be mapped to</typeparam>
        /// <returns>The cinemas on the selected page</returns>
        public IEnumerable<TModel> GetPageItems<TModel>(int page, int cinemasOnPage, Func<Cinema, TModel> mapToModelFunc)
        {
            var cinemas = GetAll();
            var selectedCinemas = cinemas.Skip(cinemasOnPage * (page - 1)).Take(cinemasOnPage);
            return selectedCinemas.Select(x => mapToModelFunc(x));
        }

        /// <summary>
        /// Update a cinema in the database
        /// and send email to people who had reserved tickets to inform about the changes.
        /// </summary>
        /// <param name="id">The id of the cinema</param>
        /// <param name="id">The URL of the projection</param>
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
                        await emailSender.SendTicketUpdateEmailAsync(ticket.Holder.Email, 
                                                                     ticket.Projection,
                                                                     ticketUrlPattern);
                    }
                }
                context.Entry(cinemaInContext).CurrentValues.SetValues(cinema);
                await context.SaveChangesAsync();
            }
        }
        
        /// <summary>
        /// Delete a cinema from the database,
        /// reject all the projections in the cinema 
        /// and send email to people who had reserved tickets to cancel their reservations
        /// </summary>
        /// <param name="id">The id of the cinema.</param>
        /// <param name="id">The URL of the projection.</param>
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
                        await emailSender.SendTicketCancelationEmailAsync(ticket.Holder.Email,
                                                                          ticket.Projection,
                                                                          projectionsUrlPattern);
                    }
                }
                context.Cinemas.Remove(cinemaInContext);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Checks if cinema is already added to the database
        /// </summary>
        /// <param name="cinemaName">The name of the searched cinema</param>
        /// <returns>True, if the name of the cinema exists, or false if not</returns>
        public bool IsAlreadyAdded(string cinemaName)
        {
            return context.Cinemas.Any(x => x.Name.ToLower().Equals(cinemaName.ToLower()));
        }

        /// <returns>
        ///The count of all cinemas in the database
        ///</returns>
        public int CountAllCinemas()
        {
            return context.Cinemas.Count();
        }
    }
}
