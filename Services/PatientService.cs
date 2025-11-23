using DurdansRazor.Models;
using DurdansRazor.Repositories;

namespace DurdansRazor.Services
{
    public interface IPatientService
    {
        Task<IEnumerable<Patient>> GetAllPatientsAsync();
        Task<Patient?> GetPatientByIdAsync(int id);
        Task<Patient?> GetPatientByPhoneAsync(string phone);
        Task<IEnumerable<Patient>> SearchPatientsAsync(string searchTerm);
        Task<int> RegisterPatientAsync(Patient patient);
        Task UpdatePatientAsync(Patient patient);
        Task DeletePatientAsync(int id);
    }

    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _repository;

        public PatientService(IPatientRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Patient?> GetPatientByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Patient?> GetPatientByPhoneAsync(string phone)
        {
            return await _repository.GetByPhoneAsync(phone);
        }

        public async Task<IEnumerable<Patient>> SearchPatientsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllPatientsAsync();

            return await _repository.SearchByNameAsync(searchTerm);
        }

        public async Task<int> RegisterPatientAsync(Patient patient)
        {
            await _repository.AddAsync(patient);
            await _repository.SaveChangesAsync();
            return patient.Id;
        }

        public async Task UpdatePatientAsync(Patient patient)
        {
            await _repository.UpdateAsync(patient);
            await _repository.SaveChangesAsync();
        }

        public async Task DeletePatientAsync(int id)
        {
            await _repository.DeleteAsync(id);
            await _repository.SaveChangesAsync();
        }
    }
}
