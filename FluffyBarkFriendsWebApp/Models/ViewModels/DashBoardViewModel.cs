using FluffyBarkFriendsWebApp.Models.Database;

namespace FluffyBarkFriendsWebApp.Models.ViewModels
{
    public class DashBoardViewModel
    {
        public int ActivePetsCount { get; set; }

        public int TodayAppointmentsCount { get; set; }

        public int PendingAppointmentsCount { get; set; }

        public int ConfirmedAppointmentsCount { get; set; }

        public int CancelledAppointmentsCount { get; set; }

        public int CompletedAppointmentsCount { get; set; }

        public int UpcomingVaccinationsCount { get; set; }

        public int OverdueVaccinationsCount { get; set; }

        public int RecentMedicalCasesCount { get; set; }

        public List<Appointment> TodayAppointments { get; set; } = new();

        public List<Vaccination> UpcomingVaccinations { get; set; } = new();

        public List<Vaccination> OverdueVaccinations { get; set; } = new();
    }
}