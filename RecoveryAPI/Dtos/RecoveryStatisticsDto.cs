namespace RecoveryAPI.Dtos
{
    public class RecoveryStatisticsDto
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public int AttemptCount { get; set; }
        public int SuccessfulCount { get; set; }
        public int FailedCount { get; set; }
    }
}