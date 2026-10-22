using Microsoft.EntityFrameworkCore;
using RecoveryAPI.Dtos;
using RecoveryAPI.Models;

namespace RecoveryAPI.Services
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
            var attempts = await _context.RecoveryAttempt.ToListAsync();

            return attempts.Select(attempt => ToDto(attempt)).ToList();
        }

        public async Task<RecoveryAttemptDto?> GetAttemptAsync(int id)
        {
            var attempt = await _context.RecoveryAttempt.FindAsync(id);

            if (attempt == null)
            {
                return null;
            }

            return ToDto(attempt);
        }

        public async Task<(RecoveryAttemptDto Attempt, string? Error)> AddAttemptAsync(RecoveryAttempt attempt)
        {
            var error = await GetValidationErrorAsync(attempt);

            if (error != null)
            {
                return (ToDto(attempt), error);
            }

            _context.RecoveryAttempt.Add(attempt);
            await _context.SaveChangesAsync();

            return (ToDto(attempt), null);
        }

        public async Task<(bool Found, string? Error)> UpdateAttemptAsync(RecoveryAttempt attempt)
        {
            var existingAttempt = await _context.RecoveryAttempt.FindAsync(attempt.Id);

            if (existingAttempt == null)
            {
                return (false, null);
            }

            var error = await GetValidationErrorAsync(attempt);

            if (error != null)
            {
                return (true, error);
            }

            existingAttempt.ServiceId = attempt.ServiceId;
            existingAttempt.StartedAt = attempt.StartedAt;
            existingAttempt.FinishedAt = attempt.FinishedAt;
            existingAttempt.IsSuccessful = attempt.IsSuccessful;

            await _context.SaveChangesAsync();

            return (true, null);
        }

        public async Task<bool> DeleteAttemptAsync(int id)
        {
            var attempt = await _context.RecoveryAttempt.FindAsync(id);

            if (attempt == null)
            {
                return false;
            }

            _context.RecoveryAttempt.Remove(attempt);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<(RecoveryAttemptDto? Attempt, RecoveryCompletionResult Result)> CompleteAttemptAsync(int id, bool isSuccessful)
        {
            var attempt = await _context.RecoveryAttempt.FindAsync(id);

            if (attempt == null)
            {
                return (null, RecoveryCompletionResult.None);
            }

            var result = attempt.Complete(isSuccessful, DateTime.UtcNow);

            if (result != RecoveryCompletionResult.Completed)
            {
                return (ToDto(attempt), result);
            }

            await _context.SaveChangesAsync();

            return (ToDto(attempt), result);
        }

        public async Task<(bool Found, RecoveryAttemptDurationDto? Duration)> GetDurationAsync(int id)
        {
            var attempt = await _context.RecoveryAttempt.FindAsync(id);

            if (attempt == null)
            {
                return (false, null);
            }

            var duration = new RecoveryAttemptDurationDto
            {
                DurationSeconds = attempt.GetDurationSeconds()
            };

            return (true, duration);
        }

        private async Task<string?> GetValidationErrorAsync(RecoveryAttempt attempt)
        {
            var serviceExists = await _context.Service.AnyAsync(service => service.Id == attempt.ServiceId);

            if (!serviceExists)
            {
                return "Service was not found.";
            }

            if (attempt.FinishedAt.HasValue && attempt.FinishedAt.Value < attempt.StartedAt)
            {
                return "Finish time cannot be earlier than start time.";
            }

            if (attempt.FinishedAt.HasValue != attempt.IsSuccessful.HasValue)
            {
                return "Finish time and recovery result must either both be provided or both be omitted.";
            }

            return null;
        }

        private static RecoveryAttemptDto ToDto(RecoveryAttempt attempt)
        {
            return new RecoveryAttemptDto
            {
                Id = attempt.Id,
                ServiceId = attempt.ServiceId,
                StartedAt = attempt.StartedAt,
                FinishedAt = attempt.FinishedAt,
                IsSuccessful = attempt.IsSuccessful
            };
        }
    }
}