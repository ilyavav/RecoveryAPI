using RecoveryAPI.Models;

namespace RecoveryAPI.Dtos
{
    public class RecoveryHistoryDto
    {
        public int Id { get; set; }
        public int RecoverySessionId { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public RecoveryAttemptStatus Status { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public string? ErrorMessage { get; set; }
    }
}