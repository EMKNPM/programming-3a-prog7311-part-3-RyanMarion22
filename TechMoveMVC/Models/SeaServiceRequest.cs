using TechMoveMVC.Interfaces;

namespace TechMoveMVC.Models
{
    public class SeaServiceRequest : ServiceRequest, IServiceRequest
    {
        public string GetServiceType() => "Sea Freight";
    }
}