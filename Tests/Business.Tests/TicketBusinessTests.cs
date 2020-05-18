using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Data;
using Data.Models;
using System.Linq;
using System.Threading.Tasks;
using CinemaShare.Common.Mapping;
using Business;

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
            mockContext.Setup(c => c.ProjectionTickets.FindAsync(It.IsAny<string>())).Returns(new ValueTask<ProjectionTicket>((tickets.First())));

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
            mockContext.Setup(c => c.ProjectionTickets.FindAsync(It.IsAny<string>())).Returns(new ValueTask<ProjectionTicket>((tickets.First())));

            var projectionTicketBusiness = new ProjectionTicketBusiness(mockContext.Object);
            var searchedTicket = tickets.First();

            // Act
            var resultTicketsCount = projectionTicketBusiness.GetAll().ToList().Count();

            // Assert
            Assert.AreEqual(2, resultTicketsCount, "Doesn't return all elements in the database.");
        }
    }
}
