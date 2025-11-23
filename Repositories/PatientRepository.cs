using DurdansRazor.Data;
using DurdansRazor.Models;
using Microsoft.EntityFrameworkCore;

namespace DurdansRazor.Repositories
{
    public interface IPatientRepository : IRepository<Patient>
    {
        Task<Patient?> GetByPhoneAsync(string phone);
        Task<IEnumerable<Patient>> SearchByNameAsync(string name);
    }

    public class PatientRepository : Repository<Patient>, IPatientRepository
    {
        public PatientRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Patient?> GetByPhoneAsync(string phone)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.ContactNumber == phone);
        }

        public async Task<IEnumerable<Patient>> SearchByNameAsync(string name)
        {
            return await _dbSet
                .Where(p => p.Name.Contains(name))
                .ToListAsync();
        }
    }
}
