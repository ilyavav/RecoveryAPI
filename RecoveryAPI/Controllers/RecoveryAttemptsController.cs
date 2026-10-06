using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecoveryAPI.Models;

[Route("api/[controller]")]
[ApiController]
public class RecoveryAttemptsController : ControllerBase
{
    private readonly RecoveryAPIContext _context;
    public RecoveryAttemptsController(RecoveryAPIContext context)
    {
        _context = context;
    }

    // GET: api/RecoveryAttempt
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RecoveryAttempt>>> GetRecoveryAttempt()
    {
        return await _context.RecoveryAttempt.ToListAsync();
    }

    // GET: api/RecoveryAttempt/5
    [HttpGet("{id}")]
    public async Task<ActionResult<RecoveryAttempt>> GetRecoveryAttempt(int id)
    {
        var recoveryattempt = await _context.RecoveryAttempt.FindAsync(id);

        if (recoveryattempt == null)
        {
            return NotFound();
        }

        return recoveryattempt;
    }

    // PUT: api/RecoveryAttempt/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutRecoveryAttempt(int? id, RecoveryAttempt recoveryattempt)
    {
        if (id != recoveryattempt.Id)
        {
            return BadRequest();
        }

        _context.Entry(recoveryattempt).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!RecoveryAttemptExists(id))
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

    // POST: api/RecoveryAttempt
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<RecoveryAttempt>> PostRecoveryAttempt(RecoveryAttempt recoveryattempt)
    {
        _context.RecoveryAttempt.Add(recoveryattempt);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetRecoveryAttempt", new { id = recoveryattempt.Id }, recoveryattempt);
    }

    // DELETE: api/RecoveryAttempt/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecoveryAttempt(int? id)
    {
        var recoveryattempt = await _context.RecoveryAttempt.FindAsync(id);
        if (recoveryattempt == null)
        {
            return NotFound();
        }

        _context.RecoveryAttempt.Remove(recoveryattempt);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool RecoveryAttemptExists(int? id)
    {
        return _context.RecoveryAttempt.Any(e => e.Id == id);
    }
}
