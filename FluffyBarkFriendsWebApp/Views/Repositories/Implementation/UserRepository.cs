using Microsoft.EntityFrameworkCore;
using FluffyBarkFriendsWebApp.Models.Database;
using FluffyBarkFriendsWebApp.Views.Repositories.Interface;

namespace FluffyBarkFriendsWebApp.Views.Repositories.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly FluffyBarkFriendsWebAppContext _context;

        public UserRepository(FluffyBarkFriendsWebAppContext context)
        {
            _context = context;
        }

        public Task<List<User>> GetAllAsync()
        {
            return _context.Users
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.FullName)
                .ToListAsync();
        }

        public Task<User?> GetByIdAsync(int id)
        {
            return _context.Users
                .FirstOrDefaultAsync(x => x.UserId == id && x.IsActive);
        }

        public Task<User?> GetByUsernameAsync(string username)
        {
            return _context.Users
                .FirstOrDefaultAsync(x => x.UserName == username && x.IsActive);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
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