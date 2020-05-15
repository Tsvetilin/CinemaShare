using Business;
using CinemaShare.Common.Mapping;
using CinemaShare.Models.ViewModels;
using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Business.Tests
{
    class FilmDataBusinessTests
    {
        [Test]
        public async Task FilmBusinessGetAsyncReturnsSearchedElement()
        {
            // Arrange
            var mapper = new Mapper();
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

            var filmDataBusiness = new FilmDataBusiness(mockContext.Object);

            // Act
            var searchedFilm = films.First();
            var resultFilm = await filmDataBusiness.GetAsync(searchedFilm.Id);

            // Assert TODOOOOOOO
            Assert.AreEqual(resultFilm.Director, films.First().Rating, "Doesn't return all elements in the database.");
        }

        [Test]
        public async Task FilmBusinessGetAsyncReturnsSearchedElementMapped()
        {
            // Arrange
            var mapper = new Mapper();
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

            var filmDataBusiness = new FilmDataBusiness(mockContext.Object);

            // Act
            var searchedFilm = films.First();
            var resultFilm = await filmDataBusiness.GetAsync(searchedFilm.Id, mapper.MapToFilmDataViewModel);

            // Assert TODOOOOOO
            Assert.AreEqual(resultFilm.Director, searchedFilm.Rating, "Doesn't return all elements in the database.");
        }
    }
}
