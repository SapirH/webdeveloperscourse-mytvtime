﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
        }

        public DbSet<User> User { get; set; }

        public DbSet<Movie> Movie { get; set; }

        public DbSet<Genre> Genre { get; set; }

        public DbSet<MovieGenres> MovieGenres { get; set; }
    }
}
