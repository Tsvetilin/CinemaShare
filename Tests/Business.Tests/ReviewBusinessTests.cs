using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Business;
using Data;
using Data.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using CinemaShare.Common.Mapping;

namespace Tests.Business.Tests
{
    class ReviewBusinessTests
    {
        [Test]
        public async Task AddAsyncAddsElementToDb()
        {
            // Arrange
            var reviews = new List<FilmReview>
            {
                new FilmReview {},
                new FilmReview {}
            }.AsQueryable();

            var mockSet = new Mock<DbSet<FilmReview>>();
            mockSet.As<IQueryable<FilmReview>>().Setup(m => m.Provider).Returns(reviews.Provider);
            mockSet.As<IQueryable<FilmReview>>().Setup(m => m.Expression).Returns(reviews.Expression);
            mockSet.As<IQueryable<FilmReview>>().Setup(m => m.ElementType).Returns(reviews.ElementType);
            mockSet.As<IQueryable<FilmReview>>().Setup(m => m.GetEnumerator()).Returns(reviews.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.FilmReviews).Returns(mockSet.Object);
            mockContext.Setup(m => m.AddAsync(It.IsAny<FilmReview>(), It.IsAny<CancellationToken>()))
                .Returns(new ValueTask<EntityEntry<FilmReview>>(Task.FromResult((EntityEntry<FilmReview>)null)));

            var filmReviewBusiness = new FilmReviewBusiness(mockContext.Object);
            var mapper = new Mapper();
            var filmReview = new FilmReview { };

            // Act
            await filmReviewBusiness.AddAsync(filmReview);

            // Assert
            mockSet.Verify(m => m.AddAsync(It.IsAny<FilmReview>(), It.IsAny<CancellationToken>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task GetAsyncReturnsElementFromDb()
        {
            // Arrange
            var reviews = new List<FilmReview>
            {
                new FilmReview {},
                new FilmReview {}
            }.AsQueryable();

            var mockSet = new Mock<DbSet<FilmReview>>();
            mockSet.As<IQueryable<FilmReview>>().Setup(m => m.Provider).Returns(reviews.Provider);
            mockSet.As<IQueryable<FilmReview>>().Setup(m => m.Expression).Returns(reviews.Expression);
            mockSet.As<IQueryable<FilmReview>>().Setup(m => m.ElementType).Returns(reviews.ElementType);
            mockSet.As<IQueryable<FilmReview>>().Setup(m => m.GetEnumerator()).Returns(reviews.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.FilmReviews).Returns(mockSet.Object);
            mockContext.Setup(c => c.FilmReviews.FindAsync(It.IsAny<string>())).
                        Returns(new ValueTask<FilmReview>(reviews.First()));


            var filmReviewBusiness = new FilmReviewBusiness(mockContext.Object);

            // Act
            var filmReview = await filmReviewBusiness.GetAsync(reviews.First().Id);

            // Assert
            Assert.AreEqual(reviews.First().Id, filmReview.Id, "Doesn't return the searched review from database");
        }
    }
}
