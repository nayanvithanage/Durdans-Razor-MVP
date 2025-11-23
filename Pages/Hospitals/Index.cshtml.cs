using DurdansRazor.Models;
using DurdansRazor.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DurdansRazor.Pages.Hospitals
{
    public class IndexModel : PageModel
    {
        private readonly IHospitalService _hospitalService;

        public IndexModel(IHospitalService hospitalService)
        {
            _hospitalService = hospitalService;
        }

        public IEnumerable<Hospital> Hospitals { get; set; } = new List<Hospital>();

        public async Task OnGetAsync()
        {
            Hospitals = await _hospitalService.GetAllHospitalsAsync();
        }
    }
}
