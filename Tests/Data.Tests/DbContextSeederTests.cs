using Data;
using Data.Enums;
using Data.Models;
using Data.Seeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tests.Data.Tests
{
    class DbContextSeederTests
    {

        [Test]
        public async Task SeederExecutes()
        {
            // Arrange
            var films = new List<Film> {new Film() }.AsQueryable();
            var cinemas = new List<Cinema> {new Cinema() }.AsQueryable();
            var projections = new List<FilmProjection> { new FilmProjection()}.AsQueryable();
            var users = new List<CinemaUser> { new CinemaUser()}.AsQueryable();
            var roles = new List<CinemaRole> { new CinemaRole() }.AsQueryable();

            var filmsMockSet = new Mock<DbSet<Film>>();
            filmsMockSet.As<IQueryable<Film>>().Setup(m => m.Provider).Returns(films.Provider);
            filmsMockSet.As<IQueryable<Film>>().Setup(m => m.Expression).Returns(films.Expression);
            filmsMockSet.As<IQueryable<Film>>().Setup(m => m.ElementType).Returns(films.ElementType);
            filmsMockSet.As<IQueryable<Film>>().Setup(m => m.GetEnumerator()).Returns(films.GetEnumerator());

            var cinemasMockSet = new Mock<DbSet<Cinema>>();
            cinemasMockSet.As<IQueryable<Cinema>>().Setup(m => m.Provider).Returns(cinemas.Provider);
            cinemasMockSet.As<IQueryable<Cinema>>().Setup(m => m.Expression).Returns(cinemas.Expression);
            cinemasMockSet.As<IQueryable<Cinema>>().Setup(m => m.ElementType).Returns(cinemas.ElementType);
            cinemasMockSet.As<IQueryable<Cinema>>().Setup(m => m.GetEnumerator()).Returns(cinemas.GetEnumerator());

            var projectionsMockSet = new Mock<DbSet<FilmProjection>>();
            projectionsMockSet.As<IQueryable<FilmProjection>>().Setup(m => m.Provider).Returns(projections.Provider);
            projectionsMockSet.As<IQueryable<FilmProjection>>().Setup(m => m.Expression).Returns(projections.Expression);
            projectionsMockSet.As<IQueryable<FilmProjection>>().Setup(m => m.ElementType).Returns(projections.ElementType);
            projectionsMockSet.As<IQueryable<FilmProjection>>().Setup(m => m.GetEnumerator()).Returns(projections.GetEnumerator());

            var usersMockSet = new Mock<DbSet<CinemaUser>>();
            usersMockSet.As<IQueryable<CinemaUser>>().Setup(m => m.Provider).Returns(users.Provider);
            usersMockSet.As<IQueryable<CinemaUser>>().Setup(m => m.Expression).Returns(users.Expression);
            usersMockSet.As<IQueryable<CinemaUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            usersMockSet.As<IQueryable<CinemaUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var rolesMockSet = new Mock<DbSet<CinemaRole>>();
            rolesMockSet.As<IQueryable<CinemaRole>>().Setup(m => m.Provider).Returns(roles.Provider);
            rolesMockSet.As<IQueryable<CinemaRole>>().Setup(m => m.Expression).Returns(roles.Expression);
            rolesMockSet.As<IQueryable<CinemaRole>>().Setup(m => m.ElementType).Returns(roles.ElementType);
            rolesMockSet.As<IQueryable<CinemaRole>>().Setup(m => m.GetEnumerator()).Returns(roles.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Films).Returns(filmsMockSet.Object);
            mockContext.Setup(c => c.Cinemas).Returns(cinemasMockSet.Object);
            mockContext.Setup(c => c.FilmProjections).Returns(projectionsMockSet.Object);
            mockContext.Setup(c => c.Users).Returns(usersMockSet.Object);
            mockContext.Setup(c => c.Roles).Returns(rolesMockSet.Object);

            var seeder = new CinemaDbContextSeeder(mockContext.Object);

            // Act
            await seeder.SeedAsync();

            // Assert
            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task RolesSeederExecutes()
        {
            // Arrange
            var roles = new List<CinemaRole>().AsQueryable();

            var rolesMockSet = new Mock<DbSet<CinemaRole>>();
            rolesMockSet.As<IQueryable<CinemaRole>>().Setup(m => m.Provider).Returns(roles.Provider);
            rolesMockSet.As<IQueryable<CinemaRole>>().Setup(m => m.Expression).Returns(roles.Expression);
            rolesMockSet.As<IQueryable<CinemaRole>>().Setup(m => m.ElementType).Returns(roles.ElementType);
            rolesMockSet.As<IQueryable<CinemaRole>>().Setup(m => m.GetEnumerator()).Returns(roles.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Roles).Returns(rolesMockSet.Object);
            mockContext.Setup(m => m.AddAsync(It.IsAny<CinemaRole>(), It.IsAny<CancellationToken>())).Returns(new ValueTask<EntityEntry<CinemaRole>>(Task.FromResult((EntityEntry<CinemaRole>)null)));

            var seeder = new RolesSeeder();

            // Act
            await seeder.SeedAsync(mockContext.Object);

            // Assert
            mockContext.Verify(x => x.Roles.AddAsync(It.IsAny<CinemaRole>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task RolesSeederDoesntAffectFilledDb()
        {
            // Arrange
            var roles = new List<CinemaRole> { new CinemaRole() }.AsQueryable();

            var rolesMockSet = new Mock<DbSet<CinemaRole>>();
            rolesMockSet.As<IQueryable<CinemaRole>>().Setup(m => m.Provider).Returns(roles.Provider);
            rolesMockSet.As<IQueryable<CinemaRole>>().Setup(m => m.Expression).Returns(roles.Expression);
            rolesMockSet.As<IQueryable<CinemaRole>>().Setup(m => m.ElementType).Returns(roles.ElementType);
            rolesMockSet.As<IQueryable<CinemaRole>>().Setup(m => m.GetEnumerator()).Returns(roles.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Roles).Returns(rolesMockSet.Object);
            mockContext.Setup(m => m.AddAsync(It.IsAny<CinemaRole>(), It.IsAny<CancellationToken>())).Returns(new ValueTask<EntityEntry<CinemaRole>>(Task.FromResult((EntityEntry<CinemaRole>)null)));

            var seeder = new RolesSeeder();

            // Act
            await seeder.SeedAsync(mockContext.Object);

            // Assert
            mockContext.Verify(x => x.Films.AddAsync(It.IsAny<Film>(), It.IsAny<CancellationToken>()), Times.Never);
            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task UsersSeederExecutes()
        {
            // Arrange
            var roles = new List<CinemaRole> { new CinemaRole { Name = "Admin", NormalizedName = "ADMIN", Role = RoleType.Admin } }.AsQueryable();
            var users = new List<CinemaUser>().AsQueryable();
            var usersRoles = new List<IdentityUserRole<string>>().AsQueryable();

            var rolesMockSet = new Mock<DbSet<CinemaRole>>();
            rolesMockSet.As<IQueryable<CinemaRole>>().Setup(m => m.Provider).Returns(roles.Provider);
            rolesMockSet.As<IQueryable<CinemaRole>>().Setup(m => m.Expression).Returns(roles.Expression);
            rolesMockSet.As<IQueryable<CinemaRole>>().Setup(m => m.ElementType).Returns(roles.ElementType);
            rolesMockSet.As<IQueryable<CinemaRole>>().Setup(m => m.GetEnumerator()).Returns(roles.GetEnumerator());

            var usersMockSet = new Mock<DbSet<CinemaUser>>();
            usersMockSet.As<IQueryable<CinemaUser>>().Setup(m => m.Provider).Returns(users.Provider);
            usersMockSet.As<IQueryable<CinemaUser>>().Setup(m => m.Expression).Returns(users.Expression);
            usersMockSet.As<IQueryable<CinemaUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            usersMockSet.As<IQueryable<CinemaUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var usersRolesMockSet = new Mock<DbSet<IdentityUserRole<string>>>();
            usersRolesMockSet.As<IQueryable<IdentityUserRole<string>>>().Setup(m => m.Provider).Returns(usersRoles.Provider);
            usersRolesMockSet.As<IQueryable<IdentityUserRole<string>>>().Setup(m => m.Expression).Returns(usersRoles.Expression);
            usersRolesMockSet.As<IQueryable<IdentityUserRole<string>>>().Setup(m => m.ElementType).Returns(usersRoles.ElementType);
            usersRolesMockSet.As<IQueryable<IdentityUserRole<string>>>().Setup(m => m.GetEnumerator()).Returns(usersRoles.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Roles).Returns(rolesMockSet.Object);
            mockContext.Setup(c => c.Users).Returns(usersMockSet.Object);
            mockContext.Setup(c => c.UserRoles).Returns(usersRolesMockSet.Object);

            mockContext.Setup(m => m.AddAsync(It.IsAny<CinemaUser>(), It.IsAny<CancellationToken>())).Returns(new ValueTask<EntityEntry<CinemaUser>>(Task.FromResult((EntityEntry<CinemaUser>)null)));
            mockContext.Setup(m => m.AddAsync(It.IsAny<CinemaRole>(), It.IsAny<CancellationToken>())).Returns(new ValueTask<EntityEntry<CinemaRole>>(Task.FromResult((EntityEntry<CinemaRole>)null)));
            mockContext.Setup(m => m.AddAsync(It.IsAny<IdentityUserRole<string>>(), It.IsAny<CancellationToken>())).Returns(new ValueTask<EntityEntry<IdentityUserRole<string>>>(Task.FromResult((EntityEntry<IdentityUserRole<string>>)null)));

            var seeder = new UsersSeeder();

            // Act
            await seeder.SeedAsync(mockContext.Object);

            // Assert
            mockContext.Verify(x => x.Users.AddAsync(It.IsAny<CinemaUser>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task UsersSeederDoesntAffectFilledDb()
        {
            // Arrange
            var roles = new List<CinemaRole> { new CinemaRole { Name = "Admin", NormalizedName = "ADMIN", Role = RoleType.Admin } }.AsQueryable();
            var users = new List<CinemaUser> {new CinemaUser() }.AsQueryable();
            var usersRoles = new List<IdentityUserRole<string>>().AsQueryable();

            var rolesMockSet = new Mock<DbSet<CinemaRole>>();
            rolesMockSet.As<IQueryable<CinemaRole>>().Setup(m => m.Provider).Returns(roles.Provider);
            rolesMockSet.As<IQueryable<CinemaRole>>().Setup(m => m.Expression).Returns(roles.Expression);
            rolesMockSet.As<IQueryable<CinemaRole>>().Setup(m => m.ElementType).Returns(roles.ElementType);
            rolesMockSet.As<IQueryable<CinemaRole>>().Setup(m => m.GetEnumerator()).Returns(roles.GetEnumerator());

            var usersMockSet = new Mock<DbSet<CinemaUser>>();
            usersMockSet.As<IQueryable<CinemaUser>>().Setup(m => m.Provider).Returns(users.Provider);
            usersMockSet.As<IQueryable<CinemaUser>>().Setup(m => m.Expression).Returns(users.Expression);
            usersMockSet.As<IQueryable<CinemaUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            usersMockSet.As<IQueryable<CinemaUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var usersRolesMockSet = new Mock<DbSet<IdentityUserRole<string>>>();
            usersRolesMockSet.As<IQueryable<IdentityUserRole<string>>>().Setup(m => m.Provider).Returns(usersRoles.Provider);
            usersRolesMockSet.As<IQueryable<IdentityUserRole<string>>>().Setup(m => m.Expression).Returns(usersRoles.Expression);
            usersRolesMockSet.As<IQueryable<IdentityUserRole<string>>>().Setup(m => m.ElementType).Returns(usersRoles.ElementType);
            usersRolesMockSet.As<IQueryable<IdentityUserRole<string>>>().Setup(m => m.GetEnumerator()).Returns(usersRoles.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Roles).Returns(rolesMockSet.Object);
            mockContext.Setup(c => c.Users).Returns(usersMockSet.Object);
            mockContext.Setup(c => c.UserRoles).Returns(usersRolesMockSet.Object);

            mockContext.Setup(m => m.AddAsync(It.IsAny<CinemaUser>(), It.IsAny<CancellationToken>())).Returns(new ValueTask<EntityEntry<CinemaUser>>(Task.FromResult((EntityEntry<CinemaUser>)null)));
            mockContext.Setup(m => m.AddAsync(It.IsAny<CinemaRole>(), It.IsAny<CancellationToken>())).Returns(new ValueTask<EntityEntry<CinemaRole>>(Task.FromResult((EntityEntry<CinemaRole>)null)));
            mockContext.Setup(m => m.AddAsync(It.IsAny<IdentityUserRole<string>>(), It.IsAny<CancellationToken>())).Returns(new ValueTask<EntityEntry<IdentityUserRole<string>>>(Task.FromResult((EntityEntry<IdentityUserRole<string>>)null)));

            var seeder = new UsersSeeder();

            // Act
            await seeder.SeedAsync(mockContext.Object);

            // Assert
            mockContext.Verify(x => x.Users.AddAsync(It.IsAny<CinemaUser>(), It.IsAny<CancellationToken>()), Times.Never);
            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task FilmsSeederExecutes()
        {
            // Arrange
            var films = new List<Film>().AsQueryable();
            var filmDatas = new List<FilmData>().AsQueryable();
            var users = new List<CinemaUser>() { new CinemaUser { } }.AsQueryable();

            var filmsMockSet = new Mock<DbSet<Film>>();
            filmsMockSet.As<IQueryable<Film>>().Setup(m => m.Provider).Returns(films.Provider);
            filmsMockSet.As<IQueryable<Film>>().Setup(m => m.Expression).Returns(films.Expression);
            filmsMockSet.As<IQueryable<Film>>().Setup(m => m.ElementType).Returns(films.ElementType);
            filmsMockSet.As<IQueryable<Film>>().Setup(m => m.GetEnumerator()).Returns(films.GetEnumerator());

            var filmDatasMockSet = new Mock<DbSet<FilmData>>();
            filmDatasMockSet.As<IQueryable<FilmData>>().Setup(m => m.Provider).Returns(filmDatas.Provider);
            filmDatasMockSet.As<IQueryable<FilmData>>().Setup(m => m.Expression).Returns(filmDatas.Expression);
            filmDatasMockSet.As<IQueryable<FilmData>>().Setup(m => m.ElementType).Returns(filmDatas.ElementType);
            filmDatasMockSet.As<IQueryable<FilmData>>().Setup(m => m.GetEnumerator()).Returns(filmDatas.GetEnumerator());

            var usersMockSet = new Mock<DbSet<CinemaUser>>();
            usersMockSet.As<IQueryable<CinemaUser>>().Setup(m => m.Provider).Returns(users.Provider);
            usersMockSet.As<IQueryable<CinemaUser>>().Setup(m => m.Expression).Returns(users.Expression);
            usersMockSet.As<IQueryable<CinemaUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            usersMockSet.As<IQueryable<CinemaUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Films).Returns(filmsMockSet.Object);
            mockContext.Setup(c => c.Users).Returns(usersMockSet.Object);
            mockContext.Setup(c => c.FilmDatas).Returns(filmDatasMockSet.Object);


            mockContext.Setup(m => m.AddAsync(It.IsAny<Film>(), It.IsAny<CancellationToken>())).Returns(new ValueTask<EntityEntry<Film>>(Task.FromResult((EntityEntry<Film>)null)));
            mockContext.Setup(m => m.AddAsync(It.IsAny<FilmData>(), It.IsAny<CancellationToken>())).Returns(new ValueTask<EntityEntry<FilmData>>(Task.FromResult((EntityEntry<FilmData>)null)));
            mockContext.Setup(m => m.AddAsync(It.IsAny<CinemaUser>(), It.IsAny<CancellationToken>())).Returns(new ValueTask<EntityEntry<CinemaUser>>(Task.FromResult((EntityEntry<CinemaUser>)null)));
          
            var seeder = new FilmsSeeder();

            // Act
            await seeder.SeedAsync(mockContext.Object);

            // Assert
            mockContext.Verify(x => x.Films.AddAsync(It.IsAny<Film>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
            mockContext.Verify(x => x.FilmDatas.AddAsync(It.IsAny<FilmData>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task FilmsSeederDoesntAffectFilledDb()
        {
            // Arrange
            var films = new List<Film> {new Film() }.AsQueryable();
            var filmDatas = new List<FilmData>().AsQueryable();
            var users = new List<CinemaUser>() { new CinemaUser() }.AsQueryable();

            var filmsMockSet = new Mock<DbSet<Film>>();
            filmsMockSet.As<IQueryable<Film>>().Setup(m => m.Provider).Returns(films.Provider);
            filmsMockSet.As<IQueryable<Film>>().Setup(m => m.Expression).Returns(films.Expression);
            filmsMockSet.As<IQueryable<Film>>().Setup(m => m.ElementType).Returns(films.ElementType);
            filmsMockSet.As<IQueryable<Film>>().Setup(m => m.GetEnumerator()).Returns(films.GetEnumerator());

            var filmDatasMockSet = new Mock<DbSet<FilmData>>();
            filmDatasMockSet.As<IQueryable<FilmData>>().Setup(m => m.Provider).Returns(filmDatas.Provider);
            filmDatasMockSet.As<IQueryable<FilmData>>().Setup(m => m.Expression).Returns(filmDatas.Expression);
            filmDatasMockSet.As<IQueryable<FilmData>>().Setup(m => m.ElementType).Returns(filmDatas.ElementType);
            filmDatasMockSet.As<IQueryable<FilmData>>().Setup(m => m.GetEnumerator()).Returns(filmDatas.GetEnumerator());

            var usersMockSet = new Mock<DbSet<CinemaUser>>();
            usersMockSet.As<IQueryable<CinemaUser>>().Setup(m => m.Provider).Returns(users.Provider);
            usersMockSet.As<IQueryable<CinemaUser>>().Setup(m => m.Expression).Returns(users.Expression);
            usersMockSet.As<IQueryable<CinemaUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            usersMockSet.As<IQueryable<CinemaUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Films).Returns(filmsMockSet.Object);
            mockContext.Setup(c => c.Users).Returns(usersMockSet.Object);
            mockContext.Setup(c => c.FilmDatas).Returns(filmDatasMockSet.Object);


            mockContext.Setup(m => m.AddAsync(It.IsAny<Film>(), It.IsAny<CancellationToken>())).Returns(new ValueTask<EntityEntry<Film>>(Task.FromResult((EntityEntry<Film>)null)));
            mockContext.Setup(m => m.AddAsync(It.IsAny<FilmData>(), It.IsAny<CancellationToken>())).Returns(new ValueTask<EntityEntry<FilmData>>(Task.FromResult((EntityEntry<FilmData>)null)));
            mockContext.Setup(m => m.AddAsync(It.IsAny<CinemaUser>(), It.IsAny<CancellationToken>())).Returns(new ValueTask<EntityEntry<CinemaUser>>(Task.FromResult((EntityEntry<CinemaUser>)null)));

            var seeder = new FilmsSeeder();

            // Act
            await seeder.SeedAsync(mockContext.Object);

            // Assert
            mockContext.Verify(x => x.Films.AddAsync(It.IsAny<Film>(), It.IsAny<CancellationToken>()), Times.Never);
            mockContext.Verify(x => x.FilmDatas.AddAsync(It.IsAny<FilmData>(), It.IsAny<CancellationToken>()), Times.Never);
            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task ProjectionsSeederExecutes()
        {
            // Arrange
            var films = new List<Film>{ new Film() }.AsQueryable();
            var cinemas = new List<Cinema> { new Cinema() }.AsQueryable();
            var projections = new List<FilmProjection>().AsQueryable();

            var filmsMockSet = new Mock<DbSet<Film>>();
            filmsMockSet.As<IQueryable<Film>>().Setup(m => m.Provider).Returns(films.Provider);
            filmsMockSet.As<IQueryable<Film>>().Setup(m => m.Expression).Returns(films.Expression);
            filmsMockSet.As<IQueryable<Film>>().Setup(m => m.ElementType).Returns(films.ElementType);
            filmsMockSet.As<IQueryable<Film>>().Setup(m => m.GetEnumerator()).Returns(films.GetEnumerator());

            var cinemasMockSet = new Mock<DbSet<Cinema>>();
            cinemasMockSet.As<IQueryable<Cinema>>().Setup(m => m.Provider).Returns(cinemas.Provider);
            cinemasMockSet.As<IQueryable<Cinema>>().Setup(m => m.Expression).Returns(cinemas.Expression);
            cinemasMockSet.As<IQueryable<Cinema>>().Setup(m => m.ElementType).Returns(cinemas.ElementType);
            cinemasMockSet.As<IQueryable<Cinema>>().Setup(m => m.GetEnumerator()).Returns(cinemas.GetEnumerator());

            var projectionsMockSet = new Mock<DbSet<FilmProjection>>();
            projectionsMockSet.As<IQueryable<FilmProjection>>().Setup(m => m.Provider).Returns(projections.Provider);
            projectionsMockSet.As<IQueryable<FilmProjection>>().Setup(m => m.Expression).Returns(projections.Expression);
            projectionsMockSet.As<IQueryable<FilmProjection>>().Setup(m => m.ElementType).Returns(projections.ElementType);
            projectionsMockSet.As<IQueryable<FilmProjection>>().Setup(m => m.GetEnumerator()).Returns(projections.GetEnumerator());


            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Films).Returns(filmsMockSet.Object);
            mockContext.Setup(c => c.Cinemas).Returns(cinemasMockSet.Object);
            mockContext.Setup(c => c.FilmProjections).Returns(projectionsMockSet.Object);

            mockContext.Setup(m => m.AddAsync(It.IsAny<FilmProjection>(), It.IsAny<CancellationToken>())).Returns(new ValueTask<EntityEntry<FilmProjection>>(Task.FromResult((EntityEntry<FilmProjection>)null)));
   
            var seeder = new ProjectionsSeeder();
            // Act
            await seeder.SeedAsync(mockContext.Object);

            // Assert
            mockContext.Verify(x => x.FilmProjections.AddAsync(It.IsAny<FilmProjection>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task ProjectionsSeederDoesntAffectFilledDb()
        {
            // Arrange
            var projections = new List<FilmProjection> { new FilmProjection()}.AsQueryable();

            var projectionsMockSet = new Mock<DbSet<FilmProjection>>();
            projectionsMockSet.As<IQueryable<FilmProjection>>().Setup(m => m.Provider).Returns(projections.Provider);
            projectionsMockSet.As<IQueryable<FilmProjection>>().Setup(m => m.Expression).Returns(projections.Expression);
            projectionsMockSet.As<IQueryable<FilmProjection>>().Setup(m => m.ElementType).Returns(projections.ElementType);
            projectionsMockSet.As<IQueryable<FilmProjection>>().Setup(m => m.GetEnumerator()).Returns(projections.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.FilmProjections).Returns(projectionsMockSet.Object);

            mockContext.Setup(m => m.AddAsync(It.IsAny<FilmProjection>(), It.IsAny<CancellationToken>())).Returns(new ValueTask<EntityEntry<FilmProjection>>(Task.FromResult((EntityEntry<FilmProjection>)null)));

            var seeder = new ProjectionsSeeder();
            // Act
            await seeder.SeedAsync(mockContext.Object);

            // Assert
            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            mockContext.Verify(x => x.AddAsync(It.IsAny<FilmProjection>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task CinemaSeederExecutes()
        {
            // Arrange
            var cinemas = new List<Cinema>().AsQueryable();
            var users = new List<CinemaUser>(){ new CinemaUser { UserName="Admin"} }.AsQueryable();

            var cinemasMockSet = new Mock<DbSet<Cinema>>();
            cinemasMockSet.As<IQueryable<Cinema>>().Setup(m => m.Provider).Returns(cinemas.Provider);
            cinemasMockSet.As<IQueryable<Cinema>>().Setup(m => m.Expression).Returns(cinemas.Expression);
            cinemasMockSet.As<IQueryable<Cinema>>().Setup(m => m.ElementType).Returns(cinemas.ElementType);
            cinemasMockSet.As<IQueryable<Cinema>>().Setup(m => m.GetEnumerator()).Returns(cinemas.GetEnumerator());

            var usersMockSet = new Mock<DbSet<CinemaUser>>();
            usersMockSet.As<IQueryable<CinemaUser>>().Setup(m => m.Provider).Returns(users.Provider);
            usersMockSet.As<IQueryable<CinemaUser>>().Setup(m => m.Expression).Returns(users.Expression);
            usersMockSet.As<IQueryable<CinemaUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            usersMockSet.As<IQueryable<CinemaUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Cinemas).Returns(cinemasMockSet.Object);
            mockContext.Setup(c => c.Users).Returns(usersMockSet.Object);

            mockContext.Setup(m => m.AddAsync(It.IsAny<Cinema>(), It.IsAny<CancellationToken>())).Returns(new ValueTask<EntityEntry<Cinema>>(Task.FromResult((EntityEntry<Cinema>)null)));

            var seeder = new CinemasSeeder();
            // Act
            await seeder.SeedAsync(mockContext.Object);

            // Assert
            mockContext.Verify(x => x.Cinemas.AddAsync(It.IsAny<Cinema>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Test]
        public async Task CinemaSeederDoesntAffectFilledDb()
        {
            // Arrange
            var cinemas = new List<Cinema> {new Cinema() }.AsQueryable();
            var users = new List<CinemaUser>() { new CinemaUser { UserName = "Admin" } }.AsQueryable();

            var cinemasMockSet = new Mock<DbSet<Cinema>>();
            cinemasMockSet.As<IQueryable<Cinema>>().Setup(m => m.Provider).Returns(cinemas.Provider);
            cinemasMockSet.As<IQueryable<Cinema>>().Setup(m => m.Expression).Returns(cinemas.Expression);
            cinemasMockSet.As<IQueryable<Cinema>>().Setup(m => m.ElementType).Returns(cinemas.ElementType);
            cinemasMockSet.As<IQueryable<Cinema>>().Setup(m => m.GetEnumerator()).Returns(cinemas.GetEnumerator());

            var usersMockSet = new Mock<DbSet<CinemaUser>>();
            usersMockSet.As<IQueryable<CinemaUser>>().Setup(m => m.Provider).Returns(users.Provider);
            usersMockSet.As<IQueryable<CinemaUser>>().Setup(m => m.Expression).Returns(users.Expression);
            usersMockSet.As<IQueryable<CinemaUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            usersMockSet.As<IQueryable<CinemaUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Cinemas).Returns(cinemasMockSet.Object);
            mockContext.Setup(c => c.Users).Returns(usersMockSet.Object);

            mockContext.Setup(m => m.AddAsync(It.IsAny<Cinema>(), It.IsAny<CancellationToken>())).Returns(new ValueTask<EntityEntry<Cinema>>(Task.FromResult((EntityEntry<Cinema>)null)));

            var seeder = new CinemasSeeder();
            // Act
            await seeder.SeedAsync(mockContext.Object);

            // Assert
            mockContext.Verify(x => x.Cinemas.AddAsync(It.IsAny<Cinema>(), It.IsAny<CancellationToken>()), Times.Never);
            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task FilmsSeederExecutesWithNoUser()
        {
            // Arrange
            var films = new List<Film>().AsQueryable();
            var filmDatas = new List<FilmData>().AsQueryable();
            var users = new List<CinemaUser>() { new CinemaUser { UserName="Admin"} }.AsQueryable();

            var filmsMockSet = new Mock<DbSet<Film>>();
            filmsMockSet.As<IQueryable<Film>>().Setup(m => m.Provider).Returns(films.Provider);
            filmsMockSet.As<IQueryable<Film>>().Setup(m => m.Expression).Returns(films.Expression);
            filmsMockSet.As<IQueryable<Film>>().Setup(m => m.ElementType).Returns(films.ElementType);
            filmsMockSet.As<IQueryable<Film>>().Setup(m => m.GetEnumerator()).Returns(films.GetEnumerator());

            var filmDatasMockSet = new Mock<DbSet<FilmData>>();
            filmDatasMockSet.As<IQueryable<FilmData>>().Setup(m => m.Provider).Returns(filmDatas.Provider);
            filmDatasMockSet.As<IQueryable<FilmData>>().Setup(m => m.Expression).Returns(filmDatas.Expression);
            filmDatasMockSet.As<IQueryable<FilmData>>().Setup(m => m.ElementType).Returns(filmDatas.ElementType);
            filmDatasMockSet.As<IQueryable<FilmData>>().Setup(m => m.GetEnumerator()).Returns(filmDatas.GetEnumerator());

            var usersMockSet = new Mock<DbSet<CinemaUser>>();
            usersMockSet.As<IQueryable<CinemaUser>>().Setup(m => m.Provider).Returns(users.Provider);
            usersMockSet.As<IQueryable<CinemaUser>>().Setup(m => m.Expression).Returns(users.Expression);
            usersMockSet.As<IQueryable<CinemaUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            usersMockSet.As<IQueryable<CinemaUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var mockContext = new Mock<CinemaDbContext>();
            mockContext.Setup(c => c.Films).Returns(filmsMockSet.Object);
            mockContext.Setup(c => c.Users).Returns(usersMockSet.Object);
            mockContext.Setup(c => c.FilmDatas).Returns(filmDatasMockSet.Object);


            mockContext.Setup(m => m.AddAsync(It.IsAny<Film>(), It.IsAny<CancellationToken>())).Returns(new ValueTask<EntityEntry<Film>>(Task.FromResult((EntityEntry<Film>)null)));
            mockContext.Setup(m => m.AddAsync(It.IsAny<FilmData>(), It.IsAny<CancellationToken>())).Returns(new ValueTask<EntityEntry<FilmData>>(Task.FromResult((EntityEntry<FilmData>)null)));
            mockContext.Setup(m => m.AddAsync(It.IsAny<CinemaUser>(), It.IsAny<CancellationToken>())).Returns(new ValueTask<EntityEntry<CinemaUser>>(Task.FromResult((EntityEntry<CinemaUser>)null)));

            var seeder = new FilmsSeeder();

            // Act
            await seeder.SeedAsync(mockContext.Object);

            // Assert
            mockContext.Verify(x => x.Films.AddAsync(It.IsAny<Film>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
            mockContext.Verify(x => x.FilmDatas.AddAsync(It.IsAny<FilmData>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
