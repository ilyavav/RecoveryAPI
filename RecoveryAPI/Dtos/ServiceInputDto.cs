using System.ComponentModel.DataAnnotations;

namespace RecoveryAPI.Dtos
{
    public class ServiceInputDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Range(1, 5)]
        public required int DefaultPriority { get; set; }

        [Range(0, int.MaxValue)]
        public required int DesiredReplicas { get; set; }
    }
}