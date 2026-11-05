using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecoveryAPI.Dtos;
using RecoveryAPI.Managers;

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
    public async Task<ActionResult<IEnumerable<ServiceDto>>> GetServices()
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
    public async Task<IActionResult> PutService(int id, ServiceInputDto input)
    {
        var result = await _serviceManager.UpdateServiceAsync(id, input);

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

    // POST: api/Service
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ServiceDto>> PostService(ServiceInputDto input)
    {
        var service = await _serviceManager.AddServiceAsync(input);

        return CreatedAtAction(nameof(GetService), new { id = service.Id }, service);
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
            return Conflict(new ErrorDto { Message = "Service has related dependencies or recovery history." });
        }

        return NoContent();
    }

    [HttpPut("{id}/observation")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ServiceDto>> UpdateObservation(int id, ServiceObservationDto observation)
    {
        var result = await _serviceManager.UpdateObservationAsync(id, observation);

        if (result.Service == null)
        {
            return NotFound();
        }

        if (result.Error != null)
        {
            return BadRequest(new ErrorDto { Message = result.Error });
        }

        return result.Service;
    }
}