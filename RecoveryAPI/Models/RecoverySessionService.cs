using System.Text.Json.Serialization;

namespace RecoveryAPI.Models
{
    public class RecoverySessionService
    {
        public int Id { get; set; }
        public int RecoverySessionId { get; set; }
        public int ServiceId { get; set; }
        public int TargetReplicas { get; set; }
        public int? PriorityAtStart { get; set; }
        public int? FinalReadyReplicas { get; set; }
        public DateTime? FinalObservedAt { get; set; }

        [JsonIgnore]
        public RecoverySession? RecoverySession { get; set; }

        [JsonIgnore]
        public Service? Service { get; set; }

        public bool TryCaptureStartState(int targetReplicas, int priority)
        {
            if (PriorityAtStart.HasValue)
            {
                return false;
            }

            if (targetReplicas < 0 || priority < 1 || priority > 5)
            {
                return false;
            }

            TargetReplicas = targetReplicas;
            PriorityAtStart = priority;

            return true;
        }

        public bool TryCaptureFinalState(int readyReplicas, DateTime observedAt)
        {
            if (!PriorityAtStart.HasValue || FinalReadyReplicas.HasValue)
            {
                return false;
            }

            if (readyReplicas < 0 || observedAt == default)
            {
                return false;
            }

            FinalReadyReplicas = readyReplicas;
            FinalObservedAt = observedAt;

            return true;
        }

        public bool IsTargetReached(int? readyReplicas)
        {
            return PriorityAtStart.HasValue && readyReplicas.HasValue && readyReplicas.Value >= TargetReplicas;
        }
    }
}