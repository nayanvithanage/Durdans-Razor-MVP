using DurdansRazor.Models;
using Microsoft.EntityFrameworkCore;

namespace DurdansRazor.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure many-to-many relationship between Doctor and Hospital
            modelBuilder.Entity<Doctor>()
                .HasMany(d => d.Hospitals)
                .WithMany(h => h.Doctors)
                .UsingEntity(j => j.ToTable("DoctorHospitals"));

            // Configure relationships for Appointment
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany()
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany()
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Hospital)
                .WithMany()
                .HasForeignKey(a => a.HospitalId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
