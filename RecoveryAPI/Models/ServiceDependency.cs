using System.Text.Json.Serialization;

namespace RecoveryAPI.Models
{
    public class ServiceDependency
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public int DependsOnServiceId { get; set; }
        [JsonIgnore]
        public Service? Service { get; set; }
        [JsonIgnore]
        public Service? DependsOnService { get; set; }

        public void Validate()
        {
            if (ServiceId == DependsOnServiceId)
            {
                throw new InvalidOperationException("A service cannot depend on itself.");
            }
        }
    }
}
