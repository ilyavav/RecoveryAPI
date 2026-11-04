using Microsoft.EntityFrameworkCore;
using RecoveryAPI.Dtos;
using RecoveryAPI.Models;

namespace RecoveryAPI.Services
{
    public class RecoverySessionServiceManager
    {
        private readonly RecoveryAPIContext _context;

        public RecoverySessionServiceManager(RecoveryAPIContext context)
        {
            _context = context;
        }

        public async Task<List<RecoverySessionServiceDto>> GetParticipantsAsync(int? sessionId = null)
        {
            var query = GetParticipantsWithDetails().AsNoTracking();

            if (sessionId.HasValue)
            {
                query = query.Where(participant => participant.RecoverySessionId == sessionId.Value);
            }

            var participants = await query.OrderBy(participant => participant.Id).ToListAsync();

            return participants.Select(participant => ToDto(participant)).ToList();
        }

        public async Task<RecoverySessionServiceDto?> GetParticipantAsync(int id)
        {
            var participant = await GetParticipantsWithDetails().AsNoTracking().FirstOrDefaultAsync(participant => participant.Id == id);

            if (participant == null)
            {
                return null;
            }

            return ToDto(participant);
        }

        public async Task<(bool Found, RecoverySessionServiceDto? Participant, string? Error)> AddParticipantAsync(int sessionId, int serviceId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var session = await _context.RecoverySession.FindAsync(sessionId);
            var service = await _context.Service.FindAsync(serviceId);

            if (session == null || service == null)
            {
                return (false, null, null);
            }

            if (session.Status != RecoverySessionStatus.Created)
            {
                return (true, null, "Services can only be added before the session starts.");
            }

            var alreadyExists = await _context.RecoverySessionService.AnyAsync(participant => participant.RecoverySessionId == sessionId && participant.ServiceId == serviceId);

            if (alreadyExists)
            {
                return (true, null, "The service is already included in this session.");
            }

            var participant = new RecoverySessionService
            {
                RecoverySessionId = sessionId,
                ServiceId = serviceId,
                RecoverySession = session,
                Service = service
            };

            _context.RecoverySessionService.Add(participant);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, ToDto(participant), null);
        }

        public async Task<(bool Found, RecoverySessionServiceDto? Participant, string? Error)> UpdateParticipantAsync(int id, int serviceId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var participant = await GetParticipantsWithDetails().FirstOrDefaultAsync(participant => participant.Id == id);

            if (participant == null)
            {
                return (false, null, null);
            }

            if (participant.RecoverySession!.Status != RecoverySessionStatus.Created)
            {
                return (true, null, "A participant cannot be changed after the session starts.");
            }

            var service = await _context.Service.FindAsync(serviceId);

            if (service == null)
            {
                return (false, null, null);
            }

            var alreadyExists = await _context.RecoverySessionService.AnyAsync(other => other.RecoverySessionId == participant.RecoverySessionId && other.ServiceId == serviceId && other.Id != id);

            if (alreadyExists)
            {
                return (true, null, "The service is already included in this session.");
            }

            participant.ServiceId = serviceId;
            participant.Service = service;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, ToDto(participant), null);
        }

        public async Task<(bool Found, string? Error)> DeleteParticipantAsync(int id)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var participant = await GetParticipantsWithDetails().FirstOrDefaultAsync(participant => participant.Id == id);

            if (participant == null)
            {
                return (false, null);
            }

            if (participant.RecoverySession!.Status != RecoverySessionStatus.Created)
            {
                return (true, "A participant cannot be deleted after the session starts.");
            }

            var hasAttempts = await _context.RecoveryAttempt.AnyAsync(attempt => attempt.RecoverySessionServiceId == id);

            if (hasAttempts)
            {
                return (true, "The participant has recovery attempts.");
            }

            _context.RecoverySessionService.Remove(participant);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, null);
        }

        private IQueryable<RecoverySessionService> GetParticipantsWithDetails()
        {
            return _context.RecoverySessionService.Include(participant => participant.Service).Include(participant => participant.RecoverySession);
        }

        private static RecoverySessionServiceDto ToDto(RecoverySessionService participant)
        {
            var isCompleted = participant.RecoverySession!.Status == RecoverySessionStatus.Succeeded || participant.RecoverySession.Status == RecoverySessionStatus.Failed;

            return new RecoverySessionServiceDto
            {
                Id = participant.Id,
                RecoverySessionId = participant.RecoverySessionId,
                ServiceId = participant.ServiceId,
                ServiceName = participant.Service!.Name,
                TargetReplicas = participant.PriorityAtStart.HasValue ? participant.TargetReplicas : null,
                PriorityAtStart = participant.PriorityAtStart,
                ReadyReplicas = isCompleted ? participant.FinalReadyReplicas : participant.Service.ReadyReplicas,
                ObservedAt = isCompleted ? participant.FinalObservedAt : participant.Service.ObservedAt
            };
        }
    }
}