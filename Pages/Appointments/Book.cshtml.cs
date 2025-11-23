using DurdansRazor.Models;
using DurdansRazor.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DurdansRazor.Pages.Appointments
{
    public class BookModel : PageModel
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IPatientService _patientService;
        private readonly IDoctorService _doctorService;
        private readonly IHospitalService _hospitalService;

        public BookModel(
            IAppointmentService appointmentService,
            IPatientService patientService,
            IDoctorService doctorService,
            IHospitalService hospitalService)
        {
            _appointmentService = appointmentService;
            _patientService = patientService;
            _doctorService = doctorService;
            _hospitalService = hospitalService;
        }

        [BindProperty]
        public Appointment Appointment { get; set; } = new Appointment();

        public SelectList PatientSelectList { get; set; }
        public SelectList SpecializationSelectList { get; set; }
        public SelectList HospitalSelectList { get; set; }

        public async Task OnGetAsync()
        {
            await LoadDropdownsAsync();
        }

        public async Task<JsonResult> OnGetDoctorsAsync(string specialization)
        {
            var doctors = await _doctorService.GetDoctorsBySpecializationAsync(specialization);
            return new JsonResult(doctors.Select(d => new
            {
                id = d.Id,
                name = d.Name,
                consultationFee = d.ConsultationFee
            }));
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return Page();
            }

            // Validate appointment date is in the future
            if (Appointment.AppointmentDate <= DateTime.Now)
            {
                ModelState.AddModelError("Appointment.AppointmentDate", "Appointment date must be in the future.");
                await LoadDropdownsAsync();
                return Page();
            }

            // Try to book the appointment
            var success = await _appointmentService.BookAppointmentAsync(Appointment);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "This time slot is already booked. Please select a different date/time.");
                await LoadDropdownsAsync();
                return Page();
            }

            TempData["SuccessMessage"] = "Appointment booked successfully!";
            return RedirectToPage("/Appointments/Index");
        }

        private async Task LoadDropdownsAsync()
        {
            var patients = await _patientService.GetAllPatientsAsync();
            PatientSelectList = new SelectList(patients, "Id", "Name");

            var specializations = await _doctorService.GetSpecializationsAsync();
            SpecializationSelectList = new SelectList(specializations);

            var hospitals = await _hospitalService.GetAllHospitalsAsync();
            HospitalSelectList = new SelectList(hospitals, "Id", "Name");
        }
    }
}
