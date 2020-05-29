using Business;
using CinemaShare.Common.Mapping;
using CinemaShare.Models.InputModels;
using CinemaShare.Models.ViewModels;
using Data;
using Data.Enums;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tests.Business.Tests
{
    class FilmDataBusinessTests
    {
        [Test]
        public async Task GetAsyncReturnsSearchedElement()
        {
            // Arrange
            var mapper = new Mapper();
            var films = new List<FilmData>
            {
                new FilmData {},
                new FilmData {}
            }.AsQueryable();

            var mockSet = new Mock<DbSet<FilmData>>();
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.Provider).Returns(films.Provider);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.Expression).Returns(films.Expression);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.ElementType).Returns(films.ElementType);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.GetEnumerator()).Returns(films.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.FilmDatas).Returns(mockSet.Object);
            mockContext.Setup(c => c.FilmDatas.FindAsync(It.IsAny<string>())).Returns(new ValueTask<FilmData>((films.First())));

            var filmDataBusiness = new FilmDataBusiness(mockContext.Object);
            var searchedFilm = films.First();

            // Act
            var resultFilm = await filmDataBusiness.GetAsync(searchedFilm.FilmId);

            // Assert
            Assert.AreEqual(searchedFilm, resultFilm, "Doesn't return all elements in the database.");
        }

        [Test]
        public async Task GetAsyncReturnsSearchedElementMapped()
        {
            // Arrange
            var mapper = new Mapper();
            var films = new List<FilmData>
            {
                new FilmData {FilmId="Film1",
                    Film=new Film{FilmProjection = new List<FilmProjection>
                    { new FilmProjection {Film=new Film{FilmData = new FilmData()},
                                        Cinema=new Cinema{Manager=new CinemaUser(),
                                        FilmProjections= new List<FilmProjection>{new FilmProjection()} } } } } ,
                    Genre = new List<GenreType>{new GenreType{Genre= Genre.Action } },
            },
                new FilmData {Film=new Film{Rating=3 } },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<FilmData>>();
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.Provider).Returns(films.Provider);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.Expression).Returns(films.Expression);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.ElementType).Returns(films.ElementType);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.GetEnumerator()).Returns(films.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.FilmDatas).Returns(mockSet.Object);
            mockContext.Setup(c => c.FilmDatas.FindAsync(It.IsAny<string>())).Returns(new ValueTask<FilmData>((films.First())));

            var filmDataBusiness = new FilmDataBusiness(mockContext.Object);
            var searchedFilm = films.First();

            // Act
            var resultFilm = await filmDataBusiness.GetAsync(searchedFilm.FilmId, mapper.MapToFilmDataViewModel);

            // Assert
            Assert.AreEqual(searchedFilm.FilmId, resultFilm.Id, "Doesn't return all elements in the database.");
        }

        [Test]
        public void GetAllReturnsAllElements()
        {
            // Arrange
            var films = new List<FilmData>
            {
                new FilmData {},
                new FilmData {}
            }.AsQueryable();

            var mockSet = new Mock<DbSet<FilmData>>();
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.Provider).Returns(films.Provider);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.Expression).Returns(films.Expression);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.ElementType).Returns(films.ElementType);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.GetEnumerator()).Returns(films.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.FilmDatas).Returns(mockSet.Object);

            var filmDataBusiness = new FilmDataBusiness(mockContext.Object);

            // Act
            var allFilms = filmDataBusiness.GetAll().ToList();

            // Assert
            Assert.AreEqual(2, allFilms.Count, "Doesn't return all elements in the database.");
        }

        [Test]
        public void GetAllByNameReturnsAllElementsWithSearchedName()
        {
            // Arrange
            var films = new List<FilmData>
            {
                new FilmData {Title="XXX", FilmId="Film1",
                    Film=new Film{FilmProjection = new List<FilmProjection>
                    { new FilmProjection {Film=new Film{FilmData = new FilmData{Title="XXX" } },
                                        Cinema=new Cinema{Manager=new CinemaUser(),
                                        FilmProjections= new List<FilmProjection>{new FilmProjection()} } } } } ,
                    Genre = new List<GenreType>{new GenreType{Genre= Genre.Action } },
            },
                new FilmData {Title="NotThisOne", Film=new Film{Rating=3 } },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<FilmData>>();
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.Provider).Returns(films.Provider);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.Expression).Returns(films.Expression);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.ElementType).Returns(films.ElementType);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.GetEnumerator()).Returns(films.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.FilmDatas).Returns(mockSet.Object);
            mockContext.Setup(c => c.FilmDatas.FindAsync(It.IsAny<string>())).Returns(new ValueTask<FilmData>((films.First())));

            var filmDataBusiness = new FilmDataBusiness(mockContext.Object);
            var mapper = new Mapper();
            var searchedTitle = "XXX";
            // Act
            var allFilms = filmDataBusiness.GetAllByName(searchedTitle, mapper.MapToFilmDataViewModel).ToList();

            // Assert
            Assert.AreEqual(1, allFilms.Count, "Doesn't return all searched elements in the database.");
        }

        [Test]
        public void GetByNameReturnsAllElementsWithSearchedName()
        {
            // Arrange
            var films = new List<FilmData>
            {
                new FilmData {Title="XXX", FilmId="Film1",
                    Film=new Film{FilmProjection = new List<FilmProjection>
                    { new FilmProjection {Film=new Film{FilmData = new FilmData{Title="XXX" } },
                                        Cinema=new Cinema{Manager=new CinemaUser(),
                                        FilmProjections= new List<FilmProjection>{new FilmProjection()} } } } } ,
                    Genre = new List<GenreType>{new GenreType{Genre= Genre.Action } },
            },
                new FilmData {Title="NotThisOne", Film=new Film{Rating=3 } },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<FilmData>>();
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.Provider).Returns(films.Provider);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.Expression).Returns(films.Expression);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.ElementType).Returns(films.ElementType);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.GetEnumerator()).Returns(films.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.FilmDatas).Returns(mockSet.Object);
            mockContext.Setup(c => c.FilmDatas.FindAsync(It.IsAny<string>())).Returns(new ValueTask<FilmData>((films.First())));

            var filmDataBusiness = new FilmDataBusiness(mockContext.Object);
            var searchedTitle = "XXX";
            // Act
            var resultFilm = filmDataBusiness.GetByName(searchedTitle);

            // Assert
            Assert.AreEqual(films.First().Title, resultFilm.Title, "Doesn't return all searched elements in the database.");
        }

        [Test]
        public async Task AddAsyncAddsElementToDb()
        {
            // Arrange
            var films = new List<FilmData>
            {
                new FilmData {},
                new FilmData {}
            }.AsQueryable();

            var mockSet = new Mock<DbSet<FilmData>>();
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.Provider).Returns(films.Provider);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.Expression).Returns(films.Expression);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.ElementType).Returns(films.ElementType);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.GetEnumerator()).Returns(films.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.FilmDatas).Returns(mockSet.Object);
            mockContext.Setup(m => m.AddAsync(It.IsAny<FilmData>(), It.IsAny<CancellationToken>()))
                .Returns(new ValueTask<EntityEntry<FilmData>>(Task.FromResult((EntityEntry<FilmData>)null)));

            var filmDataBusiness = new FilmDataBusiness(mockContext.Object);
            var film = new Film { Id = "NewFilmId" };
            var mapper = new Mapper();
            // Act
            await filmDataBusiness.AddAsync(new FilmInputModel
            {
                Title = "NewFilmTitle",
                Genre = new List<Genre> { Genre.Action },
            },
                film, mapper.MapToFilmData);

            // Assert
            mockSet.Verify(m => m.AddAsync(It.IsAny<FilmData>(), It.IsAny<CancellationToken>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public void IsAlreadyAddedReturnsIfElementIsAlreadyAdded()
        {
            // Arrange
            var films = new List<FilmData>
            {
                new FilmData {Title="SearchedFilmTitle"},
                new FilmData {}
            }.AsQueryable();

            var mockSet = new Mock<DbSet<FilmData>>();
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.Provider).Returns(films.Provider);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.Expression).Returns(films.Expression);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.ElementType).Returns(films.ElementType);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.GetEnumerator()).Returns(films.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.FilmDatas).Returns(mockSet.Object);
            mockContext.Setup(c => c.FilmDatas.FindAsync(It.IsAny<string>())).Returns(new ValueTask<FilmData>((films.First())));

            var filmDataBusiness = new FilmDataBusiness(mockContext.Object);
            // Act
            var isAdded = filmDataBusiness.IsAlreadyAdded("SearchedFilmTitle");

            // Assert
            Assert.AreEqual(true, isAdded, "Doesn't return if the searched element is already added.");
        }

        [Test]
        public async Task DeleteAsyncDeletesElement()
        {
            // Arrange
            var films = new List<Film>
            {
                new Film { },
                new Film { }
            }.AsQueryable();

            var filmsData = new List<FilmData>
            {
                new FilmData {FilmId="SearchedFilmId", Title="XXX", Film=films.First()  },
                new FilmData { }
            }.AsQueryable();

            var mockSetData = new Mock<DbSet<FilmData>>();
            mockSetData.As<IQueryable<FilmData>>().Setup(m => m.Provider).Returns(filmsData.Provider);
            mockSetData.As<IQueryable<FilmData>>().Setup(m => m.Expression).Returns(filmsData.Expression);
            mockSetData.As<IQueryable<FilmData>>().Setup(m => m.ElementType).Returns(filmsData.ElementType);
            mockSetData.As<IQueryable<FilmData>>().Setup(m => m.GetEnumerator()).Returns(filmsData.GetEnumerator());

            var mockSet = new Mock<DbSet<Film>>();
            mockSet.As<IQueryable<Film>>().Setup(m => m.Provider).Returns(films.Provider);
            mockSet.As<IQueryable<Film>>().Setup(m => m.Expression).Returns(films.Expression);
            mockSet.As<IQueryable<Film>>().Setup(m => m.ElementType).Returns(films.ElementType);
            mockSet.As<IQueryable<Film>>().Setup(m => m.GetEnumerator()).Returns(films.GetEnumerator());

            var searchedFilm = filmsData.First();
            
            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Films).Returns(mockSet.Object);
            mockContext.Setup(c => c.FilmDatas).Returns(mockSetData.Object);
            mockContext.Setup(s => s.FilmDatas.FindAsync(It.IsAny<string>())).Returns(new ValueTask<FilmData>(searchedFilm));
            mockContext.Setup(m => m.Films.Remove(It.IsAny<Film>())).Returns((EntityEntry<Film>)null);
            mockContext.Setup(m => m.FilmDatas.Remove(It.IsAny<FilmData>())).Returns((EntityEntry<FilmData>)null);

            var filmDataBusiness = new FilmDataBusiness(mockContext.Object);

            // Act
            await filmDataBusiness.DeleteAsync(searchedFilm.FilmId);

            // Assert
            mockSetData.Verify(m => m.Remove(It.IsAny<FilmData>()), Times.Once());
            mockSet.Verify(m => m.Remove(It.IsAny<Film>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task DeleteAsyncDoesntDeleteUnexistingElement()
        {
            // Arrange
            var films = new List<FilmData>
            {
                new FilmData { },
                new FilmData { }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<FilmData>>();
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.Provider).Returns(films.Provider);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.Expression).Returns(films.Expression);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.ElementType).Returns(films.ElementType);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.GetEnumerator()).Returns(films.GetEnumerator());

            var searchedFilm = films.First();

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.FilmDatas).Returns(mockSet.Object);
            mockContext.Setup(s => s.FilmDatas.FindAsync(It.IsAny<string>())).Returns(new ValueTask<FilmData>((FilmData)null));
            mockContext.Setup(m => m.Remove(It.IsAny<Film>())).Returns((EntityEntry<Film>)null);

            var filmDataBusiness = new FilmDataBusiness(mockContext.Object);

            // Act
            await filmDataBusiness.DeleteAsync(searchedFilm.FilmId);

            // Assert
            mockSet.Verify(m => m.Remove(It.IsAny<FilmData>()), Times.Never());
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Test]
        public void GetPageItemsByNameReturnsAllElementsOnThePage()
        {
            // Arrange
            var films = new List<FilmData>
            {
                new FilmData {Title="XXX", FilmId="Film1",
                    Film=new Film{FilmProjection = new List<FilmProjection>
                    { new FilmProjection {Film=new Film{FilmData = new FilmData{Title="XXX" } },
                                        Cinema=new Cinema{Manager=new CinemaUser(),
                                        FilmProjections= new List<FilmProjection>{new FilmProjection()} } } } } ,
                    Genre = new List<GenreType>{new GenreType{Genre= Genre.Action } },
            },
                new FilmData {Title="NotThisOne", Film=new Film{Rating=3 } },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<FilmData>>();
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.Provider).Returns(films.Provider);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.Expression).Returns(films.Expression);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.ElementType).Returns(films.ElementType);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.GetEnumerator()).Returns(films.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.FilmDatas).Returns(mockSet.Object);
            mockContext.Setup(c => c.FilmDatas.FindAsync(It.IsAny<string>())).Returns(new ValueTask<FilmData>((films.First())));

            var filmDataBusiness = new FilmDataBusiness(mockContext.Object);
            var mapper = new Mapper();
            // Act
            var resultFilms = filmDataBusiness.GetPageItems(1, 2, "Name", mapper.MapToExtendedFilmCardViewModel).ToList();

            // Assert
            Assert.AreEqual(2, resultFilms.Count, "Doesn't return all searched elements in the database.");
        }

        [Test]
        public void GetPageItemsByYearReturnsAllElementsOnThePage()
        {
            // Arrange
            var films = new List<FilmData>
            {
                new FilmData {Title="XXX", FilmId="Film1",
                    Film=new Film{FilmProjection = new List<FilmProjection>
                    { new FilmProjection {Film=new Film{FilmData = new FilmData{Title="XXX" } },
                                        Cinema=new Cinema{Manager=new CinemaUser(),
                                        FilmProjections= new List<FilmProjection>{new FilmProjection()} } } } } ,
                    Genre = new List<GenreType>{new GenreType{Genre= Genre.Action } },
            },
                new FilmData {Title="NotThisOne", Film=new Film{Rating=3 } },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<FilmData>>();
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.Provider).Returns(films.Provider);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.Expression).Returns(films.Expression);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.ElementType).Returns(films.ElementType);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.GetEnumerator()).Returns(films.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.FilmDatas).Returns(mockSet.Object);
            mockContext.Setup(c => c.FilmDatas.FindAsync(It.IsAny<string>())).Returns(new ValueTask<FilmData>((films.First())));

            var filmDataBusiness = new FilmDataBusiness(mockContext.Object);
            var mapper = new Mapper();
            // Act
            var resultFilms = filmDataBusiness.GetPageItems(1, 2, "Year", mapper.MapToExtendedFilmCardViewModel).ToList();

            // Assert
            Assert.AreEqual(2, resultFilms.Count, "Doesn't return all searched elements in the database.");
        }

        [Test]
        public void GetPageItemsByRatingReturnsAllElementsOnThePage()
        {
            // Arrange
            var films = new List<FilmData>
            {
                new FilmData {Title="XXX", FilmId="Film1",
                    Film=new Film{FilmProjection = new List<FilmProjection>
                    { new FilmProjection {Film=new Film{FilmData = new FilmData{Title="XXX" } },
                                        Cinema=new Cinema{Manager=new CinemaUser(),
                                        FilmProjections= new List<FilmProjection>{new FilmProjection()} } } } } ,
                    Genre = new List<GenreType>{new GenreType{Genre= Genre.Action } },
            },
                new FilmData {Title="NotThisOne", Film=new Film{Rating=3 } },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<FilmData>>();
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.Provider).Returns(films.Provider);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.Expression).Returns(films.Expression);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.ElementType).Returns(films.ElementType);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.GetEnumerator()).Returns(films.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.FilmDatas).Returns(mockSet.Object);
            mockContext.Setup(c => c.FilmDatas.FindAsync(It.IsAny<string>())).Returns(new ValueTask<FilmData>((films.First())));

            var filmDataBusiness = new FilmDataBusiness(mockContext.Object);
            var mapper = new Mapper();
            // Act
            var resultFilms = filmDataBusiness.GetPageItems(1, 2, "Rating", mapper.MapToExtendedFilmCardViewModel).ToList();

            // Assert
            Assert.AreEqual(2, resultFilms.Count, "Doesn't return all searched elements in the database.");
        }

        [Test]
        public void GetPageItemsReturnsAllElementsOnThePage()
        {
            // Arrange
            var films = new List<FilmData>
            {
                new FilmData {Title="XXX", FilmId="Film1",
                    Film=new Film{FilmProjection = new List<FilmProjection>
                    { new FilmProjection {Film=new Film{FilmData = new FilmData{Title="XXX" } },
                                        Cinema=new Cinema{Manager=new CinemaUser(),
                                        FilmProjections= new List<FilmProjection>{new FilmProjection()} } } } } ,
                    Genre = new List<GenreType>{new GenreType{Genre= Genre.Action } },
            },
                new FilmData {Title="NotThisOne", Film=new Film{Rating=3 } },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<FilmData>>();
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.Provider).Returns(films.Provider);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.Expression).Returns(films.Expression);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.ElementType).Returns(films.ElementType);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.GetEnumerator()).Returns(films.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.FilmDatas).Returns(mockSet.Object);
            mockContext.Setup(c => c.FilmDatas.FindAsync(It.IsAny<string>())).Returns(new ValueTask<FilmData>((films.First())));

            var filmDataBusiness = new FilmDataBusiness(mockContext.Object);
            var mapper = new Mapper();
            // Act
            var resultFilms = filmDataBusiness.GetPageItems(1, 2, "", mapper.MapToExtendedFilmCardViewModel).ToList();

            // Assert
            Assert.AreEqual(2, resultFilms.Count, "Doesn't return all searched elements in the database.");
        }

        [Test]
        public void GetTopFilmsReturnsAllElementsWithHighestRating()
        {
            // Arrange
            var films = new List<FilmData>
            {
                new FilmData {Title="XXX", FilmId="Film1",
                    Film=new Film{Rating = 5 },
            },
                new FilmData {Title="YYY",  FilmId="Film1", Film=new Film{Rating=3 } },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<FilmData>>();
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.Provider).Returns(films.Provider);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.Expression).Returns(films.Expression);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.ElementType).Returns(films.ElementType);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.GetEnumerator()).Returns(films.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.FilmDatas).Returns(mockSet.Object);
            mockContext.Setup(c => c.FilmDatas.FindAsync(It.IsAny<string>())).Returns(new ValueTask<FilmData>((films.First())));

            var filmDataBusiness = new FilmDataBusiness(mockContext.Object);
            var mapper = new Mapper();
            // Act
            var resultFilm = filmDataBusiness.GetTopFilms(mapper.MapToFilmCardViewModel).ToList();

            // Assert
            Assert.AreEqual(2, resultFilm.Count, "Doesn't return all searched elements in the database.");
        }

        [Test]
        public void GetRecentFilmsReturnsAllElementsWithTheRecentReleaseDate()
        {
            // Arrange
            var films = new List<FilmData>
            {
                new FilmData {ReleaseDate=DateTime.Now, Title="XXX", FilmId="Film1",
                    Film=new Film(),
            },
                new FilmData {ReleaseDate=DateTime.UtcNow, Title="YYY",  FilmId="Film1",
                    Film=new Film() },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<FilmData>>();
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.Provider).Returns(films.Provider);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.Expression).Returns(films.Expression);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.ElementType).Returns(films.ElementType);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.GetEnumerator()).Returns(films.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.FilmDatas).Returns(mockSet.Object);
            mockContext.Setup(c => c.FilmDatas.FindAsync(It.IsAny<string>())).Returns(new ValueTask<FilmData>((films.First())));

            var filmDataBusiness = new FilmDataBusiness(mockContext.Object);
            var mapper = new Mapper();
            // Act
            var resultFilm = filmDataBusiness.GetRecentFilms(mapper.MapToFilmCardViewModel).ToList();

            // Assert
            Assert.AreEqual(2, resultFilm.Count, "Doesn't return all searched elements in the database.");
        }

        [Test]
        public void CountAllFilmsReturnsTheCountOfTheFilmsInTheDb()
        {
            // Arrange
            var films = new List<Film>
            {
                new Film{},
                new Film{},
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Film>>();
            mockSet.As<IQueryable<Film>>().Setup(m => m.Provider).Returns(films.Provider);
            mockSet.As<IQueryable<Film>>().Setup(m => m.Expression).Returns(films.Expression);
            mockSet.As<IQueryable<Film>>().Setup(m => m.ElementType).Returns(films.ElementType);
            mockSet.As<IQueryable<Film>>().Setup(m => m.GetEnumerator()).Returns(films.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            //Tuka pak Films e prazno :(
            mockContext.Setup(c => c.Films).Returns(mockSet.Object);

            var filmDataBusiness = new FilmDataBusiness(mockContext.Object);

            // Act
            var filmsCount = filmDataBusiness.CountAllFilms();

            // Assert
            Assert.AreEqual(2, filmsCount, "Doesn't return all searched elements in the database.");
        } 
    }
}
