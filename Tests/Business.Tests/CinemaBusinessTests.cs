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
            mockContext.Setup(c => c.Cinemas.FindAsync(It.IsAny<string>())).
                          Returns(new ValueTask<Cinema>(cinemas.First()));

            var  cinemaBusiness = new CinemaBusiness(mockContext.Object,
                                                     new EmailSender("TestAPIKey", "TestSender", "TestSenderName"));
            var searchedCinema = cinemas.First();

            // Act
            var resultCinema = await cinemaBusiness.GetAsync(searchedCinema.Id);

            // Assert
            Assert.AreEqual(searchedCinema, resultCinema, "Doesn't return the searched element from the cinema.");
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
            mockContext.Setup(c => c.Cinemas.FindAsync(It.IsAny<string>())).
                          Returns(new ValueTask<Cinema>(cinemas.First()));

            var cinemaBusiness = new CinemaBusiness(mockContext.Object, 
                                                    new EmailSender("TestAPIKey", "TestSender", "TestSenderName"));

            // Act
            var resultCinemas = cinemaBusiness.GetAll().ToList();

            // Assert
            Assert.AreEqual(2, resultCinemas.Count(), "Doesn't return all elements in the cinema.");
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
            mockContext.Setup(c => c.Cinemas.FindAsync(It.IsAny<string>())).
                          Returns(new ValueTask<Cinema>(cinemas.First()));

            var cinemaBusiness = new CinemaBusiness(mockContext.Object, 
                                                    new EmailSender("TestAPIKey", "TestSender", "TestSenderName"));

            // Act
            var resultCinemasCount = cinemaBusiness.CountAllCinemas();

            // Assert
            Assert.AreEqual(2, resultCinemasCount, "Doesn't return correct count of cinemas in the database.");
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
            mockContext.Setup(c => c.Cinemas.FindAsync(It.IsAny<string>()))
                       .Returns(new ValueTask<Cinema>((cinemas.First())));
            mockContext.Setup(m => m.AddAsync(It.IsAny<Cinema>(), It.IsAny<CancellationToken>()))
              .Returns(new ValueTask<EntityEntry<Cinema>>(Task.FromResult((EntityEntry<Cinema>)null)));

            var cinemaBusiness = new CinemaBusiness(mockContext.Object,
                                                    new EmailSender("TestAPIKey", "TestSender", "TestSenderName"));
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
                new Cinema 
                {
                    Id="1", Manager= new CinemaUser(),
                    City="Sliven", Country="Bulgaria", 
                    FilmProjections=new List<FilmProjection>(),
                    Name="Cinema1"
                },
                new Cinema {}
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Cinema>>();
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.Provider).Returns(cinemas.Provider);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.Expression).Returns(cinemas.Expression);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.ElementType).Returns(cinemas.ElementType);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.GetEnumerator()).Returns(cinemas.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Cinemas).Returns(mockSet.Object);
            mockContext.Setup(c => c.Cinemas.FindAsync(It.IsAny<string>())).
                        Returns(new ValueTask<Cinema>((cinemas.First())));

            var cinemaBusiness = new CinemaBusiness(mockContext.Object, 
                                                    new EmailSender("TestAPIKey", "TestSender", "TestSenderName"));
            var searchedCinema = cinemas.First();
            var mapper = new Mapper();
            
            // Act
            var resultCinema = await cinemaBusiness.GetAsync(searchedCinema.Id,mapper.MapToCinemaCardViewModel);

            // Assert
            Assert.AreEqual(searchedCinema.Id, resultCinema.Id, "Doesn't return the correct id of the cinema.");
            Assert.AreEqual(searchedCinema.Name, resultCinema.Name, "Doesn't return the correct name of the cinema.");
            Assert.AreEqual(searchedCinema.Country, resultCinema.Country, "Doesn't return the correct country of the cinema.");
            Assert.AreEqual(searchedCinema.City, resultCinema.City, "Doesn't return the correct iof the cinema.");
        }

        [Test]
        public void GetPageItemsReturnsAllElementsOnThePage()
        {
            // Arrange
            var cinemas = new List<Cinema>
            {
                new Cinema 
                {
                    Id="1", Manager= new CinemaUser(),
                    City="Sliven", Country="Bulgaria",
                    FilmProjections=new List<FilmProjection>(), 
                    Name="Cinema1"
                },
                new Cinema 
                {
                    Id="2", Manager= new CinemaUser(),
                    City="Sliven", Country="Bulgaria", 
                    FilmProjections=new List<FilmProjection>(),
                    Name="Cinema2"
                }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Cinema>>();
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.Provider).Returns(cinemas.Provider);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.Expression).Returns(cinemas.Expression);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.ElementType).Returns(cinemas.ElementType);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.GetEnumerator()).Returns(cinemas.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Cinemas).Returns(mockSet.Object);
            mockContext.Setup(c => c.Cinemas.FindAsync(It.IsAny<string>())).
                       Returns(new ValueTask<Cinema>(cinemas.First()));

            var cinemaBusiness = new CinemaBusiness(mockContext.Object,
                                                    new EmailSender("TestAPIKey", "TestSender", "TestSenderName"));
            var mapper = new Mapper();

            // Act
            var resultCinemas = cinemaBusiness.GetPageItems(1,2, mapper.MapToCinemaCardViewModel).ToList();

            // Assert
            Assert.AreEqual(2, resultCinemas.Count, "Doesn't return all elements from the page.");
        }

        [Test]
        public void IsAlreadyAddedReturnsIfCinemaIsAlreadyAdded()
        {
            // Arrange
            var cinemas = new List<Cinema>
            {
                new Cinema 
                {
                    Id="1", Name="Cinema1"
                },
                new Cinema {}
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Cinema>>();
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.Provider).Returns(cinemas.Provider);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.Expression).Returns(cinemas.Expression);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.ElementType).Returns(cinemas.ElementType);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.GetEnumerator()).Returns(cinemas.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Cinemas).Returns(mockSet.Object);
            mockContext.Setup(c => c.Cinemas.FindAsync(It.IsAny<string>())).
                        Returns(new ValueTask<Cinema>((cinemas.First())));

            var cinemaBusiness = new CinemaBusiness(mockContext.Object,
                                                    new EmailSender("TestAPIKey", "TestSender", "TestSenderName"));
            // Act
            var result = cinemaBusiness.IsAlreadyAdded(cinemas.First().Name);
            bool expectedResult = true;
            // Assert
            Assert.AreEqual(expectedResult, result, "Doesn't return correct if the cinema is already added in the db.");
        }

        [Test]
        public void GetAllByNameReturnsAllTheCinemasWithTheSearchedName()
        {
            // Arrange
            var cinemas = new List<Cinema>
            {
                new Cinema 
                {
                    Id="1", Manager= new CinemaUser(),
                    City="Sliven", Country="Bulgaria",
                    FilmProjections=new List<FilmProjection>(),
                    Name="Cinema1"
                },
                new Cinema
                {
                    Id="2", Manager= new CinemaUser(),
                    City="Sliven", Country="Bulgaria", 
                    FilmProjections=new List<FilmProjection>(),
                    Name="Cinema2"
                }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Cinema>>();
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.Provider).Returns(cinemas.Provider);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.Expression).Returns(cinemas.Expression);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.ElementType).Returns(cinemas.ElementType);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.GetEnumerator()).Returns(cinemas.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Cinemas).Returns(mockSet.Object);
            mockContext.Setup(c => c.Cinemas.FindAsync(It.IsAny<string>())).
                        Returns(new ValueTask<Cinema>((cinemas.First())));

            var cinemaBusiness = new CinemaBusiness(mockContext.Object, 
                                                    new EmailSender("TestAPIKey", "TestSender", "TestSenderName"));
            var mapper = new Mapper();
            // Act
            var resultCinemas = cinemaBusiness.GetAllByName(cinemas.First().Name,mapper.MapToCinemaCardViewModel).ToList();
            // Assert
            Assert.AreEqual(1, resultCinemas.Count, "Doesn't return the cinema with the searched name.");
        }

        [Test]
        public void GetAllByCityReturnsAllTheCinemasWithTheSearchedCity()
        {
            // Arrange
            var cinemas = new List<Cinema>
            {
                new Cinema 
                {
                    Id="1", Manager= new CinemaUser(),
                    City="Sliven", Country="Bulgaria", 
                    FilmProjections=new List<FilmProjection>(),
                    Name="Cinema1"
                },
                new Cinema 
                {
                    Id="2", Manager= new CinemaUser(),
                    City="Sliven", Country="Bulgaria", 
                    FilmProjections=new List<FilmProjection>(),
                    Name="Cinema2"
                }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Cinema>>();
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.Provider).Returns(cinemas.Provider);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.Expression).Returns(cinemas.Expression);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.ElementType).Returns(cinemas.ElementType);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.GetEnumerator()).Returns(cinemas.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Cinemas).Returns(mockSet.Object);
            mockContext.Setup(c => c.Cinemas.FindAsync(It.IsAny<string>())).
                                             Returns(new ValueTask<Cinema>((cinemas.First())));

            var cinemaBusiness = new CinemaBusiness(mockContext.Object, 
                                                    new EmailSender("TestAPIKey", "TestSender", "TestSenderName"));
            var mapper = new Mapper();
            // Act
            var resultCinemas = cinemaBusiness.GetAllByCity(cinemas.First().City, mapper.MapToCinemaCardViewModel).ToList();
            // Assert
            Assert.AreEqual(2, resultCinemas.Count, "Doesn't return the cinema with the searched city.");
        }

        [Test]
        public void GetSearchResultReturnsNullIfNotFound()
        {
            // Arrange
            var cinemas = new List<Cinema>
            {
                new Cinema 
                {
                    Id="1", Manager= new CinemaUser(), 
                    City="Sliven", Country="Bulgaria",
                    FilmProjections=new List<FilmProjection>(),
                    Name="Cinema1"
                },
                new Cinema 
                {
                    Id="2", Manager= new CinemaUser(),
                    City="Sliven", Country="Bulgaria", 
                    FilmProjections=new List<FilmProjection>(),
                    Name="Cinema2"
                }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Cinema>>();
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.Provider).Returns(cinemas.Provider);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.Expression).Returns(cinemas.Expression);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.ElementType).Returns(cinemas.ElementType);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.GetEnumerator()).Returns(cinemas.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Cinemas).Returns(mockSet.Object);
            mockContext.Setup(c => c.Cinemas.FindAsync(It.IsAny<string>())).
                                             Returns(new ValueTask<Cinema>(cinemas.First()));

            var cinemaBusiness = new CinemaBusiness(mockContext.Object,
                                                    new EmailSender("TestAPIKey", "TestSender", "TestSenderName"));
            var mapper = new Mapper();

            // Act
            var resultCinemas = cinemaBusiness.GetSearchResults("UnexistingCinema", mapper.MapToCinemaCardViewModel).ToList();

            // Assert
            Assert.AreEqual(0, resultCinemas.Count, "Doesn't return correct if the element is not found in the database.");
        }

        [Test]
        public async Task DeleteAsyncDeletesElement()
        {
            // Arrange
            var projections = new List<FilmProjection>
            {
                new FilmProjection{Id="1"},
                new FilmProjection{Id="2"}
            }.AsQueryable();

            var cinemas = new List<Cinema>
            {
                new Cinema {Id="1", FilmProjections= new List<FilmProjection>{ projections.First() } },
                new Cinema { }
            }.AsQueryable();

            var mockSetData = new Mock<DbSet<FilmProjection>>();
            mockSetData.As<IQueryable<FilmProjection>>().Setup(m => m.Provider).Returns(projections.Provider);
            mockSetData.As<IQueryable<FilmProjection>>().Setup(m => m.Expression).Returns(projections.Expression);
            mockSetData.As<IQueryable<FilmProjection>>().Setup(m => m.ElementType).Returns(projections.ElementType);
            mockSetData.As<IQueryable<FilmProjection>>().Setup(m => m.GetEnumerator()).Returns(projections.GetEnumerator());

            var mockSet = new Mock<DbSet<Cinema>>();
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.Provider).Returns(cinemas.Provider);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.Expression).Returns(cinemas.Expression);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.ElementType).Returns(cinemas.ElementType);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.GetEnumerator()).Returns(cinemas.GetEnumerator());

            var searchedCinema = cinemas.First();

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Cinemas).Returns(mockSet.Object);
            mockContext.Setup(c => c.FilmProjections).Returns(mockSetData.Object);
            mockContext.Setup(s => s.Cinemas.FindAsync(It.IsAny<string>())).Returns(new ValueTask<Cinema>(searchedCinema));
            mockContext.Setup(m => m.Remove(It.IsAny<FilmProjection>())).Returns((EntityEntry<FilmProjection>)null);
            mockContext.Setup(m => m.Remove(It.IsAny<Cinema>())).Returns((EntityEntry<Cinema>)null);


            var cinemaBusiness = new CinemaBusiness(mockContext.Object, 
                                                    new EmailSender("TestAPIKey", "TestSender", "TestSenderName"));

            // Act
            await cinemaBusiness.DeleteAsync(searchedCinema.Id, "ProjectionPattern");

            // Assert
            mockSet.Verify(m => m.Remove(It.IsAny<Cinema>()), Times.Once());
            mockSetData.Verify(m => m.Remove(It.IsAny<FilmProjection>()), Times.Never());
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task DeleteAsyncDoesntDeleteUnexistingElement()
        {
            // Arrange
            var cinemas = new List<Cinema>
            {
                new Cinema { },
                new Cinema { }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Cinema>>();
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.Provider).Returns(cinemas.Provider);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.Expression).Returns(cinemas.Expression);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.ElementType).Returns(cinemas.ElementType);
            mockSet.As<IQueryable<Cinema>>().Setup(m => m.GetEnumerator()).Returns(cinemas.GetEnumerator());

            var searchedCinema = cinemas.First();

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Cinemas).Returns(mockSet.Object);
            mockContext.Setup(s => s.Cinemas.FindAsync(It.IsAny<string>())).Returns(new ValueTask<Cinema>((Cinema)null));
            mockContext.Setup(m => m.Remove(It.IsAny<Cinema>())).Returns((EntityEntry<Cinema>)null);

            var cinemaBusiness = new CinemaBusiness(mockContext.Object,
                                                    new EmailSender("TestAPIKey", "TestSender", "TestSenderName"));

            // Act
            await cinemaBusiness.DeleteAsync(searchedCinema.Id,"ProjectionPattern");

            // Assert
            mockSet.Verify(m => m.Remove(It.IsAny<Cinema>()), Times.Never());
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }
    }
}
