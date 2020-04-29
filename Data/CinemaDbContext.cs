using Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Data
{
    public class CinemaDbContext : IdentityDbContext<CinemaUser,CinemaRole,Guid>
    {
        public CinemaDbContext(DbContextOptions<CinemaDbContext> options):base(options)
        {
            this.Database.EnsureCreated();
            this.Database.Migrate();
        }

    }
}
