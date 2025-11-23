using DurdansRazor.Models;
using DurdansRazor.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DurdansRazor.Pages.Hospitals
{
    public class CreateModel : PageModel
    {
        private readonly IHospitalService _hospitalService;

        public CreateModel(IHospitalService hospitalService)
        {
            _hospitalService = hospitalService;
        }

        [BindProperty]
        public Hospital Hospital { get; set; } = new Hospital();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _hospitalService.CreateHospitalAsync(Hospital);
            TempData["SuccessMessage"] = "Hospital created successfully!";
            return RedirectToPage("/Hospitals/Index");
        }
    }
}
