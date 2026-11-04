using System.ComponentModel.DataAnnotations;

namespace RecoveryAPI.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Range(1, 5)]
        public int DefaultPriority { get; set; }

        [Range(0, int.MaxValue)]
        public int DesiredReplicas { get; set; }

        [Range(0, int.MaxValue)]
        public int? ReadyReplicas { get; set; }

        public DateTime? ObservedAt { get; set; }

        public bool TrySetPriority(int priority)
        {
            if (priority < 1 || priority > 5)
            {
                return false;
            }

            DefaultPriority = priority;

            return true;
        }

        public bool TrySetDesiredReplicas(int desiredReplicas)
        {
            if (desiredReplicas < 0)
            {
                return false;
            }

            DesiredReplicas = desiredReplicas;

            return true;
        }

        public bool TryUpdateObservation(int readyReplicas, DateTime observedAt)
        {
            if (readyReplicas < 0 || observedAt == default)
            {
                return false;
            }

            if (ObservedAt.HasValue && observedAt < ObservedAt.Value)
            {
                return false;
            }

            if (ObservedAt.HasValue && observedAt == ObservedAt.Value)
            {
                return ReadyReplicas == readyReplicas;
            }

            ReadyReplicas = readyReplicas;
            ObservedAt = observedAt;

            return true;
        }

        public int? GetMissingReplicas()
        {
            if (!ReadyReplicas.HasValue)
            {
                return null;
            }

            return Math.Max(0, DesiredReplicas - ReadyReplicas.Value);
        }
    }
}