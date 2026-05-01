using Microsoft.EntityFrameworkCore;
using FluffyBarkFriendsWebApp.Models.Database;

namespace FluffyBarkFriendsWebApp.Views.Repositories.Implementation
{
    public class UserRepository
    {
        private readonly FluffyBarkFriendsWebAppContext _context;

        public UserRepository(FluffyBarkFriendsWebAppContext context)
        {
            _context = context;
        }
        public Task<List<User>> GetActiveUsersAsync()
        {
                return _context.Users
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.FullName)
                .ToListAsync();
        }

        public Task<User?> FindByIdAsync(int userId)
        {
            return _context.Users
                .FirstOrDefaultAsync(x => x.UserId == userId && x.IsActive);
        }
        public Task<User?> FindByUsernameAsync(string username)
        {
            return _context.Users
                .FirstOrDefaultAsync(x => x.Username == username && x.IsActive);
        }

        public async Task CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUser(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            user.IsActive = false;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
}
}
