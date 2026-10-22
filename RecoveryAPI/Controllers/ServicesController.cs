using Microsoft.AspNetCore.Mvc;
using RecoveryAPI.Models;
using Microsoft.AspNetCore.Authorization;
using RecoveryAPI.Services;
using RecoveryAPI.Dtos;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ServicesController : ControllerBase
{
    private readonly ServiceManager _serviceManager;

    public ServicesController(ServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    // GET: api/Service
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceDto>>> GetService()
    {
        return await _serviceManager.GetServicesAsync();
    }

    // GET: api/Service/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceDto>> GetService(int id)
    {
        var service = await _serviceManager.GetServiceAsync(id);

        if (service == null)
        {
            return NotFound();
        }

        return service;
    }

    // PUT: api/Service/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> PutService(int id, Service service)
    {
        if (id != service.Id)
        {
            return BadRequest();
        }

        var updated = await _serviceManager.UpdateServiceAsync(service);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    // POST: api/Service
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ServiceDto>> PostService(Service service)
    {
        var createdService = await _serviceManager.AddServiceAsync(service);

        return CreatedAtAction(nameof(GetService), new { id = createdService.Id }, createdService);
    }

    // DELETE: api/Service/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteService(int id)
    {
        var result = await _serviceManager.DeleteServiceAsync(id);

        if (!result.Found)
        {
            return NotFound();
        }

        if (!result.Deleted)
        {
            return Conflict("Service has related dependencies or recovery attempts.");
        }

        return NoContent();
    }

    [HttpPut("{id}/fail")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ServiceDto>> MakeServiceFailed(int id)
    {
        var service = await _serviceManager.MakeServiceFailedAsync(id);

        if (service == null)
        {
            return NotFound();
        }

        return service;
    }

    [HttpPut("{id}/recovering")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ServiceDto>> MakeServiceRecovering(int id)
    {
        var result = await _serviceManager.StartRecoveryAsync(id);

        if (result.Service == null)
        {
            return NotFound();
        }

        if (!result.Started)
        {
            return Conflict("Recovery can only start for a failed service.");
        }

        return result.Service;
    }

    [HttpPut("{id}/recovered")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ServiceDto>> MakeServiceRecovered(int id)
    {
        var result = await _serviceManager.CompleteRecoveryAsync(id);

        if (result.Service == null)
        {
            return NotFound();
        }

        if (!result.Recovered)
        {
            return Conflict("Service is not recovering.");
        }

        return result.Service;
    }
}