using Microsoft.EntityFrameworkCore;

public class RecoveryAPIContext(DbContextOptions<RecoveryAPIContext> options) : DbContext(options)
{
    public DbSet<RecoveryAPI.Models.Service> Service { get; set; } = default!;

    public DbSet<RecoveryAPI.Models.ServiceDependency> ServiceDependency { get; set; } = default!;

    public DbSet<RecoveryAPI.Models.RecoveryAttempt> RecoveryAttempt { get; set; } = default!;
}