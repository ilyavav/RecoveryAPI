using Microsoft.EntityFrameworkCore;
using RecoveryAPI.Dtos;
using RecoveryAPI.Models;

namespace RecoveryAPI.Services
{
    public class ServiceDependencyManager
    {
        private readonly RecoveryAPIContext _context;

        public ServiceDependencyManager(RecoveryAPIContext context)
        {
            _context = context;
        }

        public async Task<List<ServiceDependencyDto>> GetDependenciesAsync()
        {
            var dependencies = await _context.ServiceDependency.AsNoTracking().ToListAsync();

            return dependencies.Select(dependency => ToDto(dependency)).ToList();
        }

        public async Task<ServiceDependencyDto?> GetDependencyAsync(int id)
        {
            var dependency = await _context.ServiceDependency.AsNoTracking().FirstOrDefaultAsync(dependency => dependency.Id == id);

            if (dependency == null)
            {
                return null;
            }

            return ToDto(dependency);
        }

        public async Task<(ServiceDependencyDto? Dependency, bool Conflict, string? Error)> AddDependencyAsync(ServiceDependencyDto input)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var dependency = new ServiceDependency
            {
                ServiceId = input.ServiceId,
                DependsOnServiceId = input.DependsOnServiceId
            };

            var validation = await GetValidationErrorAsync(dependency);

            if (validation.Error != null)
            {
                return (null, validation.Conflict, validation.Error);
            }

            _context.ServiceDependency.Add(dependency);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return (ToDto(dependency), false, null);
        }

        public async Task<(bool Found, bool Conflict, string? Error)> UpdateDependencyAsync(int id, ServiceDependencyDto input)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var existingDependency = await _context.ServiceDependency.FindAsync(id);

            if (existingDependency == null)
            {
                return (false, false, null);
            }

            var dependency = new ServiceDependency
            {
                Id = id,
                ServiceId = input.ServiceId,
                DependsOnServiceId = input.DependsOnServiceId
            };

            var validation = await GetValidationErrorAsync(dependency);

            if (validation.Error != null)
            {
                return (true, validation.Conflict, validation.Error);
            }

            existingDependency.ServiceId = dependency.ServiceId;
            existingDependency.DependsOnServiceId = dependency.DependsOnServiceId;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, false, null);
        }

        public async Task<(bool Found, string? Error)> DeleteDependencyAsync(int id)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var dependency = await _context.ServiceDependency.FindAsync(id);

            if (dependency == null)
            {
                return (false, null);
            }

            var hasRunningSession = await _context.RecoverySession.AnyAsync(session => session.Status == RecoverySessionStatus.Running);

            if (hasRunningSession)
            {
                return (true, "Dependencies cannot be changed during recovery.");
            }

            _context.ServiceDependency.Remove(dependency);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, null);
        }

        private async Task<(bool Conflict, string? Error)> GetValidationErrorAsync(ServiceDependency dependency)
        {
            var hasRunningSession = await _context.RecoverySession.AnyAsync(session => session.Status == RecoverySessionStatus.Running);

            if (hasRunningSession)
            {
                return (true, "Dependencies cannot be changed during recovery.");
            }

            if (!dependency.Validate())
            {
                return (false, "A service cannot depend on itself.");
            }

            var serviceExists = await _context.Service.AnyAsync(service => service.Id == dependency.ServiceId);

            if (!serviceExists)
            {
                return (false, "Dependent service was not found.");
            }

            var prerequisiteExists = await _context.Service.AnyAsync(service => service.Id == dependency.DependsOnServiceId);

            if (!prerequisiteExists)
            {
                return (false, "Prerequisite service was not found.");
            }

            var alreadyExists = await _context.ServiceDependency.AnyAsync(other => other.ServiceId == dependency.ServiceId && other.DependsOnServiceId == dependency.DependsOnServiceId && other.Id != dependency.Id);

            if (alreadyExists)
            {
                return (true, "This dependency already exists.");
            }

            return (false, null);
        }

        private static ServiceDependencyDto ToDto(ServiceDependency dependency)
        {
            return new ServiceDependencyDto
            {
                Id = dependency.Id,
                ServiceId = dependency.ServiceId,
                DependsOnServiceId = dependency.DependsOnServiceId
            };
        }
    }
}