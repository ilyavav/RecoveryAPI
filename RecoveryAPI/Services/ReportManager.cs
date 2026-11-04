using Microsoft.EntityFrameworkCore;
using RecoveryAPI.Dtos;
using RecoveryAPI.Models;

namespace RecoveryAPI.Services
{
    public class ReportManager
    {
        private readonly RecoveryAPIContext _context;

        public ReportManager(RecoveryAPIContext context)
        {
            _context = context;
        }

        public async Task<List<DependencyReportDto>> GetDependenciesAsync()
        {
            return await _context.ServiceDependency.AsNoTracking().Select(dependency => new DependencyReportDto
            {
                Id = dependency.Id,
                ServiceId = dependency.ServiceId,
                ServiceName = dependency.Service!.Name,
                DependsOnServiceId = dependency.DependsOnServiceId,
                DependsOnServiceName = dependency.DependsOnService!.Name
            }).ToListAsync();
        }

        public async Task<List<RecoveryHistoryDto>> GetRecoveryHistoryAsync(int serviceId)
        {
            return await _context.RecoveryAttempt.AsNoTracking().Where(attempt => attempt.RecoverySessionService!.ServiceId == serviceId).OrderBy(attempt => attempt.Id).Select(attempt => new RecoveryHistoryDto
            {
                Id = attempt.Id,
                RecoverySessionId = attempt.RecoverySessionService!.RecoverySessionId,
                ServiceId = attempt.RecoverySessionService.ServiceId,
                ServiceName = attempt.RecoverySessionService.Service!.Name,
                Status = attempt.Status,
                StartedAt = attempt.StartedAt,
                FinishedAt = attempt.FinishedAt,
                ErrorMessage = attempt.ErrorMessage
            }).ToListAsync();
        }

        public async Task<List<RecoveryStatisticsDto>> GetRecoveryStatisticsAsync()
        {
            return await _context.Service.AsNoTracking().OrderBy(service => service.Id).Select(service => new RecoveryStatisticsDto
            {
                ServiceId = service.Id,
                ServiceName = service.Name,
                AttemptCount = _context.RecoveryAttempt.Count(attempt => attempt.RecoverySessionService!.ServiceId == service.Id),
                SuccessfulCount = _context.RecoveryAttempt.Count(attempt => attempt.RecoverySessionService!.ServiceId == service.Id && attempt.Status == RecoveryAttemptStatus.Succeeded),
                FailedCount = _context.RecoveryAttempt.Count(attempt => attempt.RecoverySessionService!.ServiceId == service.Id && attempt.Status == RecoveryAttemptStatus.Failed)
            }).ToListAsync();
        }
    }
}