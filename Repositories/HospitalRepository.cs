using DurdansRazor.Data;
using DurdansRazor.Models;

namespace DurdansRazor.Repositories
{
    public interface IHospitalRepository : IRepository<Hospital>
    {
    }

    public class HospitalRepository : Repository<Hospital>, IHospitalRepository
    {
        public HospitalRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
