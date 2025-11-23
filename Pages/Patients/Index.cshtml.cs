using DurdansRazor.Models;
using DurdansRazor.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DurdansRazor.Pages.Patients
{
    public class IndexModel : PageModel
    {
        private readonly IPatientService _patientService;

        public IndexModel(IPatientService patientService)
        {
            _patientService = patientService;
        }

        public IEnumerable<Patient> Patients { get; set; } = new List<Patient>();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public async Task OnGetAsync()
        {
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                Patients = await _patientService.SearchPatientsAsync(SearchTerm);
            }
            else
            {
                Patients = await _patientService.GetAllPatientsAsync();
            }
        }
    }
}
