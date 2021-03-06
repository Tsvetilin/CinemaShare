﻿using Business;
using CinemaShare.Common.Mapping;
using Data;
using Data.Enums;
using Data.Models;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tests.Business.Tests
{
    class ProjectionBusinessTests
    {
        [Test]
        public async Task AddAddsElementToDb()
        {
            // Arrange
            var projections = new List<FilmProjection>
            {
                new FilmProjection {},
                new FilmProjection {}
            }.AsQueryable();

            var mockSet = new Mock<DbSet<FilmProjection>>();
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.Provider).Returns(projections.Provider);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.Expression).Returns(projections.Expression);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.ElementType).Returns(projections.ElementType);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.GetEnumerator()).
                                                     Returns(projections.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.FilmProjections).Returns(mockSet.Object);
            mockContext.Setup(m => m.AddAsync(It.IsAny<FilmProjection>(), It.IsAny<CancellationToken>()))
                .Returns(new ValueTask<EntityEntry<FilmProjection>>
                (Task.FromResult((EntityEntry<FilmProjection>)null)));

            var projectionBusiness = new FilmProjectionBusiness(mockContext.Object,
                                                                new EmailSender("TestAPIKey",
                                                                                "TestSender",
                                                                                "TestSenderName"));
            var projection = new FilmProjection();
            var mapper = new Mapper();
            // Act
            await projectionBusiness.AddAsync(projection);

            // Assert
            mockSet.Verify(m => m.AddAsync(It.IsAny<FilmProjection>(), It.IsAny<CancellationToken>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task GetReturnsSearchedElementFromDb()
        {
            // Arrange
            var projections = new List<FilmProjection>
            {
                new FilmProjection {Id = "1"},
                new FilmProjection {Id = "2"}
            }.AsQueryable();

            var mockSet = new Mock<DbSet<FilmProjection>>();
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.Provider).Returns(projections.Provider);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.Expression).Returns(projections.Expression);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.ElementType).Returns(projections.ElementType);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.GetEnumerator()).Returns(projections.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.FilmProjections).Returns(mockSet.Object);
            mockContext.Setup(c => c.FilmProjections.FindAsync(It.IsAny<string>())).Returns(new ValueTask<FilmProjection>((projections.First())));


            var projectionBusiness = new FilmProjectionBusiness(mockContext.Object, 
                                                                new EmailSender("TestAPIKey", 
                                                                                "TestSender",
                                                                                "TestSenderName"));
            var searchedProj = projections.First();

            // Act
            var resultProj = await projectionBusiness.GetAsync(searchedProj.Id);

            // Assert
            Assert.AreEqual(searchedProj.Id, resultProj.Id, "Doesn't return the searched element from the database.");
        }

        [Test]
        public void CountAllProjectionsReturnsTheCountOfTheProjectionsInTheDb()
        {
            // Arrange
            var projections = new List<FilmProjection>
            {
                new FilmProjection {},
                new FilmProjection {}
            }.AsQueryable();

            var mockSet = new Mock<DbSet<FilmProjection>>();
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.Provider).Returns(projections.Provider);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.Expression).Returns(projections.Expression);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.ElementType).Returns(projections.ElementType);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.GetEnumerator()).Returns(projections.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.FilmProjections).Returns(mockSet.Object);
            mockContext.Setup(c => c.FilmProjections.FindAsync(It.IsAny<string>())).Returns(new ValueTask<FilmProjection>((projections.First())));


            var projectionBusiness = new FilmProjectionBusiness(mockContext.Object, 
                                                                new EmailSender("TestAPIKey", 
                                                                                "TestSender",
                                                                                "TestSenderName"));
            var searchedProj = projections.First();

            // Act
            var resultProjCount = projectionBusiness.CountAllProjections();

            // Assert
            Assert.AreEqual(projections.Count(), resultProjCount, "Doesn't return the searched element from the database.");
        }

        [Test]
        public void GetPageItemsReturnsTheProjectionsOnThePage()
        {
            // Arrange
            var projections = new List<FilmProjection>
            {
                new FilmProjection 
                { 
                    Id = "1",
                    Cinema = new Cinema{Name = "Best1", City="Sliven", Country="Bulgaria"},
                    ProjectionType = ProjectionType._4D,
                    Date = DateTime.Now,
                    Film = new Film
                    {
                        FilmData = new FilmData {Title="FilmTitle"}
                    } 
                },
                new FilmProjection 
                { 
                    Id = "2",
                    Cinema = new Cinema{Name = "Best2", City="Sliven", Country="Bulgaria"},
                    ProjectionType = ProjectionType._4D,
                    Date = DateTime.Now,
                    Film = new Film
                    {
                        FilmData = new FilmData {Title="FilmNewTitle"}
                    } 
                }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<FilmProjection>>();
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.Provider).Returns(projections.Provider);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.Expression).Returns(projections.Expression);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.ElementType).Returns(projections.ElementType);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.GetEnumerator()).Returns(projections.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.FilmProjections).Returns(mockSet.Object);
            mockContext.Setup(c => c.FilmProjections.FindAsync(It.IsAny<string>())).
                        Returns(new ValueTask<FilmProjection>((projections.First())));


            var projectionBusiness = new FilmProjectionBusiness(mockContext.Object, 
                                                                new EmailSender("TestAPIKey",
                                                                                "TestSender",
                                                                                "TestSenderName"));
            var mapper = new Mapper();
            // Act
            var resultProjections = projectionBusiness.GetPageItems(1, 2, mapper.MapToProjectionCardViewModel);

            // Assert
            Assert.AreEqual(projections.Count(), resultProjections.Count(), "Doesn't return all projections on the page.");
        }

        [Test]
        public async Task GetAsyncReturnsSearchedElementMapped()
        {
            // Arrange
            var projections = new List<FilmProjection>
            {
                 new FilmProjection 
                 {
                     Id = "1",
                    Cinema = new Cinema
                    {
                        Name = "Best", City="Sliven", Country="Bulgaria"
                    },
                    ProjectionType = ProjectionType._4D,
                    Date = DateTime.Now,
                    Film = new Film
                    {
                        FilmData = new FilmData 
                        {
                            Title="FilmTitle",TargetAudience=TargetAudience.Adults,Runtime=50
                        }
                    },
                    TotalTickets = 100,
                    TicketPrices=new TicketPrices()
                 },
                new FilmProjection {Id = "2"},
            }.AsQueryable();

            var mockSet = new Mock<DbSet<FilmProjection>>();
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.Provider).Returns(projections.Provider);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.Expression).Returns(projections.Expression);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.ElementType).Returns(projections.ElementType);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.GetEnumerator()).Returns(projections.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.FilmProjections).Returns(mockSet.Object);
            mockContext.Setup(c => c.FilmProjections.FindAsync(It.IsAny<string>())).
                        Returns(new ValueTask<FilmProjection>(projections.First()));


            var projectionBusiness = new FilmProjectionBusiness(mockContext.Object, 
                                                                new EmailSender("TestAPIKey", 
                                                                                "TestSender",
                                                                                "TestSenderName"));
            var searchedProj = projections.First();
            var mapper = new Mapper();

            // Act
            var resultProj = await projectionBusiness.GetAsync(searchedProj.Id, mapper.MapToProjectionDataViewModel);

            // Assert
            Assert.AreEqual(searchedProj.Id, resultProj.Id, "Doesn't return the searched projection data mapped.");
        }

        [Test]
        public void GetAllReturnsAllProjecitons()
        {
            // Arrange
            var projections = new List<FilmProjection>
            {
                 new FilmProjection { Id = "1"},
                new FilmProjection {Id = "2"},
            }.AsQueryable();

            var mockSet = new Mock<DbSet<FilmProjection>>();
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.Provider).Returns(projections.Provider);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.Expression).Returns(projections.Expression);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.ElementType).Returns(projections.ElementType);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.GetEnumerator()).Returns(projections.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.FilmProjections).Returns(mockSet.Object);
            mockContext.Setup(c => c.FilmProjections.FindAsync(It.IsAny<string>())).
                        Returns(new ValueTask<FilmProjection>(projections.First()));

            var projectionBusiness = new FilmProjectionBusiness(mockContext.Object, 
                                                                new EmailSender("TestAPIKey", 
                                                                                "TestSender", 
                                                                                "TestSenderName"));

            // Act
            var resultProjecitonsCount = projectionBusiness.GetAll().Count();

            // Assert
            Assert.AreEqual(projections.Count(), resultProjecitonsCount, "Doesn't return all projections from the database.");
        }

        [Test]
        public void GetAllReturnsAllProjecitonsMapped()
        {
            // Arrange
            var projections = new List<FilmProjection>
            {
                new FilmProjection 
                {
                    Id = "1",
                    Cinema = new Cinema
                    {
                        Name = "Best1", City="Sliven", Country="Bulgaria"
                    },
                    ProjectionType = ProjectionType._4D,
                    Date = DateTime.Now,
                    Film = new Film
                    {
                        FilmData = new FilmData {Title="FilmTitle"}
                    } 
                },
                new FilmProjection 
                { 
                    Id = "2",
                    Cinema = new Cinema
                    {
                        Name = "Best2", City="Sliven", Country="Bulgaria"
                    },
                    ProjectionType = ProjectionType._4D,
                    Date = DateTime.Now,
                    Film = new Film
                    {
                        FilmData = new FilmData {Title="FilmNewTitle"}
                    } 
                }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<FilmProjection>>();
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.Provider).Returns(projections.Provider);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.Expression).Returns(projections.Expression);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.ElementType).Returns(projections.ElementType);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.GetEnumerator()).Returns(projections.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.FilmProjections).Returns(mockSet.Object);
            mockContext.Setup(c => c.FilmProjections.FindAsync(It.IsAny<string>())).
                Returns(new ValueTask<FilmProjection>(projections.First()));

            var projectionBusiness = new FilmProjectionBusiness(mockContext.Object,
                                                                new EmailSender("TestAPIKey",
                                                                                "TestSender",
                                                                                "TestSenderName")); 
            var mapper = new Mapper();

            // Act
            var resultProjecitonsCount = projectionBusiness.GetAll(mapper.MapToProjectionCardViewModel).Count();

            // Assert
            Assert.AreEqual(projections.Count(), resultProjecitonsCount, "Doesn't return all projections data mapped.");
        }

        [Test]
        public void GetAllByCinemaIdReturnsAllProjecitonsByCinemaId()
        {
            // Arrange
            var projections = new List<FilmProjection>
            {
                new FilmProjection 
                {
                    Id = "1",
                    CinemaId = "1",
                    Cinema = new Cinema
                    {   Id="1",
                        Name = "Best", 
                        City="Sliven",
                        Country="Bulgaria"
                    },
                    ProjectionType = ProjectionType._4D,
                    Date = DateTime.Now,
                    Film = new Film
                    {
                        FilmData = new FilmData
                        {
                            Title="FilmTitle",
                            TargetAudience=TargetAudience.Adults,
                            Runtime=50
                        }
                    },
                    TotalTickets = 100,
                    TicketPrices=new TicketPrices()
                },
               new FilmProjection
                {    
                    Id = "2",
                    CinemaId = "1",
                    Cinema = new Cinema
                    {
                        Id="1",
                        Name = "Best1",
                        City="Sliven",
                        Country="Bulgaria"
                    },
                    ProjectionType = ProjectionType._4D,
                    Date = DateTime.Now,
                    Film = new Film
                    {
                        FilmData = new FilmData
                        {
                            Title="FilmTitle",
                            TargetAudience=TargetAudience.Adults,
                            Runtime=50
                        }
                    },
                    TotalTickets = 100,
                    TicketPrices=new TicketPrices()},
                new FilmProjection 
                {
                    Id = "3", 
                    Cinema = new Cinema
                    {
                        Id="2"
                    } 
                },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<FilmProjection>>();
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.Provider).Returns(projections.Provider);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.Expression).Returns(projections.Expression);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.ElementType).Returns(projections.ElementType);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.GetEnumerator()).Returns(projections.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.FilmProjections).Returns(mockSet.Object);

            var projectionBusiness = new FilmProjectionBusiness(mockContext.Object,
                                                                new EmailSender("TestAPIKey",
                                                                                "TestSender",
                                                                                "TestSenderName")); 
            var mapper = new Mapper();

            // Act
            var resultProjecitonsCount = projectionBusiness.GetAllByCinemaId("1", mapper.MapToProjectionDataViewModel).
                                                            ToList().
                                                            Count();

            // Assert
            Assert.AreEqual(2, resultProjecitonsCount, "Doesn't return all projections with the searched cinema id.");
        }

        [Test]
        public async Task DeleteRemovesNothing()
        {
            // Arrange
            var projections = new List<FilmProjection>
            {
                new FilmProjection { },
                new FilmProjection { },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<FilmProjection>>();
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.Provider).Returns(projections.Provider);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.Expression).Returns(projections.Expression);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.ElementType).Returns(projections.ElementType);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.GetEnumerator()).Returns(projections.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.FilmProjections).Returns(mockSet.Object);
            mockContext.Setup(s => s.FilmProjections.FindAsync(It.IsAny<string>())).
                        Returns(new ValueTask<FilmProjection>((FilmProjection)null));
            mockContext.Setup(m => m.Remove(It.IsAny<FilmProjection>())).Returns((EntityEntry<FilmProjection>)null);

            var projectionBusiness = new FilmProjectionBusiness(mockContext.Object, 
                                                                new EmailSender("TestAPIKey",
                                                                                "TestSender", 
                                                                                "TestSenderName"));

            // Act
             await projectionBusiness.DeleteAsync(projections.First().Id,"");

            // Assert
            mockSet.Verify(m => m.Remove(It.IsAny<FilmProjection>()), Times.Never());
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Test]
        public async Task DeleteRemovesSelectedElement()
        {
            // Arrange
            var projections = new List<FilmProjection>
            {
                new FilmProjection { },
                new FilmProjection { },
            }.AsQueryable();

            var projectionTickets = new List<ProjectionTicket>
            {
                new ProjectionTicket { },
                new ProjectionTicket { },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<FilmProjection>>();
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.Provider).Returns(projections.Provider);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.Expression).Returns(projections.Expression);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.ElementType).Returns(projections.ElementType);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.GetEnumerator()).Returns(projections.GetEnumerator());

            var mockSetTicket = new Mock<DbSet<ProjectionTicket>>();
            mockSetTicket.As<IQueryable<ProjectionTicket>>().Setup(m => m.Provider).
                                                             Returns(projectionTickets.Provider);
            mockSetTicket.As<IQueryable<ProjectionTicket>>().Setup(m => m.Expression).
                                                             Returns(projectionTickets.Expression);
            mockSetTicket.As<IQueryable<ProjectionTicket>>().Setup(m => m.ElementType).
                                                             Returns(projectionTickets.ElementType);
            mockSetTicket.As<IQueryable<ProjectionTicket>>().Setup(m => m.GetEnumerator()).
                                                             Returns(projectionTickets.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.FilmProjections).Returns(mockSet.Object);
            mockContext.Setup(c => c.ProjectionTickets).Returns(mockSetTicket.Object);
            mockContext.Setup(s => s.FilmProjections.FindAsync(It.IsAny<string>())).
                        Returns(new ValueTask<FilmProjection>(projections.First()));
            mockContext.Setup(m => m.Remove(It.IsAny<FilmProjection>())).Returns((EntityEntry<FilmProjection>)null);

            var projectionBusiness = new FilmProjectionBusiness(mockContext.Object, 
                                                                new EmailSender("TestAPIKey",
                                                                                "TestSender", 
                                                                                "TestSenderName"));

            // Act
            await projectionBusiness.DeleteAsync(projections.First().Id,"DeletePattern");

            // Assert
            mockSet.Verify(m => m.Remove(It.IsAny<FilmProjection>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task DeleteRemovesSelectedElementAndSendsEmail()
        {
            // Arrange
            var projections = new List<FilmProjection>
            {
                new FilmProjection
                {
                    Film = new Film{ FilmData = new FilmData{Title="Film1" } },
                    Cinema = new Cinema{Name = "Cinema1" },
                    Date = DateTime.Now
                },
                new FilmProjection { },
            }.AsQueryable();

            var projectionTickets = new List<ProjectionTicket>
            {
                new ProjectionTicket 
                {
                    Projection = projections.First(),
                    ProjectionId = projections.First().Id,
                    Holder = new CinemaUser
                    {
                        Email="HodlerEmail"
                    } 
                },
                new ProjectionTicket 
                {

                },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<FilmProjection>>();
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.Provider).Returns(projections.Provider);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.Expression).Returns(projections.Expression);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.ElementType).Returns(projections.ElementType);
            mockSet.As<IQueryable<FilmProjection>>().Setup(m => m.GetEnumerator()).Returns(projections.GetEnumerator());

            var mockSetTicket = new Mock<DbSet<ProjectionTicket>>();
            mockSetTicket.As<IQueryable<ProjectionTicket>>().Setup(m => m.Provider).
                                                             Returns(projectionTickets.Provider);
            mockSetTicket.As<IQueryable<ProjectionTicket>>().Setup(m => m.Expression).
                                                             Returns(projectionTickets.Expression);
            mockSetTicket.As<IQueryable<ProjectionTicket>>().Setup(m => m.ElementType).
                                                             Returns(projectionTickets.ElementType);
            mockSetTicket.As<IQueryable<ProjectionTicket>>().Setup(m => m.GetEnumerator()).
                                                             Returns(projectionTickets.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.FilmProjections).Returns(mockSet.Object);
            mockContext.Setup(c => c.ProjectionTickets).Returns(mockSetTicket.Object);
            mockContext.Setup(s => s.FilmProjections.FindAsync(It.IsAny<string>())).
                        Returns(new ValueTask<FilmProjection>(projections.First()));
            mockContext.Setup(m => m.Remove(It.IsAny<FilmProjection>())).Returns((EntityEntry<FilmProjection>)null);

            var projectionBusiness = new FilmProjectionBusiness(mockContext.Object,
                                                                new EmailSender("TestAPIKey",
                                                                                "TestSender",
                                                                                "TestSenderName"));
            // Act
            await projectionBusiness.DeleteAsync(projections.First().Id, "DeletePattern");

            // Assert
            mockSet.Verify(m => m.Remove(It.IsAny<FilmProjection>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
