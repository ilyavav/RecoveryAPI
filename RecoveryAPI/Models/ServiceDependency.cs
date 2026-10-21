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

        public bool Validate()
        {
            if (ServiceId == DependsOnServiceId)
            {
                return false;
            }

            return true;
        }
    }
}