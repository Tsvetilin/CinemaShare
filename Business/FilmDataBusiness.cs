using Data;
using Data.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Business
{
    public class FilmDataBusiness : IFilmDataBusiness
    {
        private readonly CinemaDbContext context;

        public FilmDataBusiness(CinemaDbContext context)
        {
            this.context = context;
        }

        public async Task Add(FilmData filmData)
        {
            await context.FilmDatas.AddAsync(filmData);
            await context.SaveChangesAsync();
        }

        public async Task<FilmData> Get(string id)
        {
            return await context.FilmDatas.FindAsync(id);
        }

        public IEnumerable<FilmData> GetAll()
        {
            return context.FilmDatas.Include(x=>x.Film).ToList();
        }

        public async Task Update(FilmData filmData)
        {
            var filmDataInContext = await context.Films.FindAsync(filmData.Film);
            if (filmDataInContext != null)
            {
                context.Entry(filmDataInContext).CurrentValues.SetValues(filmData);
                await context.SaveChangesAsync();
            }
        }

        public async Task Delete(string id)
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
