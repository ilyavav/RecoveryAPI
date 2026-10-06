using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecoveryAPI.Models;

[Route("api/[controller]")]
[ApiController]
public class ServicesController : ControllerBase
{
    private readonly RecoveryAPIContext _context;
    public ServicesController(RecoveryAPIContext context)
    {
        _context = context;
    }

    // GET: api/Service
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Service>>> GetService()
    {
        return await _context.Service.ToListAsync();
    }

    // GET: api/Service/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Service>> GetService(int id)
    {
        var service = await _context.Service.FindAsync(id);

        if (service == null)
        {
            return NotFound();
        }

        return service;
    }

    // PUT: api/Service/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
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
    public async Task<ActionResult<Service>> PostService(Service service)
    {
        _context.Service.Add(service);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetService", new { id = service.Id }, service);
    }

    // DELETE: api/Service/5
    [HttpDelete("{id}")]
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
}
