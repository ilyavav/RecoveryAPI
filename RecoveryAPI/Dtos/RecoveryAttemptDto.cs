using RecoveryAPI.Models;

namespace RecoveryAPI.Dtos
{
    public class RecoveryAttemptDto
    {
        public int Id { get; set; }
        public int RecoverySessionServiceId { get; set; }
        public RecoveryAttemptStatus Status { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public string? ErrorMessage { get; set; }
    }
}