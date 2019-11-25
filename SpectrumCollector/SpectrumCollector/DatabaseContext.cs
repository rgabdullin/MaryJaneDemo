using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace SpectrumCollector
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Measurement> Measurements { get; set; }

        public DatabaseContext(DbContextOptions options) :
            base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder model)
        {
            model.Entity<SpectrumInfo>().ToTable("SpectrumCollector_SpectrumInfo");
            model.Entity<Measurement>().ToTable("SpectrumCollector_Measurement");
            model.Entity<ProcessingResult>().ToTable("SpectrumCollector_ProcessingResult");

            model.Entity<SpectrumInfo>()
                .HasAlternateKey("MeasurementId", "Channel");
            model.Entity<SpectrumInfo>()
                .HasOne<Measurement>("Measurement")
                .WithMany("Data")
                .HasForeignKey("MeasurementId");
            model.Entity<Measurement>()
                .HasOne<ProcessingResult>("Result")
                .WithOne("Measurement")
                .HasForeignKey<ProcessingResult>("MeasurementId");
        }
    }
}
