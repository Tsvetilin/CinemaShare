using Data;
using Data.Models;
using System.Threading.Tasks;

namespace Business
{
    public class FilmReviewBusiness : IFilmReviewBusiness
    {
        private readonly CinemaDbContext context;
        
        public FilmReviewBusiness(CinemaDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Adds a new review in the database
        /// </summary>
        /// <param name="filmReview">The review to add</param>
        public async Task AddAsync(FilmReview filmReview)
        {
            await context.FilmReviews.AddAsync(filmReview);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Finds a review by ID
        /// </summary>
        /// <param name="cinema">The ID of the review</param>
        /// <returns>Review searched, returns null if not exists</returns>
        public async Task<FilmReview> GetAsync(string id)
        {
            return await context.FilmReviews.FindAsync(id);
        }

        /// <summary>
        /// Update the selected review if exists
        /// </summary>
        /// <param name="filmReview">An updated review of the film</param>
        public async Task UpdateAsync(FilmReview filmReview)
        {
            var filmInContext = await context.FilmReviews.FindAsync(filmReview.Id);
            if (filmInContext != null)
            {
                context.Entry(filmInContext).CurrentValues.SetValues(filmReview);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Delete a review with the selected ID if exists
        /// </summary>
        /// <param name="id">Review ID</param>
        public async Task DeleteAsync(string id)
        {
            var filmInContext = await context.FilmReviews.FindAsync(id);
            if (filmInContext != null)
            {
                context.FilmReviews.Remove(filmInContext);
                await context.SaveChangesAsync();
            }
        }
    }
}
