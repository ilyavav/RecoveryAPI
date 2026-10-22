namespace RecoveryAPI.Dtos
{
    public class FailedServiceReportDto
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public int Criticality { get; set; }
        public double EstimatedRecoverySeconds { get; set; }
        public int AttemptCount { get; set; }
    }
}