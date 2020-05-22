using Data;
using Data.Models;
using System;
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

        public async Task UpdateAsync(ProjectionTicket ticket)
        {
            var ticketDataInContext = await context.ProjectionTickets.FindAsync(ticket.Id);
            if (ticketDataInContext != null)
            {
                context.Entry(ticketDataInContext).CurrentValues.SetValues(ticket);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(string id)
        {
            var ticketInContext = await context.ProjectionTickets.FindAsync(id);
            if (ticketInContext != null)
            {
                context.ProjectionTickets.Remove(ticketInContext);
                await context.SaveChangesAsync();
            }
        }

        public IEnumerable<TModel> GetForUser<TModel>(string userId, Func<ProjectionTicket, TModel> mapToModelFunc)
        {
            var allTickets = GetAll().OrderByDescending(x=>x.Projection.Date).ToList();
            return allTickets.Where(x => x.HolderId == userId).Select(x=> mapToModelFunc(x)).ToList();
        }
    }
}
