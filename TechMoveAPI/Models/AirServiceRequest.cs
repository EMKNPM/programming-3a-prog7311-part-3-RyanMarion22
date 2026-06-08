using TechMoveMVC.Interfaces;

namespace TechMoveMVC.Models
{
    public class AirServiceRequest : ServiceRequest, IServiceRequest
    {
        public string GetServiceType() => "Air Freight";
    }
}