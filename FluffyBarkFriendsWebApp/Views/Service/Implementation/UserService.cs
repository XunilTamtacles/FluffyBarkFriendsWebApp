using FluffyBarkFriendsWebApp.Models.Database;
using FluffyBarkFriendsWebApp.Views.Repositories.Interface;
using FluffyBarkFriendsWebApp.Views.Service.Interface;

namespace FluffyBarkFriendsWebApp.Views.Service.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username is required.");
            }

            return await _userRepository.GetByUsernameAsync(username.Trim());
        }

        public async Task CreateAsync(User user)
        {
            CheckUser(user);
            NormalizeUser(user);

            var existingUser = await _userRepository.GetByUsernameAsync(user.UserName);

            if (existingUser != null)
            {
                throw new InvalidOperationException("Username is already taken.");
            }

            user.IsActive = true;

            if (user.CreatedAt == default)
            {
                user.CreatedAt = DateTime.Now;
            }

            await _userRepository.AddAsync(user);
        }

        public async Task UpdateAsync(User user)
        {
            var existingUser = await _userRepository.GetByIdAsync(user.UserId);

            if (existingUser == null)
            {
                throw new InvalidOperationException("User record was not found.");
            }

            CheckUser(user);
            NormalizeUser(user);

            var duplicateUser = await _userRepository.GetByUsernameAsync(user.UserName);

            if (duplicateUser != null && duplicateUser.UserId != user.UserId)
            {
                throw new InvalidOperationException("Username is already taken.");
            }

            existingUser.FullName = user.FullName;
            existingUser.UserName = user.UserName;
            existingUser.PasswordHash = user.PasswordHash;
            existingUser.Role = user.Role;
            existingUser.IsActive = true;

            await _userRepository.UpdateAsync(existingUser);
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return;
            }

            await _userRepository.DeleteAsync(user);
        }

        private static void CheckUser(User user)
        {
            if (string.IsNullOrWhiteSpace(user.FullName))
            {
                throw new ArgumentException("Full name is required.");
            }

            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                throw new ArgumentException("Username is required.");
            }

            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                throw new ArgumentException("Password is required.");
            }

            if (string.IsNullOrWhiteSpace(user.Role))
            {
                throw new ArgumentException("Role is required.");
            }
        }

        private static void NormalizeUser(User user)
        {
            user.FullName = user.FullName.Trim();
            user.UserName = user.UserName.Trim();
            user.PasswordHash = user.PasswordHash.Trim();
            user.Role = user.Role.Trim();
        }
    }
}