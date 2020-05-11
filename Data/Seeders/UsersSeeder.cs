using Data.Enums;
using Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Seeders
{
    public class UsersSeeder
    {
        public async Task SeedAsync(CinemaDbContext context)
        {
            if (context.Users.Any())
            {
                return;
            }

            var email = "cinemashare222@gmail.com";
            var username = "Admin";

            var adminUser = new CinemaUser
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
            };
            var password = new PasswordHasher<CinemaUser>().HashPassword(adminUser, "Abcdef1.");
            adminUser.PasswordHash = password;

            await context.UserRoles.AddAsync(new IdentityUserRole<string>()
            {
                UserId = adminUser.Id,
                RoleId = context.Roles.FirstOrDefault(x=>x.Name=="Admin").Id
            });

            await context.Users.AddAsync(adminUser);

            await context.SaveChangesAsync();
        }
    }
}
