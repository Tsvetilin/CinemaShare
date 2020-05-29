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
using CinemaShare.Common.Mapping;

namespace Tests.Business.Tests
{
    class TicketBusinessTests
    {
        [Test]
        public async Task GetAsyncReturnsSearchedElementMapped()
        {
            // Arrange
            var tickets = new List<ProjectionTicket>
            {
                new ProjectionTicket { },
                new ProjectionTicket { },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<ProjectionTicket>>();
            mockSet.As<IQueryable<ProjectionTicket>>().Setup(m => m.Provider).Returns(tickets.Provider);
            mockSet.As<IQueryable<ProjectionTicket>>().Setup(m => m.Expression).Returns(tickets.Expression);
            mockSet.As<IQueryable<ProjectionTicket>>().Setup(m => m.ElementType).Returns(tickets.ElementType);
            mockSet.As<IQueryable<ProjectionTicket>>().Setup(m => m.GetEnumerator()).Returns(tickets.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.ProjectionTickets).Returns(mockSet.Object);
            mockContext.Setup(c => c.ProjectionTickets.FindAsync(It.IsAny<string>())).
                        Returns(new ValueTask<ProjectionTicket>(tickets.First()));

            var projectionTicketBusiness = new ProjectionTicketBusiness(mockContext.Object);
            var searchedTicket = tickets.First();

            // Act
            var resultTicket = await projectionTicketBusiness.GetAsync(searchedTicket.Id);

            // Assert
            Assert.AreEqual(searchedTicket.Id, resultTicket.Id, "Doesn't return all elements in the database.");
        }

        [Test]
        public void GetAllReturnsAllTickets()
        {
            // Arrange
            var tickets = new List<ProjectionTicket>
            {
                new ProjectionTicket { },
                new ProjectionTicket { },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<ProjectionTicket>>();
            mockSet.As<IQueryable<ProjectionTicket>>().Setup(m => m.Provider).Returns(tickets.Provider);
            mockSet.As<IQueryable<ProjectionTicket>>().Setup(m => m.Expression).Returns(tickets.Expression);
            mockSet.As<IQueryable<ProjectionTicket>>().Setup(m => m.ElementType).Returns(tickets.ElementType);
            mockSet.As<IQueryable<ProjectionTicket>>().Setup(m => m.GetEnumerator()).Returns(tickets.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.ProjectionTickets).Returns(mockSet.Object);
            mockContext.Setup(c => c.ProjectionTickets.FindAsync(It.IsAny<string>())).
                        Returns(new ValueTask<ProjectionTicket>(tickets.First()));

            var projectionTicketBusiness = new ProjectionTicketBusiness(mockContext.Object);
            var searchedTicket = tickets.First();

            // Act
            var resultTicketsCount = projectionTicketBusiness.GetAll().ToList().Count();

            // Assert
            Assert.AreEqual(2, resultTicketsCount, "Doesn't return all tickets from the database.");
        }

        [Test]
        public async Task AddMultipleAddsRangeOfTickets()
        {
            // Arrange
            var tickets = new List<ProjectionTicket>
            {
                new ProjectionTicket { },
                new ProjectionTicket { },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<ProjectionTicket>>();
            mockSet.As<IQueryable<ProjectionTicket>>().Setup(m => m.Provider).Returns(tickets.Provider);
            mockSet.As<IQueryable<ProjectionTicket>>().Setup(m => m.Expression).Returns(tickets.Expression);
            mockSet.As<IQueryable<ProjectionTicket>>().Setup(m => m.ElementType).Returns(tickets.ElementType);
            mockSet.As<IQueryable<ProjectionTicket>>().Setup(m => m.GetEnumerator()).Returns(tickets.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.ProjectionTickets).Returns(mockSet.Object);
            mockContext.Setup(m => m.AddAsync(It.IsAny<ProjectionTicket>(), It.IsAny<CancellationToken>()))
                       .Returns(new ValueTask<EntityEntry<ProjectionTicket>>
                       (Task.FromResult((EntityEntry<ProjectionTicket>)null)));
            mockContext.Setup(c => c.ProjectionTickets.FindAsync(It.IsAny<string>())).
                        Returns(new ValueTask<ProjectionTicket>(tickets.First()));

            var projectionTicketBusiness = new ProjectionTicketBusiness(mockContext.Object);
            var projectionTickets = new List<ProjectionTicket> { new ProjectionTicket { }, new ProjectionTicket { } };

            // Act
            await projectionTicketBusiness.AddMultipleAsync(projectionTickets);

            // Assert 
            mockSet.Verify(m => m.AddAsync(It.IsAny<ProjectionTicket>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public void GetProjectionAndUserReturnsAllTicketsInTheEnteredProjectionAndItsCreater()
        {
            // Arrange
            var tickets = new List<ProjectionTicket>
            {
                new ProjectionTicket {Projection= new FilmProjection{Id="1" }, HolderId="User1" },
                new ProjectionTicket {Projection= new FilmProjection{Id="1" }, HolderId="User1"},
            }.AsQueryable();

            var mockSet = new Mock<DbSet<ProjectionTicket>>();
            mockSet.As<IQueryable<ProjectionTicket>>().Setup(m => m.Provider).Returns(tickets.Provider);
            mockSet.As<IQueryable<ProjectionTicket>>().Setup(m => m.Expression).Returns(tickets.Expression);
            mockSet.As<IQueryable<ProjectionTicket>>().Setup(m => m.ElementType).Returns(tickets.ElementType);
            mockSet.As<IQueryable<ProjectionTicket>>().Setup(m => m.GetEnumerator()).Returns(tickets.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.ProjectionTickets).Returns(mockSet.Object);
            mockContext.Setup(c => c.ProjectionTickets.FindAsync(It.IsAny<string>())).
                        Returns(new ValueTask<ProjectionTicket>((tickets.First())));

            var projectionTicketBusiness = new ProjectionTicketBusiness(mockContext.Object);

            // Act
            var resultTickets = projectionTicketBusiness.GetForProjectionAndUser(tickets.First().Projection.Id,
                                                                                 tickets.First().HolderId);

            // Assert 
            Assert.AreEqual(2, resultTickets.Count(), "Does't return all tickets with entered holder ID");
        }

        [Test]
        public void GetForUserReturnsAllTicketHoldedByTheEnteredUser()
        {
            // Arrange
            var tickets = new List<ProjectionTicket>
            {
                new ProjectionTicket
                {
                    Holder=new CinemaUser(),
                    Price=5,
                    HolderId="User1",
                    Projection=new FilmProjection
                    {
                        Cinema=new Cinema
                        {
                            Name="Cinema1"
                        },
                        Film =new Film
                        {
                            FilmData= new FilmData
                            {
                                Title="Film1"
                            }
                        }
                    },
                    ReservedOn=DateTime.Now, Seat=1
                },
                new ProjectionTicket
                {
                    Holder=new CinemaUser(),
                    Price=8,
                    HolderId="User1",
                    Projection=new FilmProjection
                    {
                        Cinema=new Cinema
                        {
                            Name="Cinema2"
                        },
                        Film =new Film
                        {
                            FilmData= new FilmData
                            {
                                Title="Film2"
                            }
                        }
                    },
                    ReservedOn=DateTime.Now, Seat=2
                },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<ProjectionTicket>>();
            mockSet.As<IQueryable<ProjectionTicket>>().Setup(m => m.Provider).Returns(tickets.Provider);
            mockSet.As<IQueryable<ProjectionTicket>>().Setup(m => m.Expression).Returns(tickets.Expression);
            mockSet.As<IQueryable<ProjectionTicket>>().Setup(m => m.ElementType).Returns(tickets.ElementType);
            mockSet.As<IQueryable<ProjectionTicket>>().Setup(m => m.GetEnumerator()).Returns(tickets.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.ProjectionTickets).Returns(mockSet.Object);
            mockContext.Setup(c => c.ProjectionTickets.FindAsync(It.IsAny<string>())).
                        Returns(new ValueTask<ProjectionTicket>((tickets.First())));

            var projectionTicketBusiness = new ProjectionTicketBusiness(mockContext.Object);
            var mapper = new Mapper();
            // Act
            var resultTickets = projectionTicketBusiness.GetForUser(tickets.First().HolderId,
                                                                    mapper.MapToTicketCardViewModel);

            // Assert 
            Assert.AreEqual(tickets.First().Price, resultTickets.First().Price, "Does't return all tickets with entered holder ID");
        }

        [Test]
        public async Task DeleteRemovesSelectedElement()
        {
            // Arrange
            var tickets = new List<ProjectionTicket>
            {
                new ProjectionTicket { },
                new ProjectionTicket{ },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<ProjectionTicket>>();
            mockSet.As<IQueryable<ProjectionTicket>>().Setup(m => m.Provider).Returns(tickets.Provider);
            mockSet.As<IQueryable<ProjectionTicket>>().Setup(m => m.Expression).Returns(tickets.Expression);
            mockSet.As<IQueryable<ProjectionTicket>>().Setup(m => m.ElementType).Returns(tickets.ElementType);
            mockSet.As<IQueryable<ProjectionTicket>>().Setup(m => m.GetEnumerator()).Returns(tickets.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.ProjectionTickets).Returns(mockSet.Object);
            mockContext.Setup(c => c.ProjectionTickets.FindAsync(It.IsAny<string>())).
                        Returns(new ValueTask<ProjectionTicket>((tickets.First())));
            mockContext.Setup(m => m.Remove(It.IsAny<ProjectionTicket>())).
                        Returns((EntityEntry<ProjectionTicket>)null);

            var projectionTicketBusiness = new ProjectionTicketBusiness(mockContext.Object);

            // Act
            await projectionTicketBusiness.DeleteAsync(tickets.First().Id);

            // Assert 
            mockSet.Verify(m => m.Remove(It.IsAny<ProjectionTicket>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task DeleteRemovesNothing()
        {
            // Arrange
            var tickets = new List<ProjectionTicket>
            {
                new ProjectionTicket { },
                new ProjectionTicket{ },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<ProjectionTicket>>();
            mockSet.As<IQueryable<ProjectionTicket>>().Setup(m => m.Provider).Returns(tickets.Provider);
            mockSet.As<IQueryable<ProjectionTicket>>().Setup(m => m.Expression).Returns(tickets.Expression);
            mockSet.As<IQueryable<ProjectionTicket>>().Setup(m => m.ElementType).Returns(tickets.ElementType);
            mockSet.As<IQueryable<ProjectionTicket>>().Setup(m => m.GetEnumerator()).Returns(tickets.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.ProjectionTickets).Returns(mockSet.Object);
            mockContext.Setup(c => c.ProjectionTickets.FindAsync(It.IsAny<string>())).
                        Returns(new ValueTask<ProjectionTicket>((ProjectionTicket)null));
            mockContext.Setup(m => m.Remove(It.IsAny<ProjectionTicket>())).
                        Returns((EntityEntry<ProjectionTicket>)null);

            var projectionTicketBusiness = new ProjectionTicketBusiness(mockContext.Object);

            // Act
            await projectionTicketBusiness.DeleteAsync(tickets.First().Id);

            // Assert 
            mockSet.Verify(m => m.Remove(It.IsAny<ProjectionTicket>()), Times.Never());
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }
    }
}
