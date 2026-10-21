using Microsoft.AspNetCore.Mvc;
using RecoveryAPI.Models;
using Microsoft.AspNetCore.Authorization;
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
    public async Task<ActionResult<IEnumerable<RecoveryAttempt>>> GetRecoveryAttempt()
    {
        return await _attemptManager.GetAttemptsAsync();
    }

    // GET: api/RecoveryAttempt/5
    [HttpGet("{id}")]
    public async Task<ActionResult<RecoveryAttempt>> GetRecoveryAttempt(int id)
    {
        var recoveryattempt = await _attemptManager.GetAttemptAsync(id);

        if (recoveryattempt == null)
        {
            return NotFound();
        }

        return recoveryattempt;
    }

    // PUT: api/RecoveryAttempt/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> PutRecoveryAttempt(int id, RecoveryAttempt recoveryattempt)
    {
        if (id != recoveryattempt.Id)
        {
            return BadRequest();
        }

        var result = await _attemptManager.UpdateAttemptAsync(recoveryattempt);

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

    // POST: api/RecoveryAttempt
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<RecoveryAttempt>> PostRecoveryAttempt(RecoveryAttempt recoveryattempt)
    {
        var result = await _attemptManager.AddAttemptAsync(recoveryattempt);

        if (result.Error != null)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(nameof(GetRecoveryAttempt), new { id = result.Attempt.Id }, result.Attempt);
    }

    // DELETE: api/RecoveryAttempt/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteRecoveryAttempt(int id)
    {
        var deleted = await _attemptManager.DeleteAttemptAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPut("{id}/complete")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<RecoveryAttempt>> CompleteRecoveryAttempt(int id, [FromQuery] bool isSuccessful)
    {
        var result = await _attemptManager.CompleteAttemptAsync(id, isSuccessful);

        if (result.Attempt == null)
        {
            return NotFound();
        }

        if (result.Result == RecoveryCompletionResult.AlreadyCompleted)
        {
            return Conflict("Recovery attempt is already completed.");
        }

        if (result.Result == RecoveryCompletionResult.InvalidFinishTime)
        {
            return BadRequest("Finish time cannot be earlier than start time.");
        }

        return result.Attempt;
    }

    [HttpGet("{id}/duration")]
    public async Task<IActionResult> GetRecoveryAttemptDuration(int id)
    {
        var result = await _attemptManager.GetDurationAsync(id);

        if (!result.Found)
        {
            return NotFound();
        }

        return Ok(new { durationSeconds = result.DurationSeconds });
    }
}