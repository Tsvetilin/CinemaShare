using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Business;
using Data;
using Data.Models;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
