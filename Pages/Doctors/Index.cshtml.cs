using DurdansRazor.Models;
using DurdansRazor.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DurdansRazor.Pages.Doctors
{
    public class IndexModel : PageModel
    {
        private readonly IDoctorService _doctorService;

        public IndexModel(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        public IEnumerable<Doctor> Doctors { get; set; } = new List<Doctor>();

        public async Task OnGetAsync()
        {
            Doctors = await _doctorService.GetAllDoctorsAsync();
        }
    }
}
