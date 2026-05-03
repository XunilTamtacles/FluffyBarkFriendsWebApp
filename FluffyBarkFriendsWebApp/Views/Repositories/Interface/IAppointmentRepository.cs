using FluffyBarkFriendsWebApp.Models.Database;
using System.ComponentModel;

namespace FluffyBarkFriendsWebApp.Views.Repositories.Interface
{
    public interface IAppointmentRepository
    {
        Task<List<Appointment>> GetAllAsync();

        Task<List<Appointment>> GetIdAsync(int id);
        
        Task<List<Appointment>> GetByPetIdAsync(int petId);

        Task<List<Appointment>> GetByStatusAsync(string Status);

        Task<List<Appointment>> GetByDateAsync(DateOnly date);

        Task AddAsync(Appointment appointment);

        void UpdateAsync(Appointment appointment);

        void Delete(Appointment appointment);

        Task SaveAsync();


    }
}
