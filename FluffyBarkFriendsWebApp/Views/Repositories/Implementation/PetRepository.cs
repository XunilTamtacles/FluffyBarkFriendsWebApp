using Microsoft.EntityFrameworkCore;
using FluffyBarkFriendsWebApp.Models.Database;
using FluffyBarkFriendsWebApp.Views.Repositories.Interface;

namespace FluffyBarkFriendsWebApp.Views.Repositories.Implementation
{
    public class PetRepository : IPetRepository
    {
        private readonly FluffyBarkFriendsWebAppContext _context;

        public PetRepository(FluffyBarkFriendsWebAppContext context)
        {
            _context = context;
        }

        public async Task<List<Pet>> GetAllAsync()
        {
            return await _context.Pets
                .Where(p => p.IsActive)
                .OrderBy(p => p.PetName)
                .ToListAsync();
        }

        public async Task<Pet?> GetByIdAsync(int id)
        {
            return await _context.Pets
                .FirstOrDefaultAsync(p => p.PetId == id && p.IsActive);
        }

        public async Task<List<Pet>> SearchAsync(string searchTerm)
        {
            var query = _context.Pets.Where(p => p.IsActive);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p =>
                    p.PetName.Contains(searchTerm) ||
                    p.Species.Contains(searchTerm) ||
                    (p.Breed != null && p.Breed.Contains(searchTerm))
                );
            }

            return await query
                .OrderBy(p => p.PetName)
                .ToListAsync();
        }

        public async Task AddAsync(Pet pet)
        {
            await _context.Pets.AddAsync(pet);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Pet pet)
        {
            _context.Pets.Update(pet);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Pet pet)
        {
            pet.IsActive = false; 
            _context.Pets.Update(pet);
            await _context.SaveChangesAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}