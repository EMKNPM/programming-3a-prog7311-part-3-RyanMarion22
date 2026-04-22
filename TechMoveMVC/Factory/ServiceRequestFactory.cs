using TechMoveMVC.Models;

namespace TechMoveMVC.Factory
{
    public class ServiceRequestFactory
    {
        public static ServiceRequest Create(string type)
        {
            return type switch
            {
                "Air" => new AirServiceRequest(),
                "Sea" => new SeaServiceRequest(),
                "Road" => new RoadServiceRequest(),
                _ => throw new ArgumentException("Invalid Service Type")
            };
        }
    }
}