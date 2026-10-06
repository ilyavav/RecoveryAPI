using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecoveryAPI.Models;

[Route("api/[controller]")]
[ApiController]
public class ServiceDependenciesController : ControllerBase
{
    private readonly RecoveryAPIContext _context;
    public ServiceDependenciesController(RecoveryAPIContext context)
    {
        _context = context;
    }

    // GET: api/ServiceDependency
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceDependency>>> GetServiceDependency()
    {
        return await _context.ServiceDependency.ToListAsync();
    }

    // GET: api/ServiceDependency/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceDependency>> GetServiceDependency(int id)
    {
        var servicedependency = await _context.ServiceDependency.FindAsync(id);

        if (servicedependency == null)
        {
            return NotFound();
        }

        return servicedependency;
    }

    // PUT: api/ServiceDependency/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutServiceDependency(int? id, ServiceDependency servicedependency)
    {
        if (id != servicedependency.Id)
        {
            return BadRequest();
        }

        try
        {
            servicedependency.Validate();
        }
        catch(InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }

        _context.Entry(servicedependency).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ServiceDependencyExists(id))
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

    // POST: api/ServiceDependency
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<ServiceDependency>> PostServiceDependency(ServiceDependency servicedependency)
    {
        try
        {
            servicedependency.Validate();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        _context.ServiceDependency.Add(servicedependency);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetServiceDependency", new { id = servicedependency.Id }, servicedependency);
    }

    // DELETE: api/ServiceDependency/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteServiceDependency(int? id)
    {
        var servicedependency = await _context.ServiceDependency.FindAsync(id);
        if (servicedependency == null)
        {
            return NotFound();
        }

        _context.ServiceDependency.Remove(servicedependency);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ServiceDependencyExists(int? id)
    {
        return _context.ServiceDependency.Any(e => e.Id == id);
    }
}
