using DurdansRazor.Data;
using DurdansRazor.Models;
using Microsoft.EntityFrameworkCore;

namespace DurdansRazor.Repositories
{
    public interface IAppointmentRepository : IRepository<Appointment>
    {
        Task<IEnumerable<Appointment>> GetByDoctorAndDateAsync(int doctorId, DateTime date);
        Task<IEnumerable<Appointment>> GetAllWithDetailsAsync();
    }

    public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Appointment>> GetByDoctorAndDateAsync(int doctorId, DateTime date)
        {
            return await _dbSet
                .Where(a => a.DoctorId == doctorId && a.AppointmentDate.Date == date.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Include(a => a.Hospital)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }
    }
}
