using System.ComponentModel.DataAnnotations;

namespace DurdansRazor.Models
{
    public class Patient
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [Phone]
        public string ContactNumber { get; set; } = null!;
    }
}
