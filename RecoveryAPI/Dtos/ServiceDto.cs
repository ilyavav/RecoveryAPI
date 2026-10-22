using RecoveryAPI.Models;

namespace RecoveryAPI.Dtos
{
    public class ServiceDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Criticality { get; set; }
        public double EstimatedRecoverySeconds { get; set; }
        public ServiceStatus Status { get; set; }
    }
}