using Microsoft.EntityFrameworkCore;
using RecoveryAPI.Dtos;
using RecoveryAPI.Models;

namespace RecoveryAPI.Managers
{
    public class RecoveryAttemptManager
    {
        private readonly RecoveryAPIContext _context;

        public RecoveryAttemptManager(RecoveryAPIContext context)
        {
            _context = context;
        }

        public async Task<List<RecoveryAttemptDto>> GetAttemptsAsync()
        {
            var attempts = await _context.RecoveryAttempt.AsNoTracking().ToListAsync();

            return attempts.Select(attempt => ToDto(attempt)).ToList();
        }

        public async Task<RecoveryAttemptDto?> GetAttemptAsync(int id)
        {
            var attempt = await _context.RecoveryAttempt.AsNoTracking().FirstOrDefaultAsync(attempt => attempt.Id == id);

            if (attempt == null)
            {
                return null;
            }

            return ToDto(attempt);
        }

        public async Task<(bool Found, RecoveryAttemptDto? Attempt, string? Error)> AddAttemptAsync(int participantId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var participant = await _context.RecoverySessionService.Include(participant => participant.RecoverySession).FirstOrDefaultAsync(participant => participant.Id == participantId);

            if (participant == null)
            {
                return (false, null, null);
            }

            if (participant.RecoverySession!.Status != RecoverySessionStatus.Running || !participant.PriorityAtStart.HasValue)
            {
                return (true, null, "Attempts can only be added to a running recovery session.");
            }

            var attempt = new RecoveryAttempt
            {
                RecoverySessionServiceId = participantId
            };

            _context.RecoveryAttempt.Add(attempt);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, ToDto(attempt), null);
        }

        public async Task<(bool Found, RecoveryAttemptDto? Attempt, string? Error)> StartAttemptAsync(int id)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var attempt = await GetAttemptWithSessionAsync(id);

            if (attempt == null)
            {
                return (false, null, null);
            }

            if (attempt.RecoverySessionService!.RecoverySession!.Status != RecoverySessionStatus.Running)
            {
                return (true, null, "The recovery session is not running.");
            }

            if (!attempt.TryStart())
            {
                return (true, null, "Only a pending attempt can be started.");
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, ToDto(attempt), null);
        }

        public async Task<(bool Found, RecoveryAttemptDto? Attempt, string? Error)> CompleteAttemptAsync(int id, bool isSuccessful, string? errorMessage)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var attempt = await GetAttemptWithSessionAsync(id);

            if (attempt == null)
            {
                return (false, null, null);
            }

            if (attempt.RecoverySessionService!.RecoverySession!.Status != RecoverySessionStatus.Running)
            {
                return (true, null, "The recovery session is not running.");
            }

            if (!attempt.TryComplete(isSuccessful, errorMessage))
            {
                return (true, null, "The attempt is not running or its completion time is invalid.");
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, ToDto(attempt), null);
        }

        public async Task<(bool Found, string? Error)> DeleteAttemptAsync(int id)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var attempt = await _context.RecoveryAttempt.FindAsync(id);

            if (attempt == null)
            {
                return (false, null);
            }

            if (attempt.Status != RecoveryAttemptStatus.Pending)
            {
                return (true, "An attempt cannot be deleted after execution has started.");
            }

            _context.RecoveryAttempt.Remove(attempt);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, null);
        }

        public async Task<(bool Found, double? Duration)> GetDurationAsync(int id)
        {
            var attempt = await _context.RecoveryAttempt.AsNoTracking().FirstOrDefaultAsync(attempt => attempt.Id == id);

            if (attempt == null)
            {
                return (false, null);
            }

            return (true, attempt.GetDurationSeconds());
        }

        private Task<RecoveryAttempt?> GetAttemptWithSessionAsync(int id)
        {
            return _context.RecoveryAttempt.Include(attempt => attempt.RecoverySessionService).ThenInclude(participant => participant!.RecoverySession).FirstOrDefaultAsync(attempt => attempt.Id == id);
        }

        private static RecoveryAttemptDto ToDto(RecoveryAttempt attempt)
        {
            return new RecoveryAttemptDto
            {
                Id = attempt.Id,
                RecoverySessionServiceId = attempt.RecoverySessionServiceId,
                Status = attempt.Status,
                StartedAt = attempt.StartedAt,
                FinishedAt = attempt.FinishedAt,
                ErrorMessage = attempt.ErrorMessage
            };
        }
    }
}