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

        //Done
        public async Task AddAsync(Film film)
        {
            await context.Films.AddAsync(film);
            await context.SaveChangesAsync();
        }

        //Done
        public async Task<Film> GetAsync(string id)
        {
            var film = await context.Films.FindAsync(id);
            return film;
        }

        //Done
        public IEnumerable<Film> GetAll()
        {
            return context.Films.ToList();
        }

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

        public async Task AddToWatchListAsync(string userId, Film film)
        {
            var userInContext = await context.Users.FindAsync(userId);
            if (userInContext != null)
            {
                userInContext.WatchList = userInContext.WatchList.Append(film).ToList();
                await context.SaveChangesAsync();
            }
        }

        public async Task RemoveFromWatchListAsync(string userId, Film film)
        {
            var userInContext = await context.Users.FindAsync(userId);
            if (userInContext != null)
            {
                if (userInContext.WatchList.Any(x => x.Id == film.Id))
                {
                    userInContext.WatchList = userInContext.WatchList.Where(x => x.Id != film.Id).ToList();
                    await context.SaveChangesAsync();
                }
            }
        }

        public IEnumerable<TModel> GetWatchList<TModel>(string userId, Func<FilmData, TModel> mapToModelFunc)
        {
            var userInContext = context.Users.FirstOrDefault(x => x.Id == userId);
            return userInContext?.WatchList?.Select(x => mapToModelFunc(x.FilmData)).ToList();
        }

        //Isn't used
        public async Task UpdateAsync(Film film)
        {
            var filmInContext = await context.Films.FindAsync(film.Id);
            if (filmInContext != null)
            {
                context.Entry(filmInContext).CurrentValues.SetValues(film);
                await context.SaveChangesAsync();
            }
        }

        //Done
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
