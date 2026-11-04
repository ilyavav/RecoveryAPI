namespace RecoveryAPI.Dtos
{
    public class ServiceDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DefaultPriority { get; set; }
        public int DesiredReplicas { get; set; }
        public int? ReadyReplicas { get; set; }
        public DateTime? ObservedAt { get; set; }
    }
}