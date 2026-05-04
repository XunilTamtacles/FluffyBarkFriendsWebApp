using FluffyBarkFriendsWebApp.Models.Database;

namespace FluffyBarkFriendsWebApp.Views.Service.Interface
{
    public interface IUserService
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByUsernameAsync(string username);

        Task CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
    }
}