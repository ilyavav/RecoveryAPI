namespace RecoveryAPI.Models
{
    public class RecoverySession
    {
        public int Id { get; set; }
        public RecoverySessionStatus Status { get; set; } = RecoverySessionStatus.Created;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }

        public bool TryStart()
        {
            if (Status != RecoverySessionStatus.Created)
            {
                return false;
            }

            StartedAt = DateTime.UtcNow;
            Status = RecoverySessionStatus.Running;

            return true;
        }

        public bool TryComplete(bool isSuccessful)
        {
            if (Status != RecoverySessionStatus.Running || !StartedAt.HasValue)
            {
                return false;
            }

            var finishedAt = DateTime.UtcNow;

            if (finishedAt < StartedAt.Value)
            {
                return false;
            }

            FinishedAt = finishedAt;
            Status = isSuccessful ? RecoverySessionStatus.Succeeded : RecoverySessionStatus.Failed;

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

    public enum RecoverySessionStatus
    {
        Created = 0,
        Running = 1,
        Succeeded = 2,
        Failed = 3
    }
}