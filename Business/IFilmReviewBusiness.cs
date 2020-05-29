using Data.Models;
using System.Threading.Tasks;

namespace Business
{
    public interface IFilmReviewBusiness
    {
        public Task AddAsync(FilmReview filmReview);

        public Task<FilmReview> GetAsync(string id);

        public Task UpdateAsync(FilmReview filmReview);

        public Task DeleteAsync(string id);
    }
}
