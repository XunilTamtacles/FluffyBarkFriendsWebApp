using FluffyBarkFriendsWebApp.Models.Database;

namespace FluffyBarkFriendsWebApp.Views.Service.Interface
{
    public interface IMedicalHistoryService
    {
        Task<List<MedicalHistory>> GetAllAsync();
        Task<MedicalHistory?> GetByIdAsync(int id);

        Task<List<MedicalHistory>> GetByPetIdAsync(int petId);

        Task<List<MedicalHistory>> GetRecentCasesAsync(); 

        Task CreateAsync(MedicalHistory history);
        Task UpdateAsync(MedicalHistory history);
        Task DeleteAsync(int id);
    }
}