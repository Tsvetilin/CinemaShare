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

        public CinemaDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //Rewrite the default max length of the fields to be suitable for MySql database
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

            //Map user to films
            builder.Entity<CinemaUser>().HasMany(user => user.AddedFilms).WithOne();
            builder.Entity<CinemaUser>().HasMany(user => user.WatchList).WithOne();
            builder.Entity<Film>().HasOne(film => film.AddedByUser);
            //builder.Entity<FilmData>().HasOne(filmData => filmData.Film).WithOne();

            base.OnModelCreating(builder);
        }

        public virtual DbSet<Film> Films { get; set; }
        public virtual DbSet<FilmData> FilmDatas { get; set; }
        public virtual DbSet<FilmProjection> FilmProjections { get; set; }
        public virtual DbSet<FilmReview> FilmReviews { get; set; }
        public virtual DbSet<ProjectionTicket> ProjectionTickets { get; set; }
        public virtual DbSet<Cinema> Cinemas { get; set; }
    }
}
