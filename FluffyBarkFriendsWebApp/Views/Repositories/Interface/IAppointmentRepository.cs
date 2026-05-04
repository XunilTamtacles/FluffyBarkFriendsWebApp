using FluffyBarkFriendsWebApp.Models.Database;

namespace FluffyBarkFriendsWebApp.Views.Repositories.Interface
{
    public interface IAppointmentRepository
    {
        Task<List<Appointment>> GetAllAsync();

        Task<Appointment?> GetByIdAsync(int id);

        Task<List<Appointment>> GetByPetIdAsync(int petId);

        Task<List<Appointment>> GetByStatusAsync(string status);

        Task<List<Appointment>> GetByDateAsync(DateOnly date);

        Task AddAsync(Appointment appointment);

        Task UpdateAsync(Appointment appointment);

        Task DeleteAsync(Appointment appointment);

        Task SaveAsync();
    }
}