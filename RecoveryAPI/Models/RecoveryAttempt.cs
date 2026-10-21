using System.Text.Json.Serialization;

namespace RecoveryAPI.Models
{
    public class RecoveryAttempt
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        [JsonIgnore]
        public Service? Service { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public bool? IsSuccessful { get; set; }

        public RecoveryCompletionResult Complete(bool isSuccessful, DateTime finishedAt)
        {
            if (FinishedAt != null)
            {
                return RecoveryCompletionResult.AlreadyCompleted;
            }

            if (finishedAt < StartedAt)
            {
                return RecoveryCompletionResult.InvalidFinishTime;
            }

            FinishedAt = finishedAt;
            IsSuccessful = isSuccessful;

            return RecoveryCompletionResult.Completed;
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

    public enum RecoveryCompletionResult
    {
        None = 0,
        Completed = 1,
        AlreadyCompleted = 2,
        InvalidFinishTime = 3
    }
}