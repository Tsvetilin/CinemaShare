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
    class FilmBusinessTests
    {
        [Test]
        public void GetAllReturnsAllElements()
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
            Assert.AreEqual(2, allFilms.Count, "Doesn't return all elements of the film.");
        }

        [Test]
        public async Task GetAsyncReturnsSearchedElement()
        {
            // Arrange
            var films = new List<Film>
            {
                new Film
                {
                    Rating=  2,
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

            var searchedFilm = films.First(x => x.Rating == 2);

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Films).Returns(mockSet.Object);
            mockContext.Setup(s => s.Films.FindAsync(It.IsAny<string>())).
                       Returns(new ValueTask<Film>(films.FirstOrDefault(y => y.Id == searchedFilm.Id)));

            var filmBusiness = new FilmBusiness(mockContext.Object);

            // Act
            var resultFilm = await filmBusiness.GetAsync(searchedFilm.Id);

            // Assert
            Assert.AreEqual(searchedFilm.Id, resultFilm.Id, "Doesn't return searched element from the film.");
            Assert.AreEqual(searchedFilm.Rating, resultFilm.Rating, "Doesn't return searched element from the film.");
            Assert.AreEqual(searchedFilm.AddedByUser, resultFilm.AddedByUser, "Doesn't return searched element from the film.");
            Assert.AreEqual(searchedFilm.AddedByUserId, resultFilm.AddedByUserId, "Doesn't return searched element from the film.");
            Assert.AreEqual(searchedFilm.FilmData, resultFilm.FilmData, "Doesn't return searched element from the film.");
            Assert.AreEqual(searchedFilm.FilmProjection, resultFilm.FilmProjection, "Doesn't return searched element from the film.");
            Assert.AreEqual(searchedFilm.FilmReviews, resultFilm.FilmReviews, "Doesn't return searched element from the film.");
            Assert.AreEqual(searchedFilm.OnWatchlistUsers, resultFilm.OnWatchlistUsers, "Doesn't return searched element from the film.");
            Assert.AreEqual(searchedFilm.Rating, resultFilm.Rating, "Doesn't return searched element from the film.");
            Assert.AreEqual(searchedFilm.Ratings, resultFilm.Ratings, "Doesn't return searched element from the film.");
        }

        [Test]
        public async Task AddAsyncAddsElement()
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
            mockContext.Setup(m => m.AddAsync(It.IsAny<Film>(), It.IsAny<CancellationToken>()))
                .Returns(new ValueTask<EntityEntry<Film>>(Task.FromResult((EntityEntry<Film>)null)));

            var filmBusiness = new FilmBusiness(mockContext.Object);

            // Act
            await filmBusiness.AddAsync(film);

            // Assert
            mockSet.Verify(m => m.AddAsync(It.IsAny<Film>(), It.IsAny<CancellationToken>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task DeleteAsyncDeletesElement()
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

            var searchedFilm = films.First();

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Films).Returns(mockSet.Object);
            mockContext.Setup(s => s.Films.FindAsync(It.IsAny<string>())).Returns(new ValueTask<Film>(searchedFilm));
            mockContext.Setup(m => m.Remove(It.IsAny<Film>())).Returns((EntityEntry<Film>)null);

            var filmBusiness = new FilmBusiness(mockContext.Object);

            // Act
            await filmBusiness.DeleteAsync(searchedFilm.Id);

            // Assert
            mockSet.Verify(m => m.Remove(It.IsAny<Film>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task DeleteAsyncDoesntDeleteUnexistingElement()
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

            var searchedFilm = films.First();

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Films).Returns(mockSet.Object);
            mockContext.Setup(s => s.Films.FindAsync(It.IsAny<string>())).Returns(new ValueTask<Film>((Film)null));
            mockContext.Setup(m => m.Remove(It.IsAny<Film>())).Returns((EntityEntry<Film>)null);

            var filmBusiness = new FilmBusiness(mockContext.Object);

            // Act
            await filmBusiness.DeleteAsync(searchedFilm.Id);

            // Assert
            mockSet.Verify(m => m.Remove(It.IsAny<Film>()), Times.Never());
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Test]
        public async Task RateAsyncRatesTheFilmByChangingExistingRate()
        {
            // Arrange
            var films = new List<Film>
            {
                new Film
                {
                    Rating=  2,
                    Ratings = new List<FilmRating>{
                        new FilmRating
                        {
                            UserId="TestUserId",
                            Rating=2
                        }
                    }
                },
                new Film {Rating = 3}
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Film>>();
            mockSet.As<IQueryable<Film>>().Setup(m => m.Provider).Returns(films.Provider);
            mockSet.As<IQueryable<Film>>().Setup(m => m.Expression).Returns(films.Expression);
            mockSet.As<IQueryable<Film>>().Setup(m => m.ElementType).Returns(films.ElementType);
            mockSet.As<IQueryable<Film>>().Setup(m => m.GetEnumerator()).Returns(films.GetEnumerator());

            var searchedFilm = films.First(x => x.Rating == 2);

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Films).Returns(mockSet.Object);
            mockContext.Setup(s => s.Films.FindAsync(It.IsAny<string>())).Returns(new ValueTask<Film>(searchedFilm));

            var filmBusiness = new FilmBusiness(mockContext.Object);
            var expectedRating = 5;

            // Act
            await filmBusiness.RateAsync(searchedFilm.Id, "TestUserId", expectedRating);
            var updatedFilm = mockSet.Object.First(x => x.Id == searchedFilm.Id);

            // Assert
            Assert.AreEqual(expectedRating, updatedFilm.Rating, "Film rating not updated");
            Assert.AreEqual(1, updatedFilm.Ratings.Count(), "Adds new rate when should update exisitng");
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task RateAsyncRatesTheFilmByCreatingNewRate()
        {
            // Arrange
            var films = new List<Film>
            {
                new Film
                {
                    Rating =  2,
                    Ratings = new List<FilmRating>{
                        new FilmRating
                        {
                            UserId="TestUserId",
                            Rating=2
                        }
                    }
                },
                new Film {Rating = 3}
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Film>>();
            mockSet.As<IQueryable<Film>>().Setup(m => m.Provider).Returns(films.Provider);
            mockSet.As<IQueryable<Film>>().Setup(m => m.Expression).Returns(films.Expression);
            mockSet.As<IQueryable<Film>>().Setup(m => m.ElementType).Returns(films.ElementType);
            mockSet.As<IQueryable<Film>>().Setup(m => m.GetEnumerator()).Returns(films.GetEnumerator());

            var searchedFilm = films.First(x => x.Rating == 2);

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Films).Returns(mockSet.Object);
            mockContext.Setup(s => s.Films.FindAsync(It.IsAny<string>())).Returns(new ValueTask<Film>(searchedFilm));

            var filmBusiness = new FilmBusiness(mockContext.Object);

            // Act
            await filmBusiness.RateAsync(searchedFilm.Id, "UnexistingUserId", 4);
            var updatedFilm = mockSet.Object.First(x => x.Id == searchedFilm.Id);

            // Assert
            Assert.AreEqual(3, updatedFilm.Rating, "Film rating not updated correctly");
            Assert.AreEqual(2, updatedFilm.Ratings.Count(), "Not creating new rate");
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task RateAsyncDoesntRateUnexistingFilm()
        {
            // Arrange
            var films = new List<Film>
            {
                new Film {Rating = 2},
                new Film {Rating = 3}
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Film>>();
            mockSet.As<IQueryable<Film>>().Setup(m => m.Provider).Returns(films.Provider);
            mockSet.As<IQueryable<Film>>().Setup(m => m.Expression).Returns(films.Expression);
            mockSet.As<IQueryable<Film>>().Setup(m => m.ElementType).Returns(films.ElementType);
            mockSet.As<IQueryable<Film>>().Setup(m => m.GetEnumerator()).Returns(films.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Films).Returns(mockSet.Object);
            mockContext.Setup(s => s.Films.FindAsync(It.IsAny<string>())).Returns(new ValueTask<Film>((Film)null));

            var filmBusiness = new FilmBusiness(mockContext.Object);

            // Act
            await filmBusiness.RateAsync("UnexisingFilmId", "TestUserId", 4);

            // Assert
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Test]
        public async Task RateAsyncDoesntRateNullUserId()
        {
            // Arrange
            var films = new List<Film>
            {
                new Film {Rating = 2},
                new Film {Rating = 3}
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Film>>();
            mockSet.As<IQueryable<Film>>().Setup(m => m.Provider).Returns(films.Provider);
            mockSet.As<IQueryable<Film>>().Setup(m => m.Expression).Returns(films.Expression);
            mockSet.As<IQueryable<Film>>().Setup(m => m.ElementType).Returns(films.ElementType);
            mockSet.As<IQueryable<Film>>().Setup(m => m.GetEnumerator()).Returns(films.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Films).Returns(mockSet.Object);
            mockContext.Setup(s => s.Films.FindAsync(It.IsAny<string>())).Returns(new ValueTask<Film>((Film)null));

            var filmBusiness = new FilmBusiness(mockContext.Object);

            // Act
            await filmBusiness.RateAsync(films.First().Id, null, 4);

            // Assert
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }


        [Test]
        public async Task AddToWatchListAsyncAddsTheFilm()
        {
            // Arrange
            var email = "TestUserEmail@email.com";
            var username = "TestUserUsername";
            var user = new CinemaUser
            {
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Email = email,
                NormalizedEmail = email.ToUpper(),
                EmailConfirmed = true,
                UserName = username,
                NormalizedUserName = username.ToUpper(),
                FirstName = username,
                LastName = username,
                Gender = GenderType.Male,
                SecurityStamp = Guid.NewGuid().ToString(),
                WatchList = new List<Film> { new Film() }
            };
            var users = new List<CinemaUser>{user}.AsQueryable();

            var mockSet = new Mock<DbSet<CinemaUser>>();
            mockSet.As<IQueryable<CinemaUser>>().Setup(m => m.Provider).Returns(users.Provider);
            mockSet.As<IQueryable<CinemaUser>>().Setup(m => m.Expression).Returns(users.Expression);
            mockSet.As<IQueryable<CinemaUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockSet.As<IQueryable<CinemaUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);
            mockContext.Setup(s => s.Users.FindAsync(It.IsAny<string>())).Returns(new ValueTask<CinemaUser>(user));

            var filmBusiness = new FilmBusiness(mockContext.Object);

            var film = new Film { Rating = 2 };

            // Act
            await filmBusiness.AddToWatchListAsync(user.Id, film);

            // Assert
            Assert.AreEqual(2, mockSet.Object.First().WatchList.Count(), "Film not added to watchlist");
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task AddToWatchListAsyncDoesntAddAlredyAddedFilm()
        {
            // Arrange
            var film = new Film { Rating = 2 };
            var user = new CinemaUser{WatchList = new List<Film> { film }};
            var users = new List<CinemaUser>{user}.AsQueryable();

            var mockSet = new Mock<DbSet<CinemaUser>>();
            mockSet.As<IQueryable<CinemaUser>>().Setup(m => m.Provider).Returns(users.Provider);
            mockSet.As<IQueryable<CinemaUser>>().Setup(m => m.Expression).Returns(users.Expression);
            mockSet.As<IQueryable<CinemaUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockSet.As<IQueryable<CinemaUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);
            mockContext.Setup(s => s.Users.FindAsync(It.IsAny<string>())).Returns(new ValueTask<CinemaUser>(user));

            var filmBusiness = new FilmBusiness(mockContext.Object);

            // Act
            await filmBusiness.AddToWatchListAsync(user.Id, film);

            // Assert
            Assert.AreEqual(1, mockSet.Object.First().WatchList.Count(), "Film added to watchlist when already exists");
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Test]
        public async Task AddToWatchListAsyncDoesntAddForUnexistingUser()
        {
            // Arrange
            var film = new Film { Rating = 2 };
            var users = new List<CinemaUser>
            {
                new CinemaUser{Id="TestUserId" }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<CinemaUser>>();
            mockSet.As<IQueryable<CinemaUser>>().Setup(m => m.Provider).Returns(users.Provider);
            mockSet.As<IQueryable<CinemaUser>>().Setup(m => m.Expression).Returns(users.Expression);
            mockSet.As<IQueryable<CinemaUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockSet.As<IQueryable<CinemaUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);
            mockContext.Setup(s => s.Users.FindAsync(It.IsAny<string>())).
                        Returns(new ValueTask<CinemaUser>((CinemaUser)null));

            var filmBusiness = new FilmBusiness(mockContext.Object);

            // Act
            await filmBusiness.AddToWatchListAsync("UnexistingUserId", film);

            // Assert
            Assert.AreEqual(0, mockSet.Object.First().WatchList.Count(), "Film added to watchlist for unexising user");
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Test]
        public async Task RemoveFromWatchListAsyncRemovesTheFilm()
        {
            // Arrange
            var film = new Film();
            var user = new CinemaUser
            {
                WatchList = new List<Film> { film }
            };
            var users = new List<CinemaUser> { user }.AsQueryable();

            var mockSet = new Mock<DbSet<CinemaUser>>();
            mockSet.As<IQueryable<CinemaUser>>().Setup(m => m.Provider).Returns(users.Provider);
            mockSet.As<IQueryable<CinemaUser>>().Setup(m => m.Expression).Returns(users.Expression);
            mockSet.As<IQueryable<CinemaUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockSet.As<IQueryable<CinemaUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);
            mockContext.Setup(s => s.Users.FindAsync(It.IsAny<string>())).Returns(new ValueTask<CinemaUser>(user));

            var filmBusiness = new FilmBusiness(mockContext.Object);

            // Act
            await filmBusiness.RemoveFromWatchListAsync(user.Id, film);

            // Assert
            Assert.AreEqual(0, mockSet.Object.First().WatchList.Count(), "Film not removed from watchlist");
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task RemoveFromWatchListAsyncDoesntRemoveNotAddedFilm()
        {
            // Arrange
            var film = new Film { Rating = 2 };
            var user = new CinemaUser { WatchList = new List<Film> { film } };
            var users = new List<CinemaUser> { user }.AsQueryable();

            var mockSet = new Mock<DbSet<CinemaUser>>();
            mockSet.As<IQueryable<CinemaUser>>().Setup(m => m.Provider).Returns(users.Provider);
            mockSet.As<IQueryable<CinemaUser>>().Setup(m => m.Expression).Returns(users.Expression);
            mockSet.As<IQueryable<CinemaUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockSet.As<IQueryable<CinemaUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);
            mockContext.Setup(s => s.Users.FindAsync(It.IsAny<string>())).Returns(new ValueTask<CinemaUser>(user));

            var filmBusiness = new FilmBusiness(mockContext.Object);

            // Act
            await filmBusiness.RemoveFromWatchListAsync(user.Id, new Film());

            // Assert
            Assert.AreEqual(1, mockSet.Object.First().WatchList.Count(), "Wrong film removed from watchlist");
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Test]
        public async Task RemoveFromWatchListAsyncDoesntRemoveForUnexistingUser()
        {
            // Arrange
            var film = new Film { Rating = 2 };
            var users = new List<CinemaUser>
            {
                new CinemaUser{Id="TestUserId" }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<CinemaUser>>();
            mockSet.As<IQueryable<CinemaUser>>().Setup(m => m.Provider).Returns(users.Provider);
            mockSet.As<IQueryable<CinemaUser>>().Setup(m => m.Expression).Returns(users.Expression);
            mockSet.As<IQueryable<CinemaUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockSet.As<IQueryable<CinemaUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);
            mockContext.Setup(s => s.Users.FindAsync(It.IsAny<string>())).
                        Returns(new ValueTask<CinemaUser>((CinemaUser)null));

            var filmBusiness = new FilmBusiness(mockContext.Object);

            // Act
            await filmBusiness.RemoveFromWatchListAsync("UnexistingUserId", film);

            // Assert
            Assert.AreEqual(0, mockSet.Object.First().WatchList.Count(), "Film removed from watchlist for wrong user");
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Test]
        public void GetWatchListReturnsAllElements()
        {
            // Arrange
            var filmData1 = new FilmData
            {
                Title = "Title1",
                Genre = new List<GenreType> { new GenreType { Genre = Genre.Action } },
                Poster = "Poster1",
                FilmId = "Film1"
            };
            var filmData2 = new FilmData
            {
                Title = "Title2",
                Genre = new List<GenreType> { new GenreType { Genre = Genre.Action } },
                Poster = "Poster2",
                FilmId = "Film2"
            };
            var film1 = new Film
            {
                Rating = 2,
                Id = "Film1",
                FilmData = filmData1
            };
            var film2 = new Film
            {
                Rating = 3,
                Id = "Film2",
                FilmData = filmData2
            };
            filmData1.Film = film1;
            filmData2.Film = film2;
            var user = new CinemaUser {
                WatchList = new List<Film> {film1, film2 }
            };
            var users = new List<CinemaUser> { user }.AsQueryable();

            var mockSet = new Mock<DbSet<CinemaUser>>();
            mockSet.As<IQueryable<CinemaUser>>().Setup(m => m.Provider).Returns(users.Provider);
            mockSet.As<IQueryable<CinemaUser>>().Setup(m => m.Expression).Returns(users.Expression);
            mockSet.As<IQueryable<CinemaUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockSet.As<IQueryable<CinemaUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);
            mockContext.Setup(s => s.Users.FindAsync(It.IsAny<string>())).Returns(new ValueTask<CinemaUser>(user));

            var filmBusiness = new FilmBusiness(mockContext.Object);
            var mapper = new Mapper();
            var expectedWatchList = user.WatchList.Select(x => mapper.MapToFilmCardViewModel(x.FilmData)).ToList();

            // Act
            var resultWatchList  = filmBusiness.GetWatchList(user.Id, mapper.MapToFilmCardViewModel).ToList();

            // Assert
            Assert.AreEqual(expectedWatchList.Count, resultWatchList.Count, "Incorrect watchlist returned");
            Assert.AreEqual(expectedWatchList[0].Id, resultWatchList[0].Id,"Incorect data returned");
            Assert.AreEqual(expectedWatchList[0].Poster, resultWatchList[0].Poster, "Incorect data returned");
            Assert.AreEqual(expectedWatchList[0].Rating, resultWatchList[0].Rating, "Incorect data returned");
            Assert.AreEqual(expectedWatchList[0].Title, resultWatchList[0].Title, "Incorect data returned");
            Assert.AreEqual(expectedWatchList[0].Genres, resultWatchList[0].Genres, "Incorect data returned");
            Assert.AreEqual(expectedWatchList[1].Id, resultWatchList[1].Id, "Incorect data returned");
        }

        [Test]
        public void GetWatchListReturnsNoElementsForInvalidUser()
        {
            // Arrange
            var filmData1 = new FilmData
            {
                Title = "Title1",
                Genre = new List<GenreType> { new GenreType { Genre = Genre.Action } },
                Poster = "Poster1",
                FilmId = "Film1"
            };
            var filmData2 = new FilmData
            {
                Title = "Title2",
                Genre = new List<GenreType> { new GenreType { Genre = Genre.Action } },
                Poster = "Poster2",
                FilmId = "Film2"
            };
            var film1 = new Film
            {
                Rating = 2,
                Id = "Film1",
                FilmData = filmData1
            };
            var film2 = new Film
            {
                Rating = 3,
                Id = "Film2",
                FilmData = filmData2
            };
            filmData1.Film = film1;
            filmData2.Film = film2;
            var user = new CinemaUser
            {
                WatchList = new List<Film> { film1, film2 }
            };
            var users = new List<CinemaUser> { user }.AsQueryable();

            var mockSet = new Mock<DbSet<CinemaUser>>();
            mockSet.As<IQueryable<CinemaUser>>().Setup(m => m.Provider).Returns(users.Provider);
            mockSet.As<IQueryable<CinemaUser>>().Setup(m => m.Expression).Returns(users.Expression);
            mockSet.As<IQueryable<CinemaUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockSet.As<IQueryable<CinemaUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);
            mockContext.Setup(s => s.Users.FindAsync(It.IsAny<string>())).
                        Returns(new ValueTask<CinemaUser>(Task.FromResult((CinemaUser)null)));

            var filmBusiness = new FilmBusiness(mockContext.Object);
            var mapper = new Mapper();
            var expectedWatchList = new List<FilmCardViewModel>();

            // Act
            var resultWatchList = filmBusiness.GetWatchList("UnexistingUserId", mapper.MapToFilmCardViewModel);

            // Assert
            Assert.AreEqual(null, resultWatchList, "Incorrect watchlist returned");
        }
    }
}
