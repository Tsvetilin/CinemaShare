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
using System;
using Data.Enums;
using CinemaShare.Common.Mapping;
using CinemaShare.Models.ViewModels;

namespace Tests.Business.Tests
{
    class CinemaBusinessTests
    {
        [Test]
        public async Task GetAsyncReturnsSearchedElement()
        {
            // Arrange
            var cinemas = new List<Cinema>
            {
                new Cinema {Id="1"},
                new Cinema {}
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Cinema>>();
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.Provider).Returns(cinemas.Provider);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.Expression).Returns(cinemas.Expression);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.ElementType).Returns(cinemas.ElementType);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.GetEnumerator()).Returns(cinemas.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Cinemas).Returns(mockSet.Object);
            mockContext.Setup(c => c.Cinemas.FindAsync(It.IsAny<string>())).Returns(new ValueTask<Cinema>((cinemas.First())));

            var  cinemaBusiness = new CinemaBusiness(mockContext.Object, new EmailSender("TestAPIKey", "TestSender", "TestSenderName"));
            var searchedCinema = cinemas.First();

            // Act
            var resultCinema = await cinemaBusiness.GetAsync(searchedCinema.Id);

            // Assert
            Assert.AreEqual(searchedCinema, resultCinema, "Doesn't return all elements in the database.");
        }

        [Test]
        public void GetAllReturnsAllElements()
        {
            // Arrange
            var cinemas = new List<Cinema>
            {
                new Cinema {},
                new Cinema {}
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Cinema>>();
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.Provider).Returns(cinemas.Provider);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.Expression).Returns(cinemas.Expression);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.ElementType).Returns(cinemas.ElementType);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.GetEnumerator()).Returns(cinemas.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Cinemas).Returns(mockSet.Object);
            mockContext.Setup(c => c.Cinemas.FindAsync(It.IsAny<string>())).Returns(new ValueTask<Cinema>((cinemas.First())));

            var cinemaBusiness = new CinemaBusiness(mockContext.Object, new EmailSender("TestAPIKey", "TestSender", "TestSenderName"));

            // Act
            var resultCinemas = cinemaBusiness.GetAll().ToList();

            // Assert
            Assert.AreEqual(2, resultCinemas.Count(), "Doesn't return all elements in the database.");
        }

        [Test]
        public void CountAllCinemasReturnsTheCountOfTheCinemas()
        {
            // Arrange
            var cinemas = new List<Cinema>
            {
                new Cinema {},
                new Cinema {}
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Cinema>>();
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.Provider).Returns(cinemas.Provider);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.Expression).Returns(cinemas.Expression);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.ElementType).Returns(cinemas.ElementType);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.GetEnumerator()).Returns(cinemas.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Cinemas).Returns(mockSet.Object);
            mockContext.Setup(c => c.Cinemas.FindAsync(It.IsAny<string>())).Returns(new ValueTask<Cinema>((cinemas.First())));

            var cinemaBusiness = new CinemaBusiness(mockContext.Object, new EmailSender("TestAPIKey", "TestSender", "TestSenderName"));

            // Act
            var resultCinemasCount = cinemaBusiness.CountAllCinemas();

            // Assert
            Assert.AreEqual(2, resultCinemasCount, "Doesn't return all elements in the database.");
        }

        [Test]
        public async Task AddAsyncAddsSelectedElementToDb()
        {
            // Arrange
            var cinemas = new List<Cinema>
            {
                new Cinema {},
                new Cinema {}
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Cinema>>();
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.Provider).Returns(cinemas.Provider);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.Expression).Returns(cinemas.Expression);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.ElementType).Returns(cinemas.ElementType);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.GetEnumerator()).Returns(cinemas.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Cinemas).Returns(mockSet.Object);
            mockContext.Setup(c => c.Cinemas.FindAsync(It.IsAny<string>())).Returns(new ValueTask<Cinema>((cinemas.First())));
            mockContext.Setup(m => m.AddAsync(It.IsAny<Cinema>(), It.IsAny<CancellationToken>()))
              .Returns(new ValueTask<EntityEntry<Cinema>>(Task.FromResult((EntityEntry<Cinema>)null)));

            var cinemaBusiness = new CinemaBusiness(mockContext.Object, new EmailSender("TestAPIKey", "TestSender", "TestSenderName"));
            var newCinema = new Cinema();

            // Act
            await cinemaBusiness.AddAsync(newCinema);

            // Assert
            mockSet.Verify(m => m.AddAsync(It.IsAny<Cinema>(), It.IsAny<CancellationToken>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task GetAsyncReturnsSearchedElementMapped()
        {
            // Arrange
            var cinemas = new List<Cinema>
            {
                new Cinema {Id="1", Manager= new CinemaUser(), City="Sliven", Country="Bulgaria", FilmProjections=new List<FilmProjection>(), Name="Pencho" },
                new Cinema {}
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Cinema>>();
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.Provider).Returns(cinemas.Provider);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.Expression).Returns(cinemas.Expression);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.ElementType).Returns(cinemas.ElementType);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.GetEnumerator()).Returns(cinemas.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Cinemas).Returns(mockSet.Object);
            mockContext.Setup(c => c.Cinemas.FindAsync(It.IsAny<string>())).Returns(new ValueTask<Cinema>((cinemas.First())));

            var cinemaBusiness = new CinemaBusiness(mockContext.Object, new EmailSender("TestAPIKey", "TestSender", "TestSenderName"));
            var searchedCinema = cinemas.First();
            var mapper = new Mapper();
            
            // Act
            var resultCinema = await cinemaBusiness.GetAsync(searchedCinema.Id,mapper.MapToCinemaCardViewModel);

            // Assert
            Assert.AreEqual(searchedCinema.Id, resultCinema.Id, "Doesn't return all elements in the database.");
            Assert.AreEqual(searchedCinema.Name, resultCinema.Name, "Doesn't return all elements in the database.");
            Assert.AreEqual(searchedCinema.Country, resultCinema.Country, "Doesn't return all elements in the database.");
            Assert.AreEqual(searchedCinema.City, resultCinema.City, "Doesn't return all elements in the database.");
        }
    }
}
