using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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

        var utcConverter = new ValueConverter<DateTime, DateTime>(value => value, value => DateTime.SpecifyKind(value, DateTimeKind.Utc));

        modelBuilder.Entity<Service>().Property(service => service.ObservedAt).HasConversion(utcConverter);
        modelBuilder.Entity<RecoverySession>().Property(session => session.CreatedAt).HasConversion(utcConverter);
        modelBuilder.Entity<RecoverySession>().Property(session => session.StartedAt).HasConversion(utcConverter);
        modelBuilder.Entity<RecoverySession>().Property(session => session.FinishedAt).HasConversion(utcConverter);
        modelBuilder.Entity<RecoverySessionService>().Property(participant => participant.FinalObservedAt).HasConversion(utcConverter);
        modelBuilder.Entity<RecoveryAttempt>().Property(attempt => attempt.StartedAt).HasConversion(utcConverter);
        modelBuilder.Entity<RecoveryAttempt>().Property(attempt => attempt.FinishedAt).HasConversion(utcConverter);

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