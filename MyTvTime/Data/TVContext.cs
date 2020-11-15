using Microsoft.EntityFrameworkCore;
using MyTvTime.Models;

namespace MyTvTime.Data
{
    public class TVContext : DbContext
    {
        public TVContext (DbContextOptions<TVContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MovieGenres>().HasKey(k => new { k.MovieID, k.GenreID });
            modelBuilder.Entity<UserMovie>().HasKey(um => new { um.UserId, um.MovieId });
        }

        public DbSet<User> User { get; set; }

        public DbSet<Movie> Movie { get; set; }

        public DbSet<Genre> Genre { get; set; }

        public DbSet<MovieGenres> MovieGenres { get; set; }

        public DbSet<Comment> Comment { get; set; }

        public DbSet<UserMovie> UserMovie { get; set; }
    }
}
