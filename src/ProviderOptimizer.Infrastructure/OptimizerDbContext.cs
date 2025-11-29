using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ProviderOptimizer.Domain.Entities;
using System.Text.Json;

namespace ProviderOptimizer.Infrastructure;

public class OptimizerDbContext : DbContext
{
    public OptimizerDbContext(DbContextOptions<OptimizerDbContext> options)
        : base(options) { }

    public DbSet<Provider> Providers => Set<Provider>();
    public DbSet<Optimization> Optimizations => Set<Optimization>();

   protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Provider>(e =>
        {
        e.ToTable("providers");

        e.HasKey(p => p.Id);

        e.Property(p => p.Id)
            .HasColumnName("id");

        e.Property(p => p.Name)
            .HasColumnName("name")
            .IsRequired();

        e.Property(p => p.Latitude)
            .HasColumnName("latitude");

        e.Property(p => p.Longitude)
            .HasColumnName("longitude");

        e.Property(p => p.IsAvailable)
            .HasColumnName("isavailable");

        e.Property(p => p.Rating)
            .HasColumnName("rating");

        e.Property(p => p.Services)
            .HasColumnName("services")
            .HasConversion(
                v => v == null ? "" : string.Join(",", v),
                v => string.IsNullOrWhiteSpace(v)
                    ? new List<string>()
                    : v.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList()
            );
    });

        b.Entity<Optimization>(e =>
        {
            e.ToTable("optimizations");

            e.HasKey(o => o.Id);

            e.Property(o => o.Id)
                .HasColumnName("id");

            e.Property(o => o.ProviderId)
                .HasColumnName("providerid");

            e.Property(o => o.RequestName)
                .HasColumnName("requestname");

            e.Property(o => o.Latitude)
                .HasColumnName("latitude");

            e.Property(o => o.Longitude)
                .HasColumnName("longitude");

            e.Property(o => o.Rating)
                .HasColumnName("rating");

            e.HasOne(o => o.Provider)
                .WithMany()
                .HasForeignKey(o => o.ProviderId);
        });
    }
}