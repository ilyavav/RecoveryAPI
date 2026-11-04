using Microsoft.EntityFrameworkCore;
using RecoveryAPI.Models;

public class RecoveryAPIContext(DbContextOptions<RecoveryAPIContext> options) : DbContext(options)
{
    public DbSet<Service> Service { get; set; } = default!;
    public DbSet<ServiceDependency> ServiceDependency { get; set; } = default!;
    public DbSet<RecoverySession> RecoverySession { get; set; } = default!;
    public DbSet<RecoverySessionService> RecoverySessionService { get; set; } = default!;
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

        modelBuilder.Entity<ServiceDependency>()
            .HasIndex(dependency => new { dependency.ServiceId, dependency.DependsOnServiceId })
            .IsUnique();

        modelBuilder.Entity<RecoverySessionService>()
            .HasOne(participant => participant.RecoverySession)
            .WithMany()
            .HasForeignKey(participant => participant.RecoverySessionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RecoverySessionService>()
            .HasOne(participant => participant.Service)
            .WithMany()
            .HasForeignKey(participant => participant.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RecoverySessionService>()
            .HasIndex(participant => new { participant.RecoverySessionId, participant.ServiceId })
            .IsUnique();

        modelBuilder.Entity<RecoveryAttempt>()
            .HasOne(attempt => attempt.RecoverySessionService)
            .WithMany()
            .HasForeignKey(attempt => attempt.RecoverySessionServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}