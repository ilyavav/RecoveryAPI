using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecoveryAPI.Dtos;
using RecoveryAPI.Managers;

namespace RecoveryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RecoverySessionsController : ControllerBase
    {
        private readonly RecoverySessionManager _sessionManager;

        public RecoverySessionsController(RecoverySessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        // GET: api/RecoverySessions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecoverySessionDto>>> GetRecoverySessions()
        {
            return await _sessionManager.GetSessionsAsync();
        }

        // GET: api/RecoverySessions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RecoverySessionDto>> GetRecoverySession(int id)
        {
            var session = await _sessionManager.GetSessionAsync(id);

            if (session == null)
            {
                return NotFound();
            }

            return session;
        }

        // POST: api/RecoverySessions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<RecoverySessionDto>> PostRecoverySession()
        {
            var session = await _sessionManager.AddSessionAsync();

            return CreatedAtAction(nameof(GetRecoverySession), new { id = session.Id }, session);
        }

        // DELETE: api/RecoverySessions/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteRecoverySession(int id)
        {
            var result = await _sessionManager.DeleteSessionAsync(id);

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
        public async Task<ActionResult<RecoverySessionDto>> StartRecoverySession(int id)
        {
            var result = await _sessionManager.StartSessionAsync(id);

            if (!result.Found)
            {
                return NotFound();
            }

            if (result.Error != null)
            {
                return Conflict(new ErrorDto { Message = result.Error });
            }

            return Ok(result.Session);
        }

        [HttpPut("{id}/complete")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<RecoverySessionDto>> CompleteRecoverySession(int id, [FromQuery] bool? isSuccessful)
        {
            if (!isSuccessful.HasValue)
            {
                return BadRequest(new ErrorDto { Message = "The recovery result is required." });
            }

            var result = await _sessionManager.CompleteSessionAsync(id, isSuccessful.Value);

            if (!result.Found)
            {
                return NotFound();
            }

            if (result.Error != null)
            {
                return Conflict(new ErrorDto { Message = result.Error });
            }

            return Ok(result.Session);
        }
    }
}