using Data.Seeders;
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
            await new RolesSeeder().SeedAsync(context);
            await new UsersSeeder().SeedAsync(context);
            await new FilmsSeeder().SeedAsync(context);
            await new CinemasSeeder().SeedAsync(context);
            await new ProjectionsSeeder().SeedAsync(context);
        }
    }
}
