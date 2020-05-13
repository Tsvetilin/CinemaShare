using Data;
using Data.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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

        public async Task AddMultipleAsync(IEnumerable<ProjectionTicket> tickets)
        {
            foreach (var ticket in tickets)
            {
                await context.ProjectionTickets.AddAsync(ticket);
            }
            await context.SaveChangesAsync();
        }

        public async Task<ProjectionTicket> GetAsync(string id)
        {
            return await context.ProjectionTickets.FindAsync(id);
        }

        public IEnumerable<ProjectionTicket> GetAll()
        {
            return context.ProjectionTickets.ToList();
        }

        public IEnumerable<ProjectionTicket> GetForProjectionAndUser(string projectionId, string userId)
        {
            var allTickets = GetAll().ToList();
            return allTickets.Where(x=>x.Projection.Id==projectionId && x.HolderId==userId).ToList();
        }

        public IEnumerable<ProjectionTicket> GetForUser(string userId)
        {
            var allTickets = GetAll().ToList();
            return allTickets.Where(x => x.HolderId == userId).ToList();
        }

        public async Task UpdateAsync(ProjectionTicket ticket)
        {
            var cinemaDataInContext = await context.Cinemas.FindAsync(ticket.Id);
            if (cinemaDataInContext != null)
            {
                context.Entry(cinemaDataInContext).CurrentValues.SetValues(ticket);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(string id)
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
