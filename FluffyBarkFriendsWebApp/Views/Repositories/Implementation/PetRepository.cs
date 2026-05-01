using Microsoft.EntityFrameworkCore;
using FluffyBarkFriendsWebApp.Models.Database;

namespace FluffyBarkFriendsWebApp.Views.Repositories.Implementation
{
    public class PetRepository
    {
        private readonly FluffyBarkFriendsWebAppContext _context;

        public PetRepository(FluffyBarkFriendsWebAppContext context)
        {
            _context = context;
        }
        public async Task<List<Pet>> GetAllAsync()
        {
            var pets = await _context.Pets.ToListAsync();

            return pets
                .Where(p => p.IsActive)
                .OrderBy(p => p.PetName)
                .ToList();
        }

        public async Task<Pet?> GetByIdAsync(int id)
        {
            var pet = await _context.Pets.FindAsync(id);

            if (pet == null || !pet.IsActive)
                return null;

            return pet;
        }

        public async Task<List<Pet>> SearchAsync(string searchTerm)
        {
            var pets = await _context.Pets.ToListAsync();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return pets
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.PetName)
                    .ToList();
            }

            return pets
                .Where(p =>
                    p.IsActive &&
                    (
                        p.PetName.Contains(searchTerm) ||
                        p.Species.Contains(searchTerm) ||
                        p.Breed != null && p.Breed.Contains(searchTerm)
                    )
                )
                .OrderBy(p => p.PetName)
                .ToList();
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
    }
}

