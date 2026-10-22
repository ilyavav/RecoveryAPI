namespace RecoveryAPI.Dtos
{
    public class ServiceDependencyDto
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public int DependsOnServiceId { get; set; }
    }
}