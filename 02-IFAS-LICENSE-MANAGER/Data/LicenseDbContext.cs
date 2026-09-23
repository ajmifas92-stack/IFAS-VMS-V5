using IFAS.LicenseManager.Models;
using Microsoft.EntityFrameworkCore;

namespace IFAS.LicenseManager.Data;

public class LicenseDbContext : DbContext
{
    public LicenseDbContext(DbContextOptions<LicenseDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<License> Licenses => Set<License>();
    public DbSet<LicenseFeature> LicenseFeatures => Set<LicenseFeature>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.CustomerId).IsUnique();
            entity.Property(x => x.CustomerId).IsRequired().HasMaxLength(100);
            entity.Property(x => x.CompanyName).IsRequired().HasMaxLength(250);
            entity.Property(x => x.Email).HasMaxLength(250);
        });

        modelBuilder.Entity<License>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.LicenseId).IsUnique();
            entity.HasIndex(x => x.LicenseKey).IsUnique();
            entity.Property(x => x.LicenseId).IsRequired().HasMaxLength(100);
            entity.Property(x => x.LicenseKey).IsRequired().HasMaxLength(200);

            entity.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LicenseFeature>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.FeatureCode).IsUnique();
            entity.Property(x => x.FeatureCode).IsRequired().HasMaxLength(100);
            entity.Property(x => x.FeatureName).IsRequired().HasMaxLength(200);
        });
    }
}
