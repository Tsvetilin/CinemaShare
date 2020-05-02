using Business;
using Data;
using Data.Enums;
using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CinemaDbContext>();
            optionsBuilder.UseMySQL<CinemaDbContext>("server=localhost;port=3306;username=root;password=root;database=cinemaapp;SslMode = None");
            optionsBuilder.UseLazyLoadingProxies();
            CinemaDbContext dbContext = new CinemaDbContext(optionsBuilder.Options);


            //var user = new CinemaUser()
            //{
            //    FirstName = "Tsvetilin",
            //    LastName = "Tsvetilov",
            //    UserName = "Ceco",
            //    CreatedOn = DateTime.UtcNow,
            //    Email="cvetilov6@abv.bg",
            //};

            //var hasher = new PasswordHasher<CinemaUser>();
            //var password = hasher.HashPassword(user, "PasswordSecretToHashxD");
            //user.PasswordHash = password;

            ////IUserStore<CinemaUser> store = new CinemaStore();
            ////IOptions<IdentityOptions> options = new CinemaOptions();
            ////var userValidator = new[] { new UserValidator<CinemaUser>() };
            ////var passwordValidator = new[] { new PasswordValidator<CinemaUser>() };
            ////var manager = new UserManager<CinemaUser>(store, options, hasher, userValidator, passwordValidator, null, null, null, null);
            ////await manager.CreateAsync(user, password);

            //await dbContext.Users.AddAsync(user);
            //await dbContext.SaveChangesAsync();

            //var getUser = await dbContext.Users.FirstOrDefaultAsync(x => x.LastName == "Tsvetilov");
            //var name = getUser?.LastName;
            //Console.WriteLine(name);

            /* Film film = new Film
             {
                 Rating = 7
             };
             Console.WriteLine(film.Id);
             FilmData filmData = new FilmData
             {
                 FilmId = film.Id,
                 Title = "Test",
                 Description = "asddsa",
                 Genre = new[] { new GenreType { Genre = Genre.Action } },
             };

             FilmBusiness filmBusiness = new FilmBusiness(dbContext);
             FilmDataBusiness filmDataBusiness = new FilmDataBusiness(dbContext);
             await filmBusiness.AddAsync(film);
             await filmDataBusiness.Add(filmData);*/

            var data = await dbContext.FilmDatas.ToListAsync();
            var mapped = data.ToList();
            Console.WriteLine(mapped);
        }
    }

    //internal class CinemaOptions : IOptions<IdentityOptions>{ }
    //internal class CinemaStore : IUserPasswordStore<CinemaUser>{ }

}
