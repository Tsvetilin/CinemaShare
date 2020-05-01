using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class CinemaDbContext : IdentityDbContext<CinemaUser,CinemaRole,string>
    {
        public CinemaDbContext(DbContextOptions<CinemaDbContext> options):base(options)
        {
            this.Database.EnsureCreated();
            //this.Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CinemaUser>(entity => entity.Property(m => m.Id).HasMaxLength(85));
            builder.Entity<CinemaUser>(entity => entity.Property(m => m.NormalizedEmail).HasMaxLength(85));
            builder.Entity<CinemaUser>(entity => entity.Property(m => m.NormalizedUserName).HasMaxLength(85));

            builder.Entity<CinemaRole>(entity => entity.Property(m => m.NormalizedName).HasMaxLength(85));
            builder.Entity<CinemaRole>(entity => entity.Property(m => m.Id).HasMaxLength(85));

            builder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.LoginProvider).HasMaxLength(85));
            builder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.ProviderKey).HasMaxLength(85));
            builder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));
            builder.Entity<IdentityUserRole<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));

            builder.Entity<IdentityUserRole<string>>(entity => entity.Property(m => m.RoleId).HasMaxLength(85));

            builder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));
            builder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.LoginProvider).HasMaxLength(85));
            builder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.Name).HasMaxLength(85));

            builder.Entity<IdentityUserClaim<string>>(entity => entity.Property(m => m.Id).HasMaxLength(85));
            builder.Entity<IdentityUserClaim<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));
            builder.Entity<IdentityRoleClaim<string>>(entity => entity.Property(m => m.Id).HasMaxLength(85));
            builder.Entity<IdentityRoleClaim<string>>(entity => entity.Property(m => m.RoleId).HasMaxLength(85));
            
            base.OnModelCreating(builder);
        }

        public DbSet<Film> Films { get; set; }
        public DbSet<FilmData> FilmDatas { get; set; }
        public DbSet<FilmProjection> FilmProjections { get; set; }
        public DbSet<FilmReview> FilmReviews { get; set; }
        public DbSet<ProjectionTicket> ProjectionTickets { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
    }
}
