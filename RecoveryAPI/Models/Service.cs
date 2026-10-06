using System.ComponentModel.DataAnnotations;

namespace RecoveryAPI.Models
{
    public class Service
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Range(1,5)]
        public int Criticality { get; set; }
        public ServiceStatus Status { get; set; }

        public void MakeFailed()
        {
            Status = ServiceStatus.Failed;
        }

        public void MakeRecovered()
        {
            if (Status != ServiceStatus.Recovering)
            {
                throw new InvalidOperationException("Service is not recovering.");
            }
            Status = ServiceStatus.Running;
        }
        public void MakeRecovering()
        {
            if (Status != ServiceStatus.Failed)
            {
                throw new InvalidOperationException("Recovery can only start for a failed service.");
            }
            Status = ServiceStatus.Recovering;
        }
    }
    public enum ServiceStatus
    {
        Unknown = 0,
        Running = 1,
        Failed = 2,
        Recovering = 3
    }
}
