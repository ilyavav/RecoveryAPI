using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecoveryAPI.Models;
using Microsoft.AspNetCore.Authorization;
using RecoveryAPI.Services;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ServicesController : ControllerBase
{
    private readonly RecoveryAPIContext _context;
    private readonly ServiceManager _serviceManager;
    public ServicesController(RecoveryAPIContext context,ServiceManager serviceManager)
    {
        _context = context;
        _serviceManager = serviceManager;
    }

    // GET: api/Service
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Service>>> GetService()
    {
        return await _serviceManager.GetServicesAsync();
    }

    // GET: api/Service/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Service>> GetService(int id)
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
    public async Task<IActionResult> PutService(int? id, Service service)
    {
        if (id != service.Id)
        {
            return BadRequest();
        }

        _context.Entry(service).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ServiceExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Service
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<Service>> PostService(Service service)
    {
        _context.Service.Add(service);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetService", new { id = service.Id }, service);
    }

    // DELETE: api/Service/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteService(int? id)
    {
        var service = await _context.Service.FindAsync(id);
        if (service == null)
        {
            return NotFound();
        }

        _context.Service.Remove(service);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ServiceExists(int? id)
    {
        return _context.Service.Any(e => e.Id == id);
    }

    [HttpPut("{id}/fail")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<Service>> MakeServiceFailed(int id)
    {
        var service = await _context.Service.FindAsync(id);
        if (service == null)
        {
            return NotFound();
        }
        service.MakeFailed();
        await _context.SaveChangesAsync();
        return service;
    }

    [HttpPut("{id}/recovering")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<Service>> MakeServiceRecovering(int id)
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
    public async Task<ActionResult<Service>> MakeServiceRecovered(int id)
    {
        var service = await _context.Service.FindAsync(id);
        if (service == null)
        {
            return NotFound();
        }

        try
        {
            service.MakeRecovered();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }

        await _context.SaveChangesAsync();
        return service;
    }
}
