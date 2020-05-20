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
        }

        public CinemaDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Rewrite the default max length of the fields to be suitable for MySql database
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

            // Claim relationships
            builder.Entity<CinemaUser>().HasMany(user => user.AddedFilms).WithOne();
            builder.Entity<CinemaUser>().HasMany(user => user.WatchList).WithOne();
            builder.Entity<CinemaUser>().HasMany(user => user.FilmReviews).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<CinemaUser>().HasOne(user => user.Cinema).WithOne(x => x.Manager).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<CinemaUser>().HasMany(user => user.AddedFilms).WithOne().OnDelete(DeleteBehavior.SetNull);
            builder.Entity<Film>().HasOne(x => x.AddedByUser).WithMany(x => x.AddedFilms).OnDelete(DeleteBehavior.SetNull);
            builder.Entity<Film>().HasOne(film => film.AddedByUser);
            builder.Entity<Film>().HasMany(film => film.FilmProjection).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Film>().HasMany(film => film.Ratings).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Film>().HasOne(film => film.FilmData).WithOne(x => x.Film).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Film>().HasMany(film => film.FilmReviews).WithOne(x => x.Film).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Cinema>().HasMany(x => x.FilmProjections).WithOne(x => x.Cinema).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<FilmProjection>().HasMany(x => x.ProjectionTickets).WithOne(x => x.Projection).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<FilmReview>().HasOne(x => x.User).WithMany(x => x.FilmReviews).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<FilmRating>().HasOne(x => x.User).WithMany().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<FilmProjection>().HasOne(x => x.Film).WithMany(x => x.FilmProjection).HasForeignKey(x => x.FilmId);
            builder.Entity<ProjectionTicket>().HasOne(x => x.Holder).WithMany(x => x.ProjectionTickets).OnDelete(DeleteBehavior.Cascade);

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
