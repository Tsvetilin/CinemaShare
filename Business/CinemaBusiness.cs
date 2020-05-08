using Data;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        public async Task Add(Cinema cinema)
        {
            await context.Cinemas.AddAsync(cinema);
            await context.SaveChangesAsync();
        }

        public async Task<Cinema> Get(string id)
        {
            return await context.Cinemas.FindAsync(id);
        }

        public async Task<IEnumerable<Cinema>> GetAll()
        {
            return await context.Cinemas.ToListAsync();
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
    }
}
