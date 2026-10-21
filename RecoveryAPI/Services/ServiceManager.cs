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
    }
}