using DurdansRazor.Models;
using DurdansRazor.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DurdansRazor.Pages.Appointments
{
    public class IndexModel : PageModel
    {
        private readonly IAppointmentService _appointmentService;

        public IndexModel(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        public IEnumerable<Appointment> Appointments { get; set; } = new List<Appointment>();

        public async Task OnGetAsync()
        {
            Appointments = await _appointmentService.GetAllAppointmentsAsync();
        }
    }
}
