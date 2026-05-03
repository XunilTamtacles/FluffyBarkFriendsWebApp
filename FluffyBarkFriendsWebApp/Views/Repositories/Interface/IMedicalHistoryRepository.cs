using FluffyBarkFriendsWebApp.Models.Database;

namespace FluffyBarkFriendsWebApp.Views.Repositories.Interface
{
    public interface IMedicalHistoryRepository
    {
        Task<List<MedicalHistory>> GetAllAsync();
        Task<MedicalHistory?> GetByIdAsync(int id);
        Task<List<MedicalHistory>> GetByPetIdAsync(int petId);
        Task<List<MedicalHistory>> SearchByDiagnosisAsync(string keyword);
        Task<List<MedicalHistory>> GetByDateRangeAsync(DateOnly start, DateOnly end);
        Task<List<MedicalHistory>> GetRecentCasesAsync();
        Task AddAsync(MedicalHistory medicalHistory);
        Task UpdateAsync(MedicalHistory medicalHistory);
        Task DeleteAsync(MedicalHistory medicalHistory);
        Task SaveAsync();
    }
}
