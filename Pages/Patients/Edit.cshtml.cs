using DurdansRazor.Models;
using DurdansRazor.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DurdansRazor.Pages.Patients
{
    public class EditModel : PageModel
    {
        private readonly IPatientService _patientService;

        public EditModel(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [BindProperty]
        public Patient Patient { get; set; } = new Patient();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            Patient = patient;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _patientService.UpdatePatientAsync(Patient);
            TempData["SuccessMessage"] = "Patient updated successfully!";
            return RedirectToPage("/Patients/Index");
        }
    }
}
