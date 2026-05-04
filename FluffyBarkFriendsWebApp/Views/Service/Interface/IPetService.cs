using FluffyBarkFriendsWebApp.Models.Database;

namespace FluffyBarkFriendsWebApp.Views.Service.Interface
{
    public interface IPetService
    {
        Task<List<Pet>> GetAllAsync();
        Task<Pet?> GetByIdAsync(int id);
        Task<List<Pet>> SearchAsync(string searchTerm);

        Task CreateAsync(Pet pet);
        Task UpdateAsync(Pet pet);
        Task DeleteAsync(int id);
    }
}