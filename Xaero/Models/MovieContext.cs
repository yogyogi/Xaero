using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Xaero.Models
{
    public class MovieContext : DbContext
    {
        public MovieContext(DbContextOptions<MovieContext> options) : base(options)
        {
        }

        public DbSet<ProductionCompany> ProductionCompany { get; set; }
        public DbSet<Movie> Movie { get; set; }
        public DbSet<MovieDetail> MovieDetail { get; set; }
        public DbSet<Distribution> Distribution { get; set; }
        public DbSet<MovieDistribution> MovieDistribution { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ProductionCompany
            modelBuilder.Entity<ProductionCompany>().HasKey(s => s.Id);
            modelBuilder.Entity<ProductionCompany>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Logo)
                    .IsRequired();

                entity.Property(e => e.AnnualRevenue)
                    .IsRequired()
                    .HasColumnType("Money");

                entity.Property(e => e.EstablishmentDate)
                    .IsRequired()
                    .HasColumnType("Date");
            });

            // Movie
            modelBuilder.Entity<Movie>().HasKey(s => s.Id);
            modelBuilder.Entity<Movie>()
                    .HasOne(e => e.ProductionCompany_R)
                    .WithMany(e => e.Movie_R)
                    .HasForeignKey(e => e.ProductionCompanyId)
                    .OnDelete(DeleteBehavior.Cascade);

            // MovieDetail
            modelBuilder.Entity<MovieDetail>().HasKey(s => s.MovieId);
            modelBuilder.Entity<MovieDetail>()
                    .HasOne(e => e.Movie_R)
                    .WithOne(e => e.MovieDetail_R)
                    .HasForeignKey<MovieDetail>(e => e.MovieId)
                    .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<MovieDetail>(entity =>
            {
                entity.Property(e => e.MovieId)
                    .IsRequired();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Poster)
                    .IsRequired();

                entity.Property(e => e.Budget)
                    .IsRequired()
                    .HasColumnType("Money"); ;

                entity.Property(e => e.Gross)
                    .IsRequired();

                entity.Property(e => e.ReleaseDate)
                    .IsRequired()
                    .HasColumnType("Date");
            });

            // Distribution
            modelBuilder.Entity<Distribution>().HasKey(s => s.Id);
            modelBuilder.Entity<Distribution>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Location)
                    .IsRequired();

                entity.Property(e => e.Telephone)
                    .IsRequired();
            });

            // MovieDistribution
            modelBuilder.Entity<MovieDistribution>().HasKey(t => new { t.MovieId, t.DistributionId });
            modelBuilder.Entity<MovieDistribution>()
                        .HasOne(t => t.Movie_R)
                        .WithMany(t => t.MovieDistribution_R)
                        .HasForeignKey(t => t.MovieId);
            modelBuilder.Entity<MovieDistribution>()
                        .HasOne(t => t.Distribution_R)
                        .WithMany(t => t.MovieDistribution_R)
                        .HasForeignKey(t => t.DistributionId);
        }
    }
}
