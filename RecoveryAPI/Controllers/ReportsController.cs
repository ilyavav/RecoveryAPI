using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyModel;
using Microsoft.AspNetCore.Authorization;

namespace RecoveryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly RecoveryAPIContext _context;
        public ReportsController(RecoveryAPIContext context)
        {
            _context = context;
        }

        [HttpGet("dependencies")]
        public async Task<IActionResult> GetDependenciesReport()
        {
            var report = await _context.ServiceDependency.Select(dependency => new
                {
                    Id = dependency.Id,
                    ServiceId = dependency.ServiceId,
                    ServiceName = dependency.Service!.Name,
                    DependsOnServiceId = dependency.DependsOnServiceId,
                    DependsOnServiceName = dependency.DependsOnService!.Name
                }).ToListAsync();

            return Ok(report);
        }

        [HttpGet("recoveryHistory/{serviceId}")]
        public async Task<IActionResult> GetRecoveryAttemptsReport(int serviceId)
        {
            var report = await _context.RecoveryAttempt.Where(attempt => attempt.ServiceId == serviceId).Select(attempt => new
            {
                Id = attempt.Id,
                ServiceId = attempt.ServiceId,
                ServiceName = attempt.Service!.Name,
                StartedAt = attempt.StartedAt,
                FinishedAt = attempt.FinishedAt,
                IsSuccessful = attempt.IsSuccessful
            }).ToListAsync();

            return Ok(report);
        }

        [HttpGet("recoveryStatistics")]
        public async Task<IActionResult> GetRecoveryStatistics()
        {
            var report = await _context.Service.Select(service => new
            {
                ServiceId = service.Id,
                ServiceName = service.Name,
                AttemptCount = _context.RecoveryAttempt.Count(attempt => attempt.ServiceId == service.Id),
                SuccessfulCount = _context.RecoveryAttempt.Count(attempt => attempt.ServiceId == service.Id && attempt.IsSuccessful == true),
                FailedCount = _context.RecoveryAttempt.Count(attempt => attempt.ServiceId == service.Id && attempt.IsSuccessful == false)
            }).ToListAsync();

            return Ok(report);
        }

    }
}
