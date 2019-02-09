using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MarsRover.WebApi.Repository.EntityModels
{
    public partial class MarsRoverContext : DbContext
    {
        public MarsRoverContext()
        {
        }

        public MarsRoverContext(DbContextOptions<MarsRoverContext> options)
            : base(options)
        {
        }

        public virtual DbSet<PhotoDate> PhotoDate { get; set; }
        public virtual DbSet<Rover> Rover { get; set; }
        public virtual DbSet<RoverPhoto> RoverPhoto { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlite("Data Source=mars-rover.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.1-servicing-10028");

            modelBuilder.Entity<PhotoDate>(entity =>
            {
                entity.Property(e => e.PhotoDateId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Rover>(entity =>
            {
                entity.Property(e => e.RoverId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<RoverPhoto>(entity =>
            {
                entity.Property(e => e.RoverPhotoId).ValueGeneratedOnAdd();

                entity.HasOne(d => d.PhotoDate)
                    .WithMany(p => p.RoverPhoto)
                    .HasForeignKey(d => d.PhotoDateId);

                entity.HasOne(d => d.Rover)
                    .WithMany(p => p.RoverPhoto)
                    .HasForeignKey(d => d.RoverId);
            });
        }
    }
}
