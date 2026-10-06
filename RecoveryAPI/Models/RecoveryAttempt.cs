namespace RecoveryAPI.Models
{
    public class RecoveryAttempt
    {
        public int Id { get;set; }
        public int ServiceId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public bool? IsSuccessful { get; set; }

        public void Complete(bool isSuccessful, DateTime finishedAt)
        {
            if (FinishedAt != null)
            {
                throw new InvalidOperationException("Recovery attempt is already completed.");
            }
            if (finishedAt < StartedAt)
            {
                throw new ArgumentException("Finish time cannot be earlier than start time.");
            }
            FinishedAt = finishedAt;
            IsSuccessful = isSuccessful;
        }

        public double? GetDurationSeconds()
        {
            if (FinishedAt == null)
            {
                return null;
            }
            else
            {
                return (FinishedAt.Value - StartedAt).TotalSeconds;
            }
        }
    }
}
