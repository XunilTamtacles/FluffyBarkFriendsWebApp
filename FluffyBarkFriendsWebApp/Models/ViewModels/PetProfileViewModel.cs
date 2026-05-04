using FluffyBarkFriendsWebApp.Models.Database;

namespace FluffyBarkFriendsWebApp.Models.ViewModels
{
    public class PetProfileViewModel
    {
        public Pet? Pet { get; set; } = null;

        public List<Appointment> Appointments { get; set; } = new();

        public List<Vaccination> Vaccinations { get; set; } = new();

        public List<MedicalHistory> MedicalHistories { get; set; } = new();
    }
}
