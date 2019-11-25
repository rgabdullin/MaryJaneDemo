using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PiProject
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Measurement> Measurements { get; set; }
        public DbSet<Dataset> Datasets { get; set; }
        public DbSet<Label> Labels { get; set; }

        public DatabaseContext(DbContextOptions options) :
            base(options)
        {
            Debug.WriteLine("Creating context");
            Database.EnsureCreated();
            Debug.WriteLine("Context Created!!!");
        }
        protected override void OnModelCreating(ModelBuilder model)
        {
            Debug.WriteLine("OnModelCreating");
            model.Entity<SpectralData>().ToTable("V001_SpectralData");
            model.Entity<Measurement>().ToTable("V001_Measurement");
            model.Entity<Label>().ToTable("V001_Label");
            model.Entity<Dataset>().ToTable("V001_Dataset");

            Debug.WriteLine("Step 1");
            model.Entity<SpectralData>()
                .HasAlternateKey("MeasurementId", "LedId", "Channel", "Freq");
            model.Entity<Dataset>()
                .HasAlternateKey("Name");
            model.Entity<Label>()
                .HasAlternateKey("DatasetId", "Name");

            Debug.WriteLine("Step 2");
            model.Entity<SpectralData>()
                .HasOne(a => a.Measurement)
                .WithMany("Data")
                .HasForeignKey("MeasurementId");

            Debug.WriteLine("Step 3");
            model.Entity<Measurement>()
                .HasOne(a => a.Dataset)
                .WithMany("Measurements")
                .HasForeignKey("DatasetId")
                .OnDelete(DeleteBehavior.SetNull);

            Debug.WriteLine("Step 4");
            model.Entity<Measurement>()
                .HasOne(a => a.Label)
                .WithMany("Measurements")
                .HasForeignKey("LabelId");

            Debug.WriteLine("Step 5");
            model.Entity<Label>()
                .HasOne(a => a.Dataset)
                .WithMany("Labels")
                .HasForeignKey("DatasetId");

            Debug.WriteLine("Final");
        }
    }
}
