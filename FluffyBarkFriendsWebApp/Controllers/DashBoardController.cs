using Microsoft.AspNetCore.Mvc;
using FluffyBarkFriendsWebApp.Models.ViewModels;
using FluffyBarkFriendsWebApp.Views.Service.Interface;

namespace FluffyBarkFriendsWebApp.Controllers
{
    public class DashBoardController : Controller
    {
        private readonly IPetService _petService;
        private readonly IAppointmentService _appointmentService;
        private readonly IVaccinationService _vaccinationService;
        private readonly IMedicalHistoryService _medicalHistoryService;

        public DashBoardController(
            IPetService petService,
            IAppointmentService appointmentService,
            IVaccinationService vaccinationService,
            IMedicalHistoryService medicalHistoryService)
        {
            _petService = petService;
            _appointmentService = appointmentService;
            _vaccinationService = vaccinationService;
            _medicalHistoryService = medicalHistoryService;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var pets = await _petService.GetAllAsync();
            var appointments = await _appointmentService.GetAllAsync();
            var upcomingVaccinations = await _vaccinationService.GetUpcomingAsync();
            var overdueVaccinations = await _vaccinationService.GetOverdueAsync();
            var recentMedicalCases = await _medicalHistoryService.GetRecentCasesAsync();

            var model = new DashBoardViewModel
            {
                ActivePetsCount = pets.Count,
                TodayAppointmentsCount = appointments.Count(a => a.AppointmentDate == today),

                PendingAppointmentsCount = appointments.Count(a => a.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase)),
                ConfirmedAppointmentsCount = appointments.Count(a => a.Status.Equals("Confirmed", StringComparison.OrdinalIgnoreCase)),
                CancelledAppointmentsCount = appointments.Count(a => a.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase)),
                CompletedAppointmentsCount = appointments.Count(a => a.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase)),

                UpcomingVaccinationsCount = upcomingVaccinations.Count,
                OverdueVaccinationsCount = overdueVaccinations.Count,
                RecentMedicalCasesCount = recentMedicalCases.Count,

                TodayAppointments = appointments
                    .Where(a => a.AppointmentDate == today)
                    .OrderBy(a => a.AppointmentTime)
                    .Take(6)
                    .ToList(),

                UpcomingVaccinations = upcomingVaccinations
                    .Take(6)
                    .ToList(),

                OverdueVaccinations = overdueVaccinations
                    .Take(6)
                    .ToList(),

                RecentMedicalCases = recentMedicalCases
                    .Take(6)
                    .ToList()
            };

            return View(model);
        }
    }
}