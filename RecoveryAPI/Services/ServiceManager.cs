using Microsoft.EntityFrameworkCore;
using RecoveryAPI.Models;

namespace RecoveryAPI.Services
{
    public class ServiceManager
    {
        private readonly RecoveryAPIContext _context;

        public ServiceManager(RecoveryAPIContext context)
        {
            _context = context;
        }

        public async Task<List<Service>> GetServicesAsync()
        {
            return await _context.Service.ToListAsync();
        }

        public async Task<Service?> GetServiceAsync(int id)
        {
            return await _context.Service.FindAsync(id);
        }

        public async Task<Service> AddServiceAsync(Service service)
        {
            _context.Service.Add(service);
            await _context.SaveChangesAsync();

            return service;
        }

        public async Task<bool> UpdateServiceAsync(Service service)
        {
            var existingService = await _context.Service.FindAsync(service.Id);

            if (existingService == null)
            {
                return false;
            }

            existingService.Name = service.Name;
            existingService.Description = service.Description;
            existingService.Criticality = service.Criticality;
            existingService.EstimatedRecoverySeconds = service.EstimatedRecoverySeconds;
            existingService.Status = service.Status;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<(bool Found, bool Deleted)> DeleteServiceAsync(int id)
        {
            var service = await _context.Service.FindAsync(id);

            if (service == null)
            {
                return (false, false);
            }

            var hasDependencies = await _context.ServiceDependency.AnyAsync(dependency => dependency.ServiceId == id|| dependency.DependsOnServiceId == id);

            var hasAttempts = await _context.RecoveryAttempt.AnyAsync(attempt => attempt.ServiceId == id);

            if (hasDependencies || hasAttempts)
            {
                return (true, false);
            }

            _context.Service.Remove(service);
            await _context.SaveChangesAsync();

            return (true, true);
        }

        public async Task<Service?> MakeServiceFailedAsync(int id)
        {
            var service = await _context.Service.FindAsync(id);

            if (service == null)
            {
                return null;
            }

            service.MakeFailed();
            await _context.SaveChangesAsync();

            return service;
        }

        public async Task<(Service? Service, bool Started)> StartRecoveryAsync(int id)
        {
            var service = await _context.Service.FindAsync(id);

            if (service == null)
            {
                return (null, false);
            }

            if (!service.TryMakeRecovering())
            {
                return (service, false);
            }

            await _context.SaveChangesAsync();

            return (service, true);
        }

        public async Task<(Service? Service, bool Recovered)> CompleteRecoveryAsync(int id)
        {
            var service = await _context.Service.FindAsync(id);

            if (service == null)
            {
                return (null, false);
            }

            if (!service.TryMakeRecovered())
            {
                return (service, false);
            }

            await _context.SaveChangesAsync();

            return (service, true);
        }
    }
}