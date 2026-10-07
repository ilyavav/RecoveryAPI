using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyModel;

namespace RecoveryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

    }
}
