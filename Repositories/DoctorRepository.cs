using DurdansRazor.Data;
using DurdansRazor.Models;
using Microsoft.EntityFrameworkCore;

namespace DurdansRazor.Repositories
{
    public interface IDoctorRepository : IRepository<Doctor>
    {
        Task<IEnumerable<Doctor>> GetBySpecializationAsync(string specialization);
        Task<Doctor?> GetWithHospitalsAsync(int id);
    }

    public class DoctorRepository : Repository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Doctor>> GetBySpecializationAsync(string specialization)
        {
            return await _dbSet
                .Where(d => d.Specialization == specialization)
                .ToListAsync();
        }

        public async Task<Doctor?> GetWithHospitalsAsync(int id)
        {
            return await _dbSet
                .Include(d => d.Hospitals)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public override async Task<IEnumerable<Doctor>> GetAllAsync()
        {
            return await _dbSet
                .Include(d => d.Hospitals)
                .ToListAsync();
        }
    }
}
