using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Business;
using Data;
using Data.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using CinemaShare.Common.Mapping;

namespace Tests.Business.Tests
{
    class FilmBusinessTests
    {
        [Test]
        public void FilmBusinessGetAllReturnsAllElements()
        {
            // Arrange
            var films = new List<Film>
            {
                new Film {Rating=  2},
                new Film {Rating = 3}
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Film>>();
            mockSet.As<IQueryable<Film>>().Setup(m => m.Provider).Returns(films.Provider);
            mockSet.As<IQueryable<Film>>().Setup(m => m.Expression).Returns(films.Expression);
            mockSet.As<IQueryable<Film>>().Setup(m => m.ElementType).Returns(films.ElementType);
            mockSet.As<IQueryable<Film>>().Setup(m => m.GetEnumerator()).Returns(films.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Films).Returns(mockSet.Object);

            var filmBusiness = new FilmBusiness(mockContext.Object);

            // Act
            var allFilms = filmBusiness.GetAll().ToList();

            // Assert
            Assert.AreEqual(2, allFilms.Count, "Doesn't return all elements in the database.");
        }

        [Test]
        public async Task FilmBusinessGetAsyncReturnsSearchedElement()
        {
            // Arrange
            var films = new List<Film>
            {
                new Film {Rating=  2},
                new Film {Rating = 3}
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Film>>();
            mockSet.As<IQueryable<Film>>().Setup(m => m.Provider).Returns(films.Provider);
            mockSet.As<IQueryable<Film>>().Setup(m => m.Expression).Returns(films.Expression);
            mockSet.As<IQueryable<Film>>().Setup(m => m.ElementType).Returns(films.ElementType);
            mockSet.As<IQueryable<Film>>().Setup(m => m.GetEnumerator()).Returns(films.GetEnumerator());
            var mockContext = new Mock<CinemaDbContext>();
            var searchedFilm = films.First(x=>x.Rating==2);
            mockContext.Setup(c => c.Films).Returns(mockSet.Object);

            var filmBusiness = new FilmBusiness(mockContext.Object);

            // Act
            var resultFilm =  filmBusiness.GetAsync(searchedFilm.Id).GetAwaiter().GetResult();

            // Assert
            Assert.AreEqual(resultFilm.Id, searchedFilm.Id, "Doesn't return searched element from the database.");
            Assert.AreEqual(resultFilm.Rating, searchedFilm.Rating, "Doesn't return searched element from the database.");
            /*  Assert.AreEqual(resultFilm.AddedByUser, films.First().AddedByUser, "Doesn't return searched element from the database.");
              Assert.AreEqual(resultFilm.AddedByUserId, films.First().AddedByUserId, "Doesn't return searched element from the database.");
              Assert.AreEqual(resultFilm.FilmData, films.First().FilmData, "Doesn't return searched element from the database.");
              Assert.AreEqual(resultFilm.FilmProjection, films.First().FilmProjection, "Doesn't return searched element from the database.");
              Assert.AreEqual(resultFilm.FilmReviews, films.First().FilmReviews, "Doesn't return searched element from the database.");
              Assert.AreEqual(resultFilm.OnWatchlistUsers, films.First().OnWatchlistUsers, "Doesn't return searched element from the database.");
              Assert.AreEqual(resultFilm.Rating, films.First().Rating, "Doesn't return searched element from the database.");
              Assert.AreEqual(resultFilm.Ratings, films.First().Ratings, "Doesn't return searched element from the database.");
      */
        }

        [Test]
        public async Task FilmBusinessAddAsyncAddsElement()
        {
            // Arrange
            var films = new List<Film>
            {
                new Film {Rating=  2},
                new Film {Rating = 3}
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Film>>();
            mockSet.As<IQueryable<Film>>().Setup(m => m.Provider).Returns(films.Provider);
            mockSet.As<IQueryable<Film>>().Setup(m => m.Expression).Returns(films.Expression);
            mockSet.As<IQueryable<Film>>().Setup(m => m.ElementType).Returns(films.ElementType);
            mockSet.As<IQueryable<Film>>().Setup(m => m.GetEnumerator()).Returns(films.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Films).Returns(mockSet.Object);

            var filmBusiness = new FilmBusiness(mockContext.Object);

            // Act
            var film = new Film() { Rating = 4 };
            await filmBusiness.AddAsync(film);

            // Assert
            Assert.AreEqual(2, mockContext.Object.Films.Count(), "Doesn't add all elements in the database.");
        }

        [Test]
        public async Task FilmBusinessDeleteAsyncAddsElement()
        {
            // Arrange
            var films = new List<Film>
            {
                new Film {Rating=  2},
                new Film {Rating = 3}
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Film>>();
            mockSet.As<IQueryable<Film>>().Setup(m => m.Provider).Returns(films.Provider);
            mockSet.As<IQueryable<Film>>().Setup(m => m.Expression).Returns(films.Expression);
            mockSet.As<IQueryable<Film>>().Setup(m => m.ElementType).Returns(films.ElementType);
            mockSet.As<IQueryable<Film>>().Setup(m => m.GetEnumerator()).Returns(films.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Films).Returns(mockSet.Object);

            var filmBusiness = new FilmBusiness(mockContext.Object);

            // Act
            await filmBusiness.DeleteAsync(films.First().Id);

            // Assert
            Assert.AreEqual(2,mockSet.Object.Count(), "Doesn't delete the elements from the database.");
        }
    }
}
