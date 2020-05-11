using Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business
{
    public interface IProjectionTicketBusiness
    {
        public Task Add(Cinema cinema);

        public Task<Cinema> Get(string id);

        public Task<IEnumerable<Cinema>> GetAll();

        public Task Update(Cinema cinema);

        public Task Delete(string id);
    }
}
