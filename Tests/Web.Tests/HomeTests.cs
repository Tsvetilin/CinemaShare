using Business;
using CinemaShare.Common.Mapping;
using CinemaShare.Controllers;
using CinemaShare.Models.ViewModels;
using Data;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests.Web.Tests
{
    class HomeTests
    {
        [Test]
        public void IndexReturnsIndexPage()
        {
            // Arrange
            var mapper = new Mapper();
            var films = new List<FilmData>
            {
                new FilmData
                {
                    ReleaseDate=DateTime.UtcNow,
                    Film=new Film
                    {
                        Rating=2,
                    }
                },
                new FilmData 
                {
                    ReleaseDate=DateTime.UtcNow.AddDays(1),
                    Film=new Film
                    {
                        Rating=1,
                    }
                }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<FilmData>>();
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.Provider).Returns(films.Provider);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.Expression).Returns(films.Expression);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.ElementType).Returns(films.ElementType);
            mockSet.As<IQueryable<FilmData>>().Setup(m => m.GetEnumerator()).Returns(films.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.FilmDatas).Returns(mockSet.Object);
            mockContext.Setup(c => c.FilmDatas.FindAsync(It.IsAny<string>())).
                        Returns(new ValueTask<FilmData>(films.First()));

            var controller = new HomeController(new FilmDataBusiness(mockContext.Object), new Mapper());

            // Act
            var result = controller.Index();
            var parsedResult = result as ViewResult;

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsAssignableFrom<HomePageViewModel>(parsedResult.Model);
        }

        [Test]
        public void PrivacyReturnsPrivacyPage()
        {
            // Arrange
            var mockContext = new Mock<CinemaDbContext>();

            var controller = new HomeController(new FilmDataBusiness(mockContext.Object), new Mapper());

            // Act
            var result = controller.Privacy();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public void StatusErrorReturnsErrorPage()
        {
            // Arrange
            var mockContext = new Mock<CinemaDbContext>();

            var controller = new HomeController(new FilmDataBusiness(mockContext.Object), new Mapper());

            // Act
            var result = controller.StatusError(404);
            var parsedResult = result as ViewResult;

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(404, (int)parsedResult.Model);
        }
    }
}
