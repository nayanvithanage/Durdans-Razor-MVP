using DurdansRazor.Models;
using DurdansRazor.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DurdansRazor.Pages.Patients
{
    public class RegisterModel : PageModel
    {
        private readonly IPatientService _patientService;

        public RegisterModel(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [BindProperty]
        public Patient Patient { get; set; } = new Patient();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Check if phone number already exists
            var existingPatient = await _patientService.GetPatientByPhoneAsync(Patient.ContactNumber);
            if (existingPatient != null)
            {
                ModelState.AddModelError("Patient.ContactNumber", "A patient with this phone number already exists.");
                return Page();
            }

            await _patientService.RegisterPatientAsync(Patient);
            TempData["SuccessMessage"] = "Patient registered successfully!";
            return RedirectToPage("/Patients/Index");
        }
    }
}
