using Data;
using Data.Models;
using System.Collections.Generic;
using System.Data.Entity;
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

        public async Task Add(Film film)
        {
            await context.Films.AddAsync(film);
            await context.SaveChangesAsync();
        }

        public async Task<Film> Get(string id)
        {
            return await context.Films.FindAsync(id);
        }

        public async Task<IEnumerable<Film>> GetAll()
        {
            return await context.Films.ToListAsync();
        }

        public async Task Update (Film film)
        {
            var filmInContext = await context.Films.FindAsync(film.Id);
            if(filmInContext!=null)
            {
                context.Entry(filmInContext).CurrentValues.SetValues(film);
                await context.SaveChangesAsync();
            }
        }

        public async Task Delete (string id)
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
