using Data;
using Data.Models;
using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Adds multiple ticket's reservation
        /// </summary>
        public async Task AddMultipleAsync(IEnumerable<ProjectionTicket> tickets)
        {
            foreach (var ticket in tickets)
            {
                await context.ProjectionTickets.AddAsync(ticket);
            }
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Gets a ticket by ID
        /// </summary>
        /// <param>Ticket ID</param>
        /// <returns>Selected ticket, returns null if not found</returns>
        public async Task<ProjectionTicket> GetAsync(string id)
        {
            return await context.ProjectionTickets.FindAsync(id);
        }
        
        /// <summary>
        /// Gets all tickets
        /// </summary>
        /// <returns>List of all tickets</returns>
        public IEnumerable<ProjectionTicket> GetAll()
        {
            return context.ProjectionTickets.ToList();
        }

        /// <summary>
        /// Gets selected user's tickets for the selected projection 
        /// </summary>
        /// <param name="projectionId">The projection ID</param>
        /// <param name="userId">The user ID</param>
        /// <returns>List of tickets for the particular user and projection</returns>
        public IEnumerable<ProjectionTicket> GetForProjectionAndUser(string projectionId, string userId)
        {
            var allTickets = GetAll().ToList();
            return allTickets.Where(x=>x.Projection.Id==projectionId && x.HolderId==userId).ToList();
        }

        /// <summary>
        /// Updates information about a ticket
        /// </summary>
        /// <param name="ticket">Updated ticket object</param>
        public async Task UpdateAsync(ProjectionTicket ticket)
        {
            var ticketDataInContext = await context.ProjectionTickets.FindAsync(ticket.Id);
            if (ticketDataInContext != null)
            {
                context.Entry(ticketDataInContext).CurrentValues.SetValues(ticket);
                await context.SaveChangesAsync();
            }
        }
        
        /// <summary>
        /// Deletes ticket by ID
        /// </summary>
        /// <param name="id">The ticket ID</param>
        public async Task DeleteAsync(string id)
        {
            var ticketInContext = await context.ProjectionTickets.FindAsync(id);
            if (ticketInContext != null)
            {
                context.ProjectionTickets.Remove(ticketInContext);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Gets selected user's tickets
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="mapToModelFunc">Method that maps the Ticket to <typeparamref name="TModel"/></param>
        /// <typeparam name="TModel">Model, the Projection Ticket model to be mapped to</typeparam>
        /// <returns>List of tickets for the particular user</returns>
        public IEnumerable<TModel> GetForUser<TModel>(string userId, Func<ProjectionTicket, TModel> mapToModelFunc)
        {
            var allTickets = GetAll().OrderByDescending(x=>x.Projection.Date).ToList();
            return allTickets.Where(x => x.HolderId == userId).Select(x=> mapToModelFunc(x)).ToList();
        }
    }
}
