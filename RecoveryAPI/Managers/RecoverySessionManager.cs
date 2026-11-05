using Microsoft.EntityFrameworkCore;
using RecoveryAPI.Dtos;
using RecoveryAPI.Models;

namespace RecoveryAPI.Managers
{
    public class RecoverySessionManager
    {
        private readonly RecoveryAPIContext _context;

        public RecoverySessionManager(RecoveryAPIContext context)
        {
            _context = context;
        }

        public async Task<List<RecoverySessionDto>> GetSessionsAsync()
        {
            var sessions = await _context.RecoverySession.AsNoTracking().OrderBy(session => session.Id).ToListAsync();

            return sessions.Select(session => ToDto(session)).ToList();
        }

        public async Task<RecoverySessionDto?> GetSessionAsync(int id)
        {
            var session = await _context.RecoverySession.AsNoTracking().FirstOrDefaultAsync(session => session.Id == id);

            if (session == null)
            {
                return null;
            }

            return ToDto(session);
        }

        public async Task<RecoverySessionDto> AddSessionAsync()
        {
            var session = new RecoverySession();

            _context.RecoverySession.Add(session);
            await _context.SaveChangesAsync();

            return ToDto(session);
        }

        public async Task<(bool Found, RecoverySessionDto? Session, string? Error)> StartSessionAsync(int id)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var session = await _context.RecoverySession.FindAsync(id);

            if (session == null)
            {
                return (false, null, null);
            }

            if (session.Status != RecoverySessionStatus.Created)
            {
                return (true, null, "Only a created session can be started.");
            }

            var hasRunningSession = await _context.RecoverySession.AnyAsync(session => session.Status == RecoverySessionStatus.Running);

            if (hasRunningSession)
            {
                return (true, null, "Another recovery session is already running.");
            }

            var participants = await _context.RecoverySessionService.Include(participant => participant.Service).Where(participant => participant.RecoverySessionId == id).ToListAsync();

            if (participants.Count == 0)
            {
                return (true, null, "The session must contain at least one service.");
            }

            foreach (var participant in participants)
            {
                if (!participant.TryCaptureStartState(participant.Service!.DesiredReplicas, participant.Service.DefaultPriority))
                {
                    return (true, null, "The recovery goal could not be captured.");
                }
            }

            if (!session.TryStart())
            {
                return (true, null, "The session could not be started.");
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, ToDto(session), null);
        }

        public async Task<(bool Found, RecoverySessionDto? Session, string? Error)> CompleteSessionAsync(int id, bool isSuccessful)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var session = await _context.RecoverySession.FindAsync(id);

            if (session == null)
            {
                return (false, null, null);
            }

            if (session.Status != RecoverySessionStatus.Running || !session.StartedAt.HasValue)
            {
                return (true, null, "Only a running session can be completed.");
            }

            var hasActiveAttempts = await _context.RecoveryAttempt.AnyAsync(attempt => attempt.RecoverySessionService!.RecoverySessionId == id && (attempt.Status == RecoveryAttemptStatus.Pending || attempt.Status == RecoveryAttemptStatus.Running));

            if (hasActiveAttempts)
            {
                return (true, null, "The session has unfinished attempts.");
            }

            var participants = await _context.RecoverySessionService.Include(participant => participant.Service).Where(participant => participant.RecoverySessionId == id).ToListAsync();

            if (participants.Count == 0)
            {
                return (true, null, "The session has no participants.");
            }

            foreach (var participant in participants)
            {
                var service = participant.Service!;

                if (isSuccessful && !participant.IsTargetReached(service.ReadyReplicas))
                {
                    return (true, null, "Not all recovery targets have been reached.");
                }

                if (isSuccessful && (!service.ObservedAt.HasValue || service.ObservedAt.Value < session.StartedAt.Value))
                {
                    return (true, null, "Successful recovery must be confirmed by observations received after the session started.");
                }

                if (service.ObservedAt.HasValue && service.ObservedAt.Value > DateTime.UtcNow)
                {
                    return (true, null, "Observation time cannot be in the future.");
                }

                if (service.ReadyReplicas.HasValue && service.ObservedAt.HasValue)
                {
                    if (!participant.TryCaptureFinalState(service.ReadyReplicas.Value, service.ObservedAt.Value))
                    {
                        return (true, null, "The final service state could not be captured.");
                    }
                }
            }

            if (!session.TryComplete(isSuccessful))
            {
                return (true, null, "The session could not be completed.");
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, ToDto(session), null);
        }

        public async Task<(bool Found, string? Error)> DeleteSessionAsync(int id)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var session = await _context.RecoverySession.FindAsync(id);

            if (session == null)
            {
                return (false, null);
            }

            if (session.Status != RecoverySessionStatus.Created)
            {
                return (true, "Only a session that has not started can be deleted.");
            }

            var hasAttempts = await _context.RecoveryAttempt.AnyAsync(attempt => attempt.RecoverySessionService!.RecoverySessionId == id);

            if (hasAttempts)
            {
                return (true, "The session has recovery attempts.");
            }

            var participants = await _context.RecoverySessionService.Where(participant => participant.RecoverySessionId == id).ToListAsync();

            _context.RecoverySessionService.RemoveRange(participants);
            _context.RecoverySession.Remove(session);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, null);
        }

        private static RecoverySessionDto ToDto(RecoverySession session)
        {
            return new RecoverySessionDto
            {
                Id = session.Id,
                Status = session.Status,
                CreatedAt = session.CreatedAt,
                StartedAt = session.StartedAt,
                FinishedAt = session.FinishedAt
            };
        }
    }
}