using Data;
using Data.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Business
{
    public class ProjectionTicketBusiness : IProjectionTicketBusiness
    {
        private readonly CinemaDbContext context;

        public ProjectionTicketBusiness(CinemaDbContext context)
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
            var cinemaDataInContext = await context.Cinemas.FindAsync(cinema.Id);
            if (cinemaDataInContext != null)
            {
                context.Entry(cinemaDataInContext).CurrentValues.SetValues(cinema);
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
