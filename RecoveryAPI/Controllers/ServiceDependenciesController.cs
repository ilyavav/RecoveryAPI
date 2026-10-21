using Microsoft.AspNetCore.Mvc;
using RecoveryAPI.Models;
using Microsoft.AspNetCore.Authorization;
using RecoveryAPI.Services;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ServiceDependenciesController : ControllerBase
{
    private readonly ServiceDependencyManager _dependencyManager;

    public ServiceDependenciesController(ServiceDependencyManager dependencyManager)
    {
        _dependencyManager = dependencyManager;
    }

    // GET: api/ServiceDependency
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceDependency>>> GetServiceDependency()
    {
        return await _dependencyManager.GetDependenciesAsync();
    }

    // GET: api/ServiceDependency/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceDependency>> GetServiceDependency(int id)
    {
        var servicedependency = await _dependencyManager.GetDependencyAsync(id);

        if (servicedependency == null)
        {
            return NotFound();
        }

        return servicedependency;
    }

    // PUT: api/ServiceDependency/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> PutServiceDependency(int id, ServiceDependency servicedependency)
    {
        if (id != servicedependency.Id)
        {
            return BadRequest();
        }

        var result = await _dependencyManager.UpdateDependencyAsync(servicedependency);

        if (!result.Found)
        {
            return NotFound();
        }

        if (result.Error != null)
        {
            return BadRequest(result.Error);
        }

        return NoContent();
    }

    // POST: api/ServiceDependency
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ServiceDependency>> PostServiceDependency(ServiceDependency servicedependency)
    {
        var result = await _dependencyManager.AddDependencyAsync(servicedependency);

        if (result.Error != null)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(nameof(GetServiceDependency), new { id = result.Dependency.Id }, result.Dependency);
    }

    // DELETE: api/ServiceDependency/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteServiceDependency(int id)
    {
        var deleted = await _dependencyManager.DeleteDependencyAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}