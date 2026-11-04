namespace RecoveryAPI.Dtos
{
    public class RecoverySessionServiceDto
    {
        public int Id { get; set; }
        public int RecoverySessionId { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public int? TargetReplicas { get; set; }
        public int? PriorityAtStart { get; set; }
        public int? ReadyReplicas { get; set; }
        public DateTime? ObservedAt { get; set; }
    }
}