namespace RecoveryAPI.Dtos
{
    public class DependencyReportDto
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public int DependsOnServiceId { get; set; }
        public string DependsOnServiceName { get; set; } = string.Empty;
    }
}