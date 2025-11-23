using DurdansRazor.Models;
using DurdansRazor.Repositories;

namespace DurdansRazor.Services
{
    public interface IHospitalService
    {
        Task<IEnumerable<Hospital>> GetAllHospitalsAsync();
        Task<Hospital?> GetHospitalByIdAsync(int id);
        Task<int> CreateHospitalAsync(Hospital hospital);
        Task UpdateHospitalAsync(Hospital hospital);
        Task DeleteHospitalAsync(int id);
    }

    public class HospitalService : IHospitalService
    {
        private readonly IHospitalRepository _repository;

        public HospitalService(IHospitalRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Hospital>> GetAllHospitalsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Hospital?> GetHospitalByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<int> CreateHospitalAsync(Hospital hospital)
        {
            await _repository.AddAsync(hospital);
            await _repository.SaveChangesAsync();
            return hospital.Id;
        }

        public async Task UpdateHospitalAsync(Hospital hospital)
        {
            await _repository.UpdateAsync(hospital);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteHospitalAsync(int id)
        {
            await _repository.DeleteAsync(id);
            await _repository.SaveChangesAsync();
        }
    }
}
