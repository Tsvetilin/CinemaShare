using Data;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class CinemaBusiness : ICinemaBusiness
    {
        private readonly CinemaDbContext context;

        public CinemaBusiness(CinemaDbContext context)
        {
            this.context = context;
        }

        public async Task AddAsync(Cinema cinema)
        {
            await context.Cinemas.AddAsync(cinema);
            await context.SaveChangesAsync();
        }

        public async Task<Cinema> Get(string id)
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
        public async Task Update(Cinema cinema)
        {
            var cinemaInContext = await context.Cinemas.FindAsync(cinema.Id);
            if (cinemaInContext != null)
            {
                context.Entry(cinemaInContext).CurrentValues.SetValues(cinema);
                await context.SaveChangesAsync();
            }
        }

        public async Task Delete(string id)
        {
            var cinemaInContext = await context.Cinemas.FindAsync(id);
            if (cinemaInContext != null)
            {
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
    }
}
