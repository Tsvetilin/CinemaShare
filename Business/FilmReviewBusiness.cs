using Data;
using Data.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Business
{
    public class FilmReviewBusiness
    {
        private readonly CinemaDbContext context;

        public FilmReviewBusiness(CinemaDbContext context)
        {
            this.context = context;
        }

        public async Task Add(FilmReview filmReview)
        {
            await context.FilmReviews.AddAsync(filmReview);
            await context.SaveChangesAsync();
        }

        public async Task<FilmReview> Get(string id)
        {
            return await context.FilmReviews.FindAsync(id);
        }

        public async Task<IEnumerable<FilmReview>> GetAll()
        {
            return await context.FilmReviews.ToListAsync();
        }

        public async Task Update(FilmReview filmReview)
        {
            var filmInContext = await context.FilmReviews.FindAsync(filmReview.Id);
            if (filmInContext != null)
            {
                context.Entry(filmInContext).CurrentValues.SetValues(filmReview);
                await context.SaveChangesAsync();
            }
        }

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
