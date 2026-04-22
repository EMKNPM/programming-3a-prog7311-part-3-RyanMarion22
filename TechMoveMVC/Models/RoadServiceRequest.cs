using TechMoveMVC.Interfaces;

namespace TechMoveMVC.Models
{
    public class RoadServiceRequest : ServiceRequest, IServiceRequest
    {
        public string GetServiceType() => "Road Transport";
    }
}