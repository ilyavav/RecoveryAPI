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
        public int Criticality { get; set; }

        public ServiceStatus Status { get; set; }

        public void MakeFailed()
        {
            Status = ServiceStatus.Failed;
        }

        public bool TryMakeRecovered()
        {
            if (Status != ServiceStatus.Recovering)
            {
                return false;
            }

            Status = ServiceStatus.Running;
            return true;
        }

        public bool TryMakeRecovering()
        {
            if (Status != ServiceStatus.Failed)
            {
                return false;
            }

            Status = ServiceStatus.Recovering;
            return true;
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