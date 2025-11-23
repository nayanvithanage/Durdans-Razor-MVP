using DurdansRazor.Models;
using DurdansRazor.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DurdansRazor.Pages.Doctors
{
    public class CreateModel : PageModel
    {
        private readonly IDoctorService _doctorService;
        private readonly IHospitalService _hospitalService;

        public CreateModel(IDoctorService doctorService, IHospitalService hospitalService)
        {
            _doctorService = doctorService;
            _hospitalService = hospitalService;
        }

        [BindProperty]
        public Doctor Doctor { get; set; } = new Doctor();

        [BindProperty]
        public List<int> SelectedHospitalIds { get; set; } = new List<int>();

        public IEnumerable<Hospital> Hospitals { get; set; } = new List<Hospital>();

        public async Task OnGetAsync()
        {
            Hospitals = await _hospitalService.GetAllHospitalsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Hospitals = await _hospitalService.GetAllHospitalsAsync();
                return Page();
            }

            await _doctorService.CreateDoctorAsync(Doctor, SelectedHospitalIds);
            TempData["SuccessMessage"] = "Doctor registered successfully!";
            return RedirectToPage("/Doctors/Index");
        }
    }
}
