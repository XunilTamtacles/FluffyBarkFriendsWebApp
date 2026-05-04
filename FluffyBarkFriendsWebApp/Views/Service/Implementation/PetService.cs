using FluffyBarkFriendsWebApp.Models.Database;
using FluffyBarkFriendsWebApp.Views.Repositories.Interface;
using FluffyBarkFriendsWebApp.Views.Service.Interface;

namespace FluffyBarkFriendsWebApp.Views.Service.Implementation
{
    public class PetService : IPetService
    {
        private readonly IPetRepository _petRepository;

        public PetService(IPetRepository petRepository)
        {
            _petRepository = petRepository;
        }

        public async Task<List<Pet>> GetAllAsync()
        {
            return await _petRepository.GetAllAsync();
        }

        public async Task<Pet?> GetByIdAsync(int id)
        {
            return await _petRepository.GetByIdAsync(id);
        }

        public async Task<List<Pet>> SearchAsync(string searchTerm)
        {
            return await _petRepository.SearchAsync(searchTerm);
        }

        public async Task CreateAsync(Pet pet)
        {
            CheckPet(pet);
            NormalizePet(pet);
            SetPetAge(pet);

            pet.IsActive = true;

            if (pet.CreatedAt == default)
            {
                pet.CreatedAt = DateTime.Now;
            }

            await _petRepository.AddAsync(pet);
        }

        public async Task UpdateAsync(Pet pet)
        {
            var existingPet = await _petRepository.GetByIdAsync(pet.PetId);

            if (existingPet == null)
            {
                throw new InvalidOperationException("Pet record was not found.");
            }

            CheckPet(pet);
            NormalizePet(pet);
            SetPetAge(pet);

            existingPet.PetName = pet.PetName;
            existingPet.Species = pet.Species;
            existingPet.Breed = pet.Breed;
            existingPet.Sex = pet.Sex;
            existingPet.BirthDate = pet.BirthDate;
            existingPet.AgeYears = pet.AgeYears;
            existingPet.Color = pet.Color;
            existingPet.Weight = pet.Weight;
            existingPet.IsActive = true;

            await _petRepository.UpdateAsync(existingPet);
        }

        public async Task DeleteAsync(int id)
        {
            var pet = await _petRepository.GetByIdAsync(id);

            if (pet == null)
            {
                return;
            }

            await _petRepository.DeleteAsync(pet);
        }

        private static void CheckPet(Pet pet)
        {
            if (string.IsNullOrWhiteSpace(pet.PetName))
            {
                throw new ArgumentException("Pet name is required.");
            }

            if (string.IsNullOrWhiteSpace(pet.Species))
            {
                throw new ArgumentException("Species is required.");
            }

            if (string.IsNullOrWhiteSpace(pet.Sex))
            {
                throw new ArgumentException("Sex is required.");
            }

            if (pet.BirthDate.HasValue && pet.BirthDate.Value > DateOnly.FromDateTime(DateTime.Today))
            {
                throw new ArgumentException("Birth date.");
            }

            if (pet.AgeYears.HasValue && pet.AgeYears.Value < 0)
            {
                throw new ArgumentException("Age.");
            }

            if (pet.Weight.HasValue && pet.Weight.Value < 0)
            {
                throw new ArgumentException("Weight.");
            }
        }

        private static void NormalizePet(Pet pet)
        {
            pet.PetName = pet.PetName.Trim();
            pet.Species = pet.Species.Trim();
            pet.Sex = pet.Sex.Trim();

            if (string.IsNullOrWhiteSpace(pet.Breed))
            {
                pet.Breed = null;
            }
            else
            {
                pet.Breed = pet.Breed.Trim();
            }

            if (string.IsNullOrWhiteSpace(pet.Color))
            {
                pet.Color = null;
            }
            else
            {
                pet.Color = pet.Color.Trim();
            }
        }

        private static void SetPetAge(Pet pet)
        {
            if (pet.BirthDate.HasValue)
            {
                var today = DateOnly.FromDateTime(DateTime.Today);
                int age = today.Year - pet.BirthDate.Value.Year;

                if (pet.BirthDate.Value > today.AddYears(-age))
                {
                    age--;
                }

                pet.AgeYears = age;
            }
        }
    }
}