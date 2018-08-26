using System.ComponentModel.DataAnnotations;

namespace AutoCare.Models
{
    public class ServiceType
    {
        public int ServicetypeId { get; set; }

        [Required]
        public string Name { get; set; }
    }
}