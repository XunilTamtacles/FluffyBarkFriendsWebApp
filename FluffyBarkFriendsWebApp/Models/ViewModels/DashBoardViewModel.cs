using System.ComponentModel.DataAnnotations;
using FluffyBarkFriendsWebApp.Models.Database;

namespace FluffyBarkFriendsWebApp.Models.ViewModels
{
    public class DashBoardViewModel
    {
        [Display(Name = "Active Pets")]
        public int ActivePetsCount { get; set; }

        [Display(Name = "Today Appointments")]
        public int TodayAppointmentsCount { get; set; }

        [Display(Name = "Pending Appointments")]
        public int PendingAppointmentsCount { get; set; }

        [Display(Name = "Confirmed Appointments")]
        public int ConfirmedAppointmentsCount { get; set; }

        [Display(Name = "Cancelled Appointments")]
        public int CancelledAppointmentsCount { get; set; }

        [Display(Name = "Completed Appointments")]
        public int CompletedAppointmentsCount { get; set; }

        [Display(Name = "Upcoming Vaccinations")]
        public int UpcomingVaccinationsCount { get; set; }

        [Display(Name = "Overdue Vaccinations")]
        public int OverdueVaccinationsCount { get; set; }

        [Display(Name = "Recent Medical Cases")]
        public int RecentMedicalCasesCount { get; set; }

        public List<Appointment> TodayAppointments { get; set; } = new List<Appointment>();

        public List<Vaccination> UpcomingVaccinations { get; set; } = new List<Vaccination>();

        public List<Vaccination> OverdueVaccinations { get; set; } = new List<Vaccination>();

        public List<MedicalHistory> RecentMedicalCases { get; set; } = new List<MedicalHistory>();
    }
}