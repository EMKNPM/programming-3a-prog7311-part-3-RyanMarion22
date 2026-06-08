using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TechMoveMVC.Models
{
    public class ServiceRequest
    {
        public int ServiceRequestId { get; set; }

        [Required(ErrorMessage = "Please select a contract")]
        public int ContractId { get; set; }

        public Contract? Contract { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public decimal CostUSD { get; set; }

        public decimal CostZAR { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";

        
        public string ServiceType { get; set; } = string.Empty;
    }
}