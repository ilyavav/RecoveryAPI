using System.Text.Json.Serialization;

namespace RecoveryAPI.Models
{
    public class RecoveryAttempt
    {
        public int Id { get; set; }
        public int RecoverySessionServiceId { get; set; }
        public RecoveryAttemptStatus Status { get; set; } = RecoveryAttemptStatus.Pending;
        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public string? ErrorMessage { get; set; }

        [JsonIgnore]
        public RecoverySessionService? RecoverySessionService { get; set; }

        public bool TryStart()
        {
            if (Status != RecoveryAttemptStatus.Pending)
            {
                return false;
            }

            StartedAt = DateTime.UtcNow;
            Status = RecoveryAttemptStatus.Running;

            return true;
        }

        public bool TryComplete(bool isSuccessful, string? errorMessage = null)
        {
            if (Status != RecoveryAttemptStatus.Running || !StartedAt.HasValue)
            {
                return false;
            }

            var finishedAt = DateTime.UtcNow;

            if (finishedAt < StartedAt.Value)
            {
                return false;
            }

            FinishedAt = finishedAt;
            Status = isSuccessful ? RecoveryAttemptStatus.Succeeded : RecoveryAttemptStatus.Failed;
            ErrorMessage = isSuccessful ? null : errorMessage?.Trim();

            return true;
        }

        public double? GetDurationSeconds()
        {
            if (!StartedAt.HasValue || !FinishedAt.HasValue)
            {
                return null;
            }

            return (FinishedAt.Value - StartedAt.Value).TotalSeconds;
        }
    }

    public enum RecoveryAttemptStatus
    {
        Pending = 0,
        Running = 1,
        Succeeded = 2,
        Failed = 3
    }
}