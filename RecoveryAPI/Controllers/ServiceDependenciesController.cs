using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecoveryAPI.Dtos;
using RecoveryAPI.Managers;

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
    public async Task<ActionResult<IEnumerable<ServiceDependencyDto>>> GetServiceDependency()
    {
        return await _dependencyManager.GetDependenciesAsync();
    }

    // GET: api/ServiceDependency/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceDependencyDto>> GetServiceDependency(int id)
    {
        var dependency = await _dependencyManager.GetDependencyAsync(id);

        if (dependency == null)
        {
            return NotFound();
        }

        return dependency;
    }

    // PUT: api/ServiceDependency/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> PutServiceDependency(int id, ServiceDependencyDto input)
    {
        var result = await _dependencyManager.UpdateDependencyAsync(id, input);

        if (!result.Found)
        {
            return NotFound();
        }

        if (result.Error != null)
        {
            if (result.Conflict)
            {
                return Conflict(new ErrorDto { Message = result.Error });
            }

            return BadRequest(new ErrorDto { Message = result.Error });
        }

        return NoContent();
    }

    // POST: api/ServiceDependency
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ServiceDependencyDto>> PostServiceDependency(ServiceDependencyDto input)
    {
        var result = await _dependencyManager.AddDependencyAsync(input);

        if (result.Error != null)
        {
            if (result.Conflict)
            {
                return Conflict(new ErrorDto { Message = result.Error });
            }

            return BadRequest(new ErrorDto { Message = result.Error });
        }

        return CreatedAtAction(nameof(GetServiceDependency), new { id = result.Dependency!.Id }, result.Dependency);
    }

    // DELETE: api/ServiceDependency/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteServiceDependency(int id)
    {
        var result = await _dependencyManager.DeleteDependencyAsync(id);

        if (!result.Found)
        {
            return NotFound();
        }

        if (result.Error != null)
        {
            return Conflict(new ErrorDto { Message = result.Error });
        }

        return NoContent();
    }
}