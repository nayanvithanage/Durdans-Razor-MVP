using DurdansRazor.Models;
using DurdansRazor.Repositories;

namespace DurdansRazor.Services
{
    public interface IAppointmentService
    {
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
        Task<Appointment?> GetAppointmentByIdAsync(int id);
        Task<bool> BookAppointmentAsync(Appointment appointment);
        Task<bool> IsSlotAvailableAsync(int doctorId, DateTime appointmentDate);
        Task CancelAppointmentAsync(int id);
    }

    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repository;

        public AppointmentService(IAppointmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
        {
            return await _repository.GetAllWithDetailsAsync();
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<bool> IsSlotAvailableAsync(int doctorId, DateTime appointmentDate)
        {
            var existingAppointments = await _repository.GetByDoctorAndDateAsync(doctorId, appointmentDate);
            return !existingAppointments.Any(a => a.Status != AppointmentStatus.Cancelled);
        }

        public async Task<bool> BookAppointmentAsync(Appointment appointment)
        {
            // Check if slot is available
            if (!await IsSlotAvailableAsync(appointment.DoctorId, appointment.AppointmentDate))
            {
                return false;
            }

            appointment.Status = AppointmentStatus.Booked;
            appointment.CreatedAt = DateTime.Now;

            await _repository.AddAsync(appointment);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task CancelAppointmentAsync(int id)
        {
            var appointment = await _repository.GetByIdAsync(id);
            if (appointment != null)
            {
                appointment.Status = AppointmentStatus.Cancelled;
                await _repository.UpdateAsync(appointment);
                await _repository.SaveChangesAsync();
            }
        }
    }
}
