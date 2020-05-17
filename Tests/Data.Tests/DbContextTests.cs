using CinemaShare.Models.InputModels;
using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Data.Tests
{
    class DbContextTests
    {
        public DbSet<FilmInputModel> dbSet { get; set; }

        [Test]
        public void FindDbSetReturnRightValue()
        {
            // Arange
            var mockContext = new Mock<CinemaDbContext>();
            
            mockContext.Setup(x => x.Find(It.IsAny<Type>())).Returns(dbSet);

            // Act
            var resultSet = (DbSet<FilmInputModel>)mockContext.Object.Find(typeof(DbSet<FilmInputModel>));

            // Assert
            mockContext.Verify(x => x.Find(It.IsAny<Type>()), Times.Once);
            Assert.AreEqual(dbSet, resultSet);
        }
    }
}
