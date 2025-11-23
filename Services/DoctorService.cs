using DurdansRazor.Models;
using DurdansRazor.Repositories;

namespace DurdansRazor.Services
{
    public interface IDoctorService
    {
        Task<IEnumerable<Doctor>> GetAllDoctorsAsync();
        Task<Doctor?> GetDoctorByIdAsync(int id);
        Task<Doctor?> GetDoctorWithHospitalsAsync(int id);
        Task<IEnumerable<Doctor>> GetDoctorsBySpecializationAsync(string specialization);
        Task<IEnumerable<string>> GetSpecializationsAsync();
        Task<int> CreateDoctorAsync(Doctor doctor, List<int> hospitalIds);
        Task UpdateDoctorAsync(Doctor doctor, List<int> hospitalIds);
        Task DeleteDoctorAsync(int id);
    }

    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IHospitalRepository _hospitalRepository;

        public DoctorService(IDoctorRepository doctorRepository, IHospitalRepository hospitalRepository)
        {
            _doctorRepository = doctorRepository;
            _hospitalRepository = hospitalRepository;
        }

        public async Task<IEnumerable<Doctor>> GetAllDoctorsAsync()
        {
            return await _doctorRepository.GetAllAsync();
        }

        public async Task<Doctor?> GetDoctorByIdAsync(int id)
        {
            return await _doctorRepository.GetByIdAsync(id);
        }

        public async Task<Doctor?> GetDoctorWithHospitalsAsync(int id)
        {
            return await _doctorRepository.GetWithHospitalsAsync(id);
        }

        public async Task<IEnumerable<Doctor>> GetDoctorsBySpecializationAsync(string specialization)
        {
            return await _doctorRepository.GetBySpecializationAsync(specialization);
        }

        public async Task<IEnumerable<string>> GetSpecializationsAsync()
        {
            var doctors = await _doctorRepository.GetAllAsync();
            return doctors.Select(d => d.Specialization).Distinct().OrderBy(s => s);
        }

        public async Task<int> CreateDoctorAsync(Doctor doctor, List<int> hospitalIds)
        {
            // Load hospitals
            var hospitals = new List<Hospital>();
            foreach (var hospitalId in hospitalIds)
            {
                var hospital = await _hospitalRepository.GetByIdAsync(hospitalId);
                if (hospital != null)
                {
                    hospitals.Add(hospital);
                }
            }
            doctor.Hospitals = hospitals;

            await _doctorRepository.AddAsync(doctor);
            await _doctorRepository.SaveChangesAsync();
            return doctor.Id;
        }

        public async Task UpdateDoctorAsync(Doctor doctor, List<int> hospitalIds)
        {
            var existingDoctor = await _doctorRepository.GetWithHospitalsAsync(doctor.Id);
            if (existingDoctor != null)
            {
                existingDoctor.Name = doctor.Name;
                existingDoctor.Specialization = doctor.Specialization;
                existingDoctor.ConsultationFee = doctor.ConsultationFee;
                existingDoctor.AvailabilityJson = doctor.AvailabilityJson;

                // Update hospitals
                existingDoctor.Hospitals.Clear();
                foreach (var hospitalId in hospitalIds)
                {
                    var hospital = await _hospitalRepository.GetByIdAsync(hospitalId);
                    if (hospital != null)
                    {
                        existingDoctor.Hospitals.Add(hospital);
                    }
                }

                await _doctorRepository.UpdateAsync(existingDoctor);
                await _doctorRepository.SaveChangesAsync();
            }
        }

        public async Task DeleteDoctorAsync(int id)
        {
            await _doctorRepository.DeleteAsync(id);
            await _doctorRepository.SaveChangesAsync();
        }
    }
}
