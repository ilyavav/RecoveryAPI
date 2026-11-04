using System.ComponentModel.DataAnnotations;

namespace RecoveryAPI.Dtos
{
    public class ServiceObservationDto
    {
        [Range(0, int.MaxValue)]
        public required int ReadyReplicas { get; set; }

        public required DateTime ObservedAt { get; set; }
    }
}