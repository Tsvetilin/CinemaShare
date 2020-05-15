using Data.Enums;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Seeders
{
    public class ProjectionsSeeder
    {
        public async Task SeedAsync(CinemaDbContext context)
        {
            if (context.FilmProjections.Any())
            {
                return;
            }

            var projection = new FilmProjection
            {
                CinemaId = context.Cinemas.First().Id,
                Date = DateTime.UtcNow.AddDays(10),
                FilmId = context.Films.First().Id,
                ProjectionType = ProjectionType._3D,
                TotalTickets = 100,
                TicketPrices = new TicketPrices
                {
                    AdultPrice = 15,
                    StudentPrice = 10,
                    ChildrenPrice = 5,
                },
            };

            await context.FilmProjections.AddAsync(projection);
            await context.SaveChangesAsync();
        }
    }
}
