using Data;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business
{
    public class FilmBusiness : IFilmBusiness
    {
        private readonly CinemaDbContext context;

        public FilmBusiness(CinemaDbContext context)
        {
            this.context = context;
        }
        
        /// <summary>
        /// Adds film to the database
        /// </summary>
        /// <param name="film">New film object</param>
        public async Task AddAsync(Film film)
        {
            await context.Films.AddAsync(film);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Finds film by ID
        /// </summary>
        /// <param name="id">The film ID</param>
        /// <returns>Selected film, returns null if not found</returns>
        public async Task<Film> GetAsync(string id)
        {
            var film = await context.Films.FindAsync(id);
            return film;
        }

        /// <summary>
        /// Gets all films from the database
        /// </summary>
        public IEnumerable<Film> GetAll()
        {
            return context.Films.ToList();
        }

        /// <summary>
        /// Gets the selected user's watchlist 
        /// </summary>
        /// <param name="userId">ID of the selected user</param>
        /// <param name="mapToModelFunc">Method that maps the Film to <typeparamref name="TModel"/></param>
        /// <typeparam name="TModel">Model, the Film model to be mapped to</typeparam>
        /// <returns>List of films mapped to <typeparamref name="TModel"/></returns>
        public IEnumerable<TModel> GetWatchList<TModel>(string userId, Func<FilmData, TModel> mapToModelFunc)
        {
            var userInContext = context.Users.FirstOrDefault(x => x.Id == userId);
            return userInContext?.WatchList.Select(x => mapToModelFunc(x.FilmData)).ToList();
        }

        /// <summary>
        /// Adds film to the user's watchlist
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <param name="film">Film to be added</param>
        public async Task AddToWatchListAsync(string userId, Film film)
        {
            var userInContext = await context.Users.FindAsync(userId);
            if (userInContext?.WatchList.Any(x => x.Id == film.Id) ?? true)
            {
                return;
            }

            userInContext.WatchList = userInContext.WatchList.Append(film).ToList();
            await context.SaveChangesAsync();

        }

        /// <summary>
        /// Removes film from the user's watchlist
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <param name="film">New film object</param>
        /// <returns></returns>
        public async Task RemoveFromWatchListAsync(string userId, Film film)
        {
            var userInContext = await context.Users.FindAsync(userId);
            if (userInContext?.WatchList.Any(x => x.Id == film.Id) ?? false)
            {
                    userInContext.WatchList = userInContext.WatchList.Where(x => x.Id != film.Id).ToList();
                    await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Adds the selected user's rating to a selected film
        /// </summary>
        /// <param name="filmId">The film ID</param>
        /// <param name="userId">The user ID</param>
        /// <param name="rating">New film rating</param>
        public async Task RateAsync(string filmId, string userId, int rating)
        {
            var filmInContext = await context.Films.FindAsync(filmId);
            if (userId == null || filmInContext == null)
            {
                return;
            }

            if (filmInContext.Ratings.Any(x => x.UserId == userId))
            {
                filmInContext.Ratings.First(x => x.UserId == userId).Rating = rating;
            }
            else
            {
                filmInContext.Ratings = filmInContext.Ratings.Append(new FilmRating
                {
                    Rating = rating,
                    UserId = userId
                }).ToList();
            }
            filmInContext.Rating = (double)filmInContext.Ratings.Select(x => x.Rating).Sum() /
                                                                filmInContext.Ratings.Count();
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes film from the database by ID
        /// </summary>
        /// <param name="id">The film ID</param>
        public async Task DeleteAsync(string id)
        {
            var filmInContext = await context.Films.FindAsync(id);
            if (filmInContext != null)
            {
                context.Films.Remove(filmInContext);
                await context.SaveChangesAsync();
            }
        }
    }
}
