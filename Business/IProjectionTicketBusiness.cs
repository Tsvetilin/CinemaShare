using Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business
{
    public interface IProjectionTicketBusiness
    {
        public Task AddMultipleAsync(IEnumerable<ProjectionTicket> tickets);
               
        public Task<ProjectionTicket> GetAsync(string id);

        public IEnumerable<ProjectionTicket> GetAll();

        public IEnumerable<ProjectionTicket> GetForProjectionAndUser(string projectionId, string userId);
        
        public IEnumerable<TModel> GetForUser<TModel>(string userId, Func<ProjectionTicket, TModel> mapToModelFunc);

        public Task UpdateAsync(ProjectionTicket ticket);

        public Task DeleteAsync(string id);
    }
}
