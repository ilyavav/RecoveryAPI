using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RecoveryAPI.Services;

namespace RecoveryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly ReportManager _reportManager;

        public ReportsController(ReportManager reportManager)
        {
            _reportManager = reportManager;
        }

        [HttpGet("dependencies")]
        public async Task<IActionResult> GetDependenciesReport()
        {
            var report = await _reportManager.GetDependenciesAsync();

            return Ok(report);
        }

        [HttpGet("recoveryHistory/{serviceId}")]
        public async Task<IActionResult> GetRecoveryAttemptsReport(int serviceId)
        {
            var report = await _reportManager.GetRecoveryHistoryAsync(serviceId);

            return Ok(report);
        }

        [HttpGet("recoveryStatistics")]
        public async Task<IActionResult> GetRecoveryStatistics()
        {
            var report = await _reportManager.GetRecoveryStatisticsAsync();

            return Ok(report);
        }
    }
}