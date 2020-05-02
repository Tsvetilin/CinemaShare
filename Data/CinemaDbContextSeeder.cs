using Data.Seeders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class CinemaDbContextSeeder
    {
        private readonly CinemaDbContext context;

        public CinemaDbContextSeeder(CinemaDbContext context)
        {
            this.context = context;
        }

        public async Task SeedAsync()
        {
            await new FilmsSeeder().SeedAsync(context);
            await new RolesSeeder().SeedAsync(context);
        }
    }
}
