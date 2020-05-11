using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Seeders
{
    public class CinemaSeeder
    {
        public async Task SeedAsync(CinemaDbContext context)
        {
            if (context.Cinemas.Any())
            {
                return;
            }

            var cinemas = new List<Cinema>()
            {
                new Cinema{
                City = "Sliven",
                Country = "Bulgaria",
                Name = "Best cinema",
                ManagerId =  context.Users.FirstOrDefault(x=>x.UserName=="Admin").Id
                }
            };

            var randomNumber = new Random();

            foreach (var cinema in cinemas)
            {
                await context.Cinemas.AddAsync(cinema);
            }

            await context.SaveChangesAsync();
        }
    }
}
