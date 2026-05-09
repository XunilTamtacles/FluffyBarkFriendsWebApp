using FluffyBarkFriendsWebApp.Models.Database;

namespace FluffyBarkFriendsWebApp.Models.ViewModels
{
    public class ClientPortalViewModel
    {
        public int PetsCount { get; set; }
        public int AppointmentsCount { get; set; }
        public int MedicalHistoryCount { get; set; }
        public int VaccinationsCount { get; set; }

        public List<Pet> Pets { get; set; } = new List<Pet>();
        public List<Appointment> Appointments { get; set; } = new List<Appointment>();
        public List<MedicalHistory> MedicalHistories { get; set; } = new List<MedicalHistory>();
        public List<Vaccination> Vaccinations { get; set; } = new List<Vaccination>();
    }
}