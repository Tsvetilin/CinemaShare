using Data;
using Data.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Business
{
    public class FilmProjectionBusiness
    {
        private readonly CinemaDbContext context;

        public FilmProjectionBusiness(CinemaDbContext context)
        {
            this.context = context;
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

        public async Task<IEnumerable<FilmProjection>> GetAll()
        {
            return await context.FilmProjections.ToListAsync();
        }

        public async Task Update(FilmProjection film)
        {
            var filmInContext = await context.FilmProjections.FindAsync(film.Id);
            if (filmInContext != null)
            {
                context.Entry(filmInContext).CurrentValues.SetValues(film);
                await context.SaveChangesAsync();
            }
        }

        public async Task Delete(string id)
        {
            var filmInContext = await context.FilmProjections.FindAsync(id);
            if (filmInContext != null)
            {
                context.FilmProjections.Remove(filmInContext);
                await context.SaveChangesAsync();
            }
        }
    }
}
