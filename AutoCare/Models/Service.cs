using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoCare.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required]
        public double Miles { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public string Detail { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm}")]
        public DateTime DateAdded { get; set; }

        public int CarId { get; set; }
        public int ServicetypeId { get; set; }

        [ForeignKey("CarId")]
        public virtual Car Car { get; set; }

        [ForeignKey("ServicetypeId")]
        public virtual ServiceType ServiceType { get; set; }
    }
}