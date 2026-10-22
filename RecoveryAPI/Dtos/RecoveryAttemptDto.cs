namespace RecoveryAPI.Dtos
{
    public class RecoveryAttemptDto
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public bool? IsSuccessful { get; set; }
    }
}