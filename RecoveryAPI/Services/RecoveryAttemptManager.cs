using Microsoft.EntityFrameworkCore;
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

        public async Task<List<RecoveryAttempt>> GetAttemptsAsync()
        {
            return await _context.RecoveryAttempt.ToListAsync();
        }

        public async Task<RecoveryAttempt?> GetAttemptAsync(int id)
        {
            return await _context.RecoveryAttempt.FindAsync(id);
        }

        public async Task<(RecoveryAttempt Attempt, string? Error)> AddAttemptAsync(RecoveryAttempt attempt)
        {
            var error = await GetValidationErrorAsync(attempt);

            if (error != null)
            {
                return (attempt, error);
            }

            _context.RecoveryAttempt.Add(attempt);
            await _context.SaveChangesAsync();

            return (attempt, null);
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

        public async Task<(RecoveryAttempt? Attempt, RecoveryCompletionResult Result)> CompleteAttemptAsync(int id, bool isSuccessful)
        {
            var attempt = await _context.RecoveryAttempt.FindAsync(id);

            if (attempt == null)
            {
                return (null, RecoveryCompletionResult.None);
            }

            var result = attempt.Complete(isSuccessful, DateTime.UtcNow);

            if (result != RecoveryCompletionResult.Completed)
            {
                return (attempt, result);
            }

            await _context.SaveChangesAsync();

            return (attempt, result);
        }

        public async Task<(bool Found, double? DurationSeconds)> GetDurationAsync(int id)
        {
            var attempt = await _context.RecoveryAttempt.FindAsync(id);

            if (attempt == null)
            {
                return (false, null);
            }

            return (true, attempt.GetDurationSeconds());
        }

        private async Task<string?> GetValidationErrorAsync(RecoveryAttempt attempt)
        {
            var serviceExists = await _context.Service.AnyAsync(service => service.Id == attempt.ServiceId);

            if (!serviceExists)
            {
                return "Service was not found.";
            }

            return null;
        }
    }
}