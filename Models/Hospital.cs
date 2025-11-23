using System.ComponentModel.DataAnnotations;

namespace DurdansRazor.Models
{
    public class Hospital
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        // Navigation property for many-to-many relationship with Doctors
        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}
