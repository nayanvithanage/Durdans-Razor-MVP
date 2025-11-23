using System.ComponentModel.DataAnnotations;

namespace DurdansRazor.Models
{
    public class Doctor
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Specialization { get; set; } = null!;

        [Range(0, 100000)]
        [Display(Name = "Consultation Fee")]
        public decimal ConsultationFee { get; set; }

        // JSON string to store availability (days/times)
        public string? AvailabilityJson { get; set; }

        // Navigation property for many-to-many relationship with Hospitals
        public ICollection<Hospital> Hospitals { get; set; } = new List<Hospital>();
    }
}
