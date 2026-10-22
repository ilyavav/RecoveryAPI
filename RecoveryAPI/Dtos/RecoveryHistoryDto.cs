namespace RecoveryAPI.Dtos
{
    public class RecoveryHistoryDto
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public bool? IsSuccessful { get; set; }
    }
}