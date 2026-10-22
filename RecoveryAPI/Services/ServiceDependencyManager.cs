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
            var dependencies = await _context.ServiceDependency.ToListAsync();

            return dependencies.Select(dependency => ToDto(dependency)).ToList();
        }

        public async Task<ServiceDependencyDto?> GetDependencyAsync(int id)
        {
            var dependency = await _context.ServiceDependency.FindAsync(id);

            if (dependency == null)
            {
                return null;
            }

            return ToDto(dependency);
        }

        public async Task<(ServiceDependencyDto Dependency, string? Error)> AddDependencyAsync(ServiceDependency dependency)
        {
            var error = await GetValidationErrorAsync(dependency);

            if (error != null)
            {
                return (ToDto(dependency), error);
            }

            _context.ServiceDependency.Add(dependency);
            await _context.SaveChangesAsync();

            return (ToDto(dependency), null);
        }

        public async Task<(bool Found, string? Error)> UpdateDependencyAsync(ServiceDependency dependency)
        {
            var existingDependency = await _context.ServiceDependency.FindAsync(dependency.Id);

            if (existingDependency == null)
            {
                return (false, null);
            }

            var error = await GetValidationErrorAsync(dependency);

            if (error != null)
            {
                return (true, error);
            }

            existingDependency.ServiceId = dependency.ServiceId;
            existingDependency.DependsOnServiceId = dependency.DependsOnServiceId;

            await _context.SaveChangesAsync();

            return (true, null);
        }

        public async Task<bool> DeleteDependencyAsync(int id)
        {
            var dependency = await _context.ServiceDependency.FindAsync(id);

            if (dependency == null)
            {
                return false;
            }

            _context.ServiceDependency.Remove(dependency);
            await _context.SaveChangesAsync();

            return true;
        }

        private async Task<string?> GetValidationErrorAsync(ServiceDependency dependency)
        {
            if (!dependency.Validate())
            {
                return "A service cannot depend on itself.";
            }

            var serviceExists = await _context.Service.AnyAsync(service => service.Id == dependency.ServiceId);

            if (!serviceExists)
            {
                return "Dependent service was not found.";
            }

            var prerequisiteExists = await _context.Service.AnyAsync(service => service.Id == dependency.DependsOnServiceId);

            if (!prerequisiteExists)
            {
                return "Prerequisite service was not found.";
            }

            return null;
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