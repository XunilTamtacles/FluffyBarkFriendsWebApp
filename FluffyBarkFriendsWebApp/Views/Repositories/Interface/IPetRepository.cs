using FluffyBarkFriendsWebApp.Models.Database;

namespace FluffyBarkFriendsWebApp.Views.Repositories.Interface
{
    public interface IPetRepository
    {
        Task<List<Pet>> GetAllAsync();
        Task<Pet?> GetByIdAsync(int id);
        Task<List<Pet>> SearchAsync(string searchTerm);
        Task AddAsync(Pet pet);
        Task UpdateAsync(Pet pet);
        Task DeleteAsync(Pet pet);
        
        Task SaveAsync();
    }
}
