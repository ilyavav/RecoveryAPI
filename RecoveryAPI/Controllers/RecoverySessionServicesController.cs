using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecoveryAPI.Dtos;
using RecoveryAPI.Managers;

namespace RecoveryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RecoverySessionServicesController : ControllerBase
    {
        private readonly RecoverySessionServiceManager _participantManager;

        public RecoverySessionServicesController(RecoverySessionServiceManager participantManager)
        {
            _participantManager = participantManager;
        }

        // GET: api/RecoverySessionServices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecoverySessionServiceDto>>> GetRecoverySessionServices([FromQuery] int? sessionId)
        {
            if (sessionId.HasValue && sessionId.Value <= 0)
            {
                return BadRequest(new ErrorDto { Message = "A valid session ID is required." });
            }

            return await _participantManager.GetParticipantsAsync(sessionId);
        }

        // GET: api/RecoverySessionServices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RecoverySessionServiceDto>> GetRecoverySessionService(int id)
        {
            var participant = await _participantManager.GetParticipantAsync(id);

            if (participant == null)
            {
                return NotFound();
            }

            return participant;
        }

        // PUT: api/RecoverySessionServices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<RecoverySessionServiceDto>> PutRecoverySessionService(int id, [FromQuery] int serviceId)
        {
            if (serviceId <= 0)
            {
                return BadRequest(new ErrorDto { Message = "A valid service ID is required." });
            }

            var result = await _participantManager.UpdateParticipantAsync(id, serviceId);

            if (!result.Found)
            {
                return NotFound();
            }

            if (result.Error != null)
            {
                return Conflict(new ErrorDto { Message = result.Error });
            }

            return Ok(result.Participant);
        }

        // POST: api/RecoverySessionServices
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<RecoverySessionServiceDto>> PostRecoverySessionService([FromQuery] int sessionId, [FromQuery] int serviceId)
        {
            if (sessionId <= 0 || serviceId <= 0)
            {
                return BadRequest(new ErrorDto { Message = "Valid session and service IDs are required." });
            }

            var result = await _participantManager.AddParticipantAsync(sessionId, serviceId);

            if (!result.Found)
            {
                return NotFound();
            }

            if (result.Error != null)
            {
                return Conflict(new ErrorDto { Message = result.Error });
            }

            return CreatedAtAction(nameof(GetRecoverySessionService), new { id = result.Participant!.Id }, result.Participant);
        }

        // DELETE: api/RecoverySessionServices/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteRecoverySessionService(int id)
        {
            var result = await _participantManager.DeleteParticipantAsync(id);

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
}