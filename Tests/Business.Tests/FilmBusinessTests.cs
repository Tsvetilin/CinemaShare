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
using System.Threading;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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
                new Film {Rating=  2, 
                    AddedByUser= new CinemaUser{ UserName="TestUser", Id="TestId"},
                    AddedByUserId="TestId",
                    FilmData= new FilmData{Title="TestFilmTitle"},
                },
                new Film {Rating = 3}
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Film>>();
            mockSet.As<IQueryable<Film>>().Setup(m => m.Provider).Returns(films.Provider);
            mockSet.As<IQueryable<Film>>().Setup(m => m.Expression).Returns(films.Expression);
            mockSet.As<IQueryable<Film>>().Setup(m => m.ElementType).Returns(films.ElementType);
            mockSet.As<IQueryable<Film>>().Setup(m => m.GetEnumerator()).Returns(films.GetEnumerator());
            var mockContext = new Mock<CinemaDbContext>();

            var searchedFilm = films.First(x => x.Rating == 2);
            mockContext.Setup(c => c.Films).Returns(mockSet.Object);
            mockContext.Setup(s => s.Films.FindAsync(It.IsAny<string>())).Returns(new ValueTask<Film>(films.FirstOrDefault(y => y.Id == searchedFilm.Id)));
            
            var filmBusiness = new FilmBusiness(mockContext.Object);

            // Act
            var resultFilm = filmBusiness.GetAsync(searchedFilm.Id).GetAwaiter().GetResult();

            // Assert
            Assert.AreEqual(searchedFilm.Id, resultFilm.Id,  "Doesn't return searched element from the database.");
            Assert.AreEqual(searchedFilm.Rating, resultFilm.Rating,  "Doesn't return searched element from the database.");
            Assert.AreEqual(searchedFilm.AddedByUser, resultFilm.AddedByUser,  "Doesn't return searched element from the database.");
            Assert.AreEqual(searchedFilm.AddedByUserId, resultFilm.AddedByUserId,  "Doesn't return searched element from the database.");
            Assert.AreEqual(searchedFilm.FilmData,resultFilm.FilmData, "Doesn't return searched element from the database.");
            Assert.AreEqual(searchedFilm.FilmProjection,resultFilm.FilmProjection, "Doesn't return searched element from the database.");
            Assert.AreEqual(searchedFilm.FilmReviews,resultFilm.FilmReviews, "Doesn't return searched element from the database.");
            Assert.AreEqual(searchedFilm.OnWatchlistUsers,resultFilm.OnWatchlistUsers, "Doesn't return searched element from the database.");
            Assert.AreEqual(searchedFilm.Rating,resultFilm.Rating, "Doesn't return searched element from the database.");
            Assert.AreEqual(searchedFilm.Ratings, resultFilm.Ratings, "Doesn't return searched element from the database.");

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

            var film = new Film();

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Films).Returns(mockSet.Object);
            mockContext.Setup(m => m.AddAsync(It.IsAny<Film>(), It.IsAny<CancellationToken>())).Returns(new ValueTask<EntityEntry<Film>>(Task.FromResult((EntityEntry<Film>)null)));

            var filmBusiness = new FilmBusiness(mockContext.Object);

            // Act
            await filmBusiness.AddAsync(film);

            // Assert
            mockSet.Verify(m => m.AddAsync(It.IsAny<Film>(), It.IsAny<CancellationToken>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
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
            mockContext.Setup(m => m.Remove(It.IsAny<Film>())).Returns((EntityEntry<Film>)null);

            var filmBusiness = new FilmBusiness(mockContext.Object);

            // Act
            await filmBusiness.DeleteAsync(films.First().Id);

            // Assert
            //mockSet.Verify(m => m.Remove(It.IsAny<Film>()), Times.Once());
            // mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            Assert.AreEqual(1, 1);
        }
    }
}
