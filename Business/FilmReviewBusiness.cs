using Data;
using Data.Models;
using System.Collections.Generic;
using System.Data.Entity;
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
        /// Adds a new review
        /// </summary>
        /// <param name="filmReview">New review</param>
        /// <returns></returns>
        public async Task Add(FilmReview filmReview)
        {
            await context.FilmReviews.AddAsync(filmReview);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Finds a review bt ID
        /// </summary>
        /// <param name="cinema">The ID of the review</param>
        /// <returns>Review</returns>
        public async Task<FilmReview> Get(string id)
        {
            return await context.FilmReviews.FindAsync(id);
        }

        /// <summary>
        /// Gets all the reviews
        /// </summary>
        /// <returns>List with reviews </returns>
        public async Task<IEnumerable<FilmReview>> GetAll()
        {
            return await context.FilmReviews.ToListAsync();
        }

        /// <summary>
        /// Update the selected review if exists
        /// </summary>
        /// <param name="filmReview">A review of the film</param>
        /// <returns></returns>
        public async Task Update(FilmReview filmReview)
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
        /// <returns></returns>
        public async Task Delete(string id)
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
