using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecoveryAPI.Dtos;
using RecoveryAPI.Services;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RecoveryAttemptsController : ControllerBase
{
    private readonly RecoveryAttemptManager _attemptManager;

    public RecoveryAttemptsController(RecoveryAttemptManager attemptManager)
    {
        _attemptManager = attemptManager;
    }

    // GET: api/RecoveryAttempt
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RecoveryAttemptDto>>> GetRecoveryAttempts()
    {
        return await _attemptManager.GetAttemptsAsync();
    }

    // GET: api/RecoveryAttempt/5
    [HttpGet("{id}")]
    public async Task<ActionResult<RecoveryAttemptDto>> GetRecoveryAttempt(int id)
    {
        var attempt = await _attemptManager.GetAttemptAsync(id);

        if (attempt == null)
        {
            return NotFound();
        }

        return attempt;
    }

    // POST: api/RecoveryAttempt
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<RecoveryAttemptDto>> PostRecoveryAttempt([FromQuery] int participantId)
    {
        if (participantId <= 0)
        {
            return BadRequest(new ErrorDto { Message = "A valid participant ID is required." });
        }

        var result = await _attemptManager.AddAttemptAsync(participantId);

        if (!result.Found)
        {
            return NotFound();
        }

        if (result.Error != null)
        {
            return Conflict(new ErrorDto { Message = result.Error });
        }

        return CreatedAtAction(nameof(GetRecoveryAttempt), new { id = result.Attempt!.Id }, result.Attempt);
    }

    // DELETE: api/RecoveryAttempt/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteRecoveryAttempt(int id)
    {
        var result = await _attemptManager.DeleteAttemptAsync(id);

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

    [HttpPut("{id}/start")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<RecoveryAttemptDto>> StartRecoveryAttempt(int id)
    {
        var result = await _attemptManager.StartAttemptAsync(id);

        if (!result.Found)
        {
            return NotFound();
        }

        if (result.Error != null)
        {
            return Conflict(new ErrorDto { Message = result.Error });
        }

        return Ok(result.Attempt);
    }

    [HttpPut("{id}/complete")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<RecoveryAttemptDto>> CompleteRecoveryAttempt(int id, [FromQuery] bool? isSuccessful, [FromQuery] string? errorMessage = null)
    {
        if (!isSuccessful.HasValue)
        {
            return BadRequest(new ErrorDto { Message = "The recovery result is required." });
        }

        var result = await _attemptManager.CompleteAttemptAsync(id, isSuccessful.Value, errorMessage);

        if (!result.Found)
        {
            return NotFound();
        }

        if (result.Error != null)
        {
            return Conflict(new ErrorDto { Message = result.Error });
        }

        return Ok(result.Attempt);
    }

    [HttpGet("{id}/duration")]
    public async Task<ActionResult<double>> GetRecoveryAttemptDuration(int id)
    {
        var result = await _attemptManager.GetDurationAsync(id);

        if (!result.Found)
        {
            return NotFound();
        }

        if (!result.Duration.HasValue)
        {
            return NoContent();
        }

        return Ok(result.Duration.Value);
    }
}