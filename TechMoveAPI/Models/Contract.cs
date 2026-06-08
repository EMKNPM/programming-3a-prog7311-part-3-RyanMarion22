using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TechMoveMVC.Interfaces;

namespace TechMoveMVC.Models
{
    public class Contract
    {
        public int ContractId { get; set; }

        [Required]
        public int ClientId { get; set; }

        [ValidateNever] 
        public Client? Client { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string ServiceLevel { get; set; } = string.Empty;

        public string? FilePath { get; set; }

        [ValidateNever] 
        public List<ServiceRequest>? ServiceRequests { get; set; }

        private List<IObserver> observers = new List<IObserver>();

        public void Attach(IObserver observer)
        {
            observers.Add(observer);
        }

        public void Notify()
        {
            foreach (var observer in observers)
            {
                observer.Update($"Contract status changed to {Status}");
            }
        }

        private string status = "Draft";

        [Required]
        public string Status
        {
            get => status;
            set
            {
                status = value;
                Notify();
            }
        }
    }
}