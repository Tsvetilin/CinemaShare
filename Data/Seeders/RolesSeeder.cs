using Data.Enums;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Seeders
{
    public class RolesSeeder
    {
        public async Task SeedAsync(CinemaDbContext context)
        {
            if (context.Roles.Any())
            {
                return;
            }

            var roles = new List<CinemaRole>()
            {
                new CinemaRole{Name="Admin",NormalizedName="ADMIN", Role= RoleType.Admin},
                new CinemaRole{Name="Manager", NormalizedName= "MANAGER", Role=RoleType.Manager},
                new CinemaRole{Name="User",NormalizedName="USER", Role=RoleType.User},
            };

            foreach (var role in roles)
            {
                await context.Roles.AddAsync(role);
            }

            await context.SaveChangesAsync();
        }
    }
}
