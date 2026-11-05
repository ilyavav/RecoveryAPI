using Microsoft.EntityFrameworkCore;
using RecoveryAPI.Dtos;
using RecoveryAPI.Models;

namespace RecoveryAPI.Managers
{
    public class ServiceManager
    {
        private readonly RecoveryAPIContext _context;

        public ServiceManager(RecoveryAPIContext context)
        {
            _context = context;
        }

        public async Task<List<ServiceDto>> GetServicesAsync()
        {
            var services = await _context.Service.AsNoTracking().ToListAsync();

            return services.Select(service => ToDto(service)).ToList();
        }

        public async Task<ServiceDto?> GetServiceAsync(int id)
        {
            var service = await _context.Service.AsNoTracking().FirstOrDefaultAsync(service => service.Id == id);

            if (service == null)
            {
                return null;
            }

            return ToDto(service);
        }

        public async Task<ServiceDto> AddServiceAsync(ServiceInputDto input)
        {
            var service = new Service
            {
                Name = input.Name.Trim(),
                Description = input.Description?.Trim(),
                DefaultPriority = input.DefaultPriority,
                DesiredReplicas = input.DesiredReplicas
            };

            _context.Service.Add(service);
            await _context.SaveChangesAsync();

            return ToDto(service);
        }

        public async Task<(bool Found, bool Conflict, string? Error)> UpdateServiceAsync(int id, ServiceInputDto input)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var service = await _context.Service.FindAsync(id);

            if (service == null)
            {
                return (false, false, null);
            }

            if (service.DesiredReplicas != input.DesiredReplicas)
            {
                var hasRunningSession = await _context.RecoverySessionService.AnyAsync(participant => participant.ServiceId == id && participant.RecoverySession!.Status == RecoverySessionStatus.Running);

                if (hasRunningSession)
                {
                    return (true, true, "Desired replicas cannot be changed during recovery.");
                }
            }

            if (!service.TrySetPriority(input.DefaultPriority) || !service.TrySetDesiredReplicas(input.DesiredReplicas))
            {
                return (true, false, "Invalid service settings.");
            }

            service.Name = input.Name.Trim();
            service.Description = input.Description?.Trim();

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, false, null);
        }

        public async Task<(bool Found, bool Deleted)> DeleteServiceAsync(int id)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var service = await _context.Service.FindAsync(id);

            if (service == null)
            {
                return (false, false);
            }

            var hasDependencies = await _context.ServiceDependency.AnyAsync(dependency => dependency.ServiceId == id || dependency.DependsOnServiceId == id);
            var hasParticipants = await _context.RecoverySessionService.AnyAsync(participant => participant.ServiceId == id);

            if (hasDependencies || hasParticipants)
            {
                return (true, false);
            }

            _context.Service.Remove(service);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, true);
        }

        public async Task<(ServiceDto? Service, string? Error)> UpdateObservationAsync(int id, ServiceObservationDto observation)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var service = await _context.Service.FindAsync(id);

            if (service == null)
            {
                return (null, null);
            }

            if (observation.ObservedAt.Kind != DateTimeKind.Utc)
            {
                return (ToDto(service), "Observation time must be in UTC.");
            }

            if (!service.TryUpdateObservation(observation.ReadyReplicas, observation.ObservedAt))
            {
                return (ToDto(service), "Observation is invalid, outdated or conflicts with an existing observation.");
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return (ToDto(service), null);
        }

        private static ServiceDto ToDto(Service service)
        {
            return new ServiceDto
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                DefaultPriority = service.DefaultPriority,
                DesiredReplicas = service.DesiredReplicas,
                ReadyReplicas = service.ReadyReplicas,
                ObservedAt = service.ObservedAt
            };
        }
    }
}