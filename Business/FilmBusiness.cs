using Data;
using Data.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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

        public async Task AddAsync(Film film)
        {
            await context.Films.AddAsync(film);
            await context.SaveChangesAsync();
        }

        public async Task<Film> GetAsync(string id)
        {
            return await context.Films.FindAsync(id);
        }

        public IEnumerable<Film> GetAll()
        {
            return context.Films.Include("FilmData").ToList();
        }

        public async Task UpdateAsync(Film film)
        {
            var filmInContext = await context.Films.FindAsync(film.Id);
            if(filmInContext!=null)
            {
                context.Entry(filmInContext).CurrentValues.SetValues(film);
                await context.SaveChangesAsync();
            }
        }

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
