using FluffyBarkFriendsWebApp.Models.Database;

namespace FluffyBarkFriendsWebApp.Views.Service.Interface
{
    public interface IVaccinationService
    {
        Task<List<Vaccination>> GetAllAsync();
        Task<Vaccination?> GetByIdAsync(int id);
        Task<List<Vaccination>> GetByPetIdAsync(int petId);
        Task<List<Vaccination>> GetByAppointmentIdAsync(int appointmentId);
        Task<List<Vaccination>> GetUpcomingAsync();
        Task<List<Vaccination>> GetOverdueAsync();

        Task CreateAsync(Vaccination vaccination);
        Task UpdateAsync(Vaccination vaccination);
        Task DeleteAsync(int id);
    }
}