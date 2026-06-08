using System.ComponentModel.DataAnnotations;

namespace TechMoveMVC.Models
{
    public class Client
    {
        public int ClientId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string ContactDetails { get; set; } = string.Empty;

        [Required]
        public string Region { get; set; } = string.Empty;

        
        public List<Contract>? Contracts { get; set; }
    }
}