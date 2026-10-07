using Microsoft.EntityFrameworkCore;
using RecoveryAPI.Models;

public class RecoveryAPIContext(
    DbContextOptions<RecoveryAPIContext> options) : DbContext(options)
{
    public DbSet<Service> Service { get; set; } = default!;
    public DbSet<ServiceDependency> ServiceDependency { get; set; } = default!;
    public DbSet<RecoveryAttempt> RecoveryAttempt { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ServiceDependency>()
            .HasOne(dependency => dependency.Service)
            .WithMany()
            .HasForeignKey(dependency => dependency.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ServiceDependency>()
            .HasOne(dependency => dependency.DependsOnService)
            .WithMany()
            .HasForeignKey(dependency => dependency.DependsOnServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RecoveryAttempt>()
            .HasOne<Service>()
            .WithMany()
            .HasForeignKey(attempt => attempt.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}