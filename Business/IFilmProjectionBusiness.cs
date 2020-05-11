using Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business
{
    public interface IFilmProjectionBusiness
    {
        public Task Add(FilmProjection filmProjection);

        public Task<FilmProjection> Get(string id);

        public Task<IEnumerable<FilmProjection>> GetAll();

        public Task Update(FilmProjection film);

        public Task Delete(string id);
    }
}
