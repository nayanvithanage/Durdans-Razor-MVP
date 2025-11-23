using System.ComponentModel.DataAnnotations;

namespace DurdansRazor.Models
{
    public enum AppointmentStatus
    {
        Booked,
        Confirmed,
        Completed,
        Cancelled
    }

    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        public int DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

        [Required]
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        [Required]
        public int HospitalId { get; set; }
        public Hospital? Hospital { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Appointment Date")]
        public DateTime AppointmentDate { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Booked;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
