using Microsoft.EntityFrameworkCore;

namespace RecoveryAPI.Services
{
    public class ReportManager
    {
        private readonly RecoveryAPIContext _context;

        public ReportManager(RecoveryAPIContext context)
        {
            _context = context;
        }

        public async Task<object> GetDependenciesAsync()
        {
            return await _context.ServiceDependency.Select(dependency => new
            {
                Id = dependency.Id,
                ServiceId = dependency.ServiceId,
                ServiceName = dependency.Service!.Name,
                DependsOnServiceId = dependency.DependsOnServiceId,
                DependsOnServiceName = dependency.DependsOnService!.Name
            }).ToListAsync();
        }

        public async Task<object> GetRecoveryHistoryAsync(int serviceId)
        {
            return await _context.RecoveryAttempt.Where(attempt => attempt.ServiceId == serviceId).Select(attempt => new
            {
                Id = attempt.Id,
                ServiceId = attempt.ServiceId,
                ServiceName = attempt.Service!.Name,
                StartedAt = attempt.StartedAt,
                FinishedAt = attempt.FinishedAt,
                IsSuccessful = attempt.IsSuccessful
            }).ToListAsync();
        }

        public async Task<object> GetRecoveryStatisticsAsync()
        {
            return await _context.Service.Select(service => new
            {
                ServiceId = service.Id,
                ServiceName = service.Name,
                AttemptCount = _context.RecoveryAttempt.Count(attempt => attempt.ServiceId == service.Id),
                SuccessfulCount = _context.RecoveryAttempt.Count(attempt => attempt.ServiceId == service.Id && attempt.IsSuccessful == true),
                FailedCount = _context.RecoveryAttempt.Count(attempt => attempt.ServiceId == service.Id && attempt.IsSuccessful == false)
            }).ToListAsync();
        }
    }
}