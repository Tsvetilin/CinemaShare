using Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business
{
    public interface IFilmReviewBusiness
    {
        public Task Add(FilmReview filmReview);

        public Task<FilmReview> Get(string id);

        public Task<IEnumerable<FilmReview>> GetAll();

        public Task Update(FilmReview filmReview);

        public Task Delete(string id);
    }
}
