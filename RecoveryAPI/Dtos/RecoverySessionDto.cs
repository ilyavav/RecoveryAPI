using RecoveryAPI.Models;

namespace RecoveryAPI.Dtos
{
    public class RecoverySessionDto
    {
        public int Id { get; set; }
        public RecoverySessionStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
    }
}