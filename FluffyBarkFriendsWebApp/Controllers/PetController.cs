using System.Security.Claims;
using FluffyBarkFriendsWebApp.Models.Database;
using FluffyBarkFriendsWebApp.Models.ViewModels;
using FluffyBarkFriendsWebApp.Views.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FluffyBarkFriendsWebApp.Controllers
{
    [Authorize(Roles = "Admin,Staff,Client")]
    public class PetController : Controller
    {
        private readonly IPetService _petService;

        public PetController(IPetService petService)
        {
            _petService = petService;
        }

        public async Task<IActionResult> Index(string? searchTerm)
        {
            var pets = string.IsNullOrWhiteSpace(searchTerm)
                ? await _petService.GetAllAsync()
                : await _petService.SearchAsync(searchTerm);

            if (User.IsInRole("Client"))
            {
                int userId = GetCurrentUserId();

                pets = pets
                    .Where(p => p.OwnerUserId == userId)
                    .ToList();
            }

            return View(pets);
        }

        public async Task<IActionResult> Details(int id)
        {
            var pet = await _petService.GetByIdAsync(id);

            if (pet == null)
            {
                return NotFound();
            }

            if (!CanAccessPet(pet))
            {
                return Forbid();
            }

            return View(pet);
        }

        public IActionResult Create()
        {
            return View(new PetFormsViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PetFormsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var pet = MapToPet(model);

            if (User.IsInRole("Client"))
            {
                pet.OwnerUserId = GetCurrentUserId();
            }

            try
            {
                await _petService.CreateAsync(pet);

                TempData["SuccessMessage"] = "Pet successfully added.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var pet = await _petService.GetByIdAsync(id);

            if (pet == null)
            {
                return NotFound();
            }

            if (!CanAccessPet(pet))
            {
                return Forbid();
            }

            var model = MapToPetFormsViewModel(pet);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PetFormsViewModel model)
        {
            if (id != model.PetId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingPet = await _petService.GetByIdAsync(id);

            if (existingPet == null)
            {
                return NotFound();
            }

            if (!CanAccessPet(existingPet))
            {
                return Forbid();
            }

            existingPet.PetName = model.PetName;
            existingPet.Species = model.Species;
            existingPet.Breed = model.Breed;
            existingPet.Sex = model.Sex;
            existingPet.BirthDate = model.BirthDate;
            existingPet.AgeYears = model.AgeYears;
            existingPet.Color = model.Color;
            existingPet.Weight = model.Weight;
            existingPet.OwnerName = model.OwnerName;
            existingPet.ContactNumber = model.ContactNumber;
            existingPet.Notes = model.Notes;

            if (User.IsInRole("Client"))
            {
                existingPet.OwnerUserId = GetCurrentUserId();
            }

            try
            {
                await _petService.UpdateAsync(existingPet);

                TempData["SuccessMessage"] = "Pet successfully updated.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                return View(model);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var pet = await _petService.GetByIdAsync(id);

            if (pet == null)
            {
                return NotFound();
            }

            if (!CanAccessPet(pet))
            {
                return Forbid();
            }

            return View(pet);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pet = await _petService.GetByIdAsync(id);

            if (pet == null)
            {
                return NotFound();
            }

            if (!CanAccessPet(pet))
            {
                return Forbid();
            }

            await _petService.DeleteAsync(id);

            TempData["SuccessMessage"] = "Pet successfully deleted.";

            return RedirectToAction(nameof(Index));
        }

        private int GetCurrentUserId()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out int userId))
            {
                throw new InvalidOperationException("Logged-in user id was not found.");
            }

            return userId;
        }

        private bool CanAccessPet(Pet pet)
        {
            if (User.IsInRole("Admin") || User.IsInRole("Staff"))
            {
                return true;
            }

            if (User.IsInRole("Client"))
            {
                return pet.OwnerUserId == GetCurrentUserId();
            }

            return false;
        }

        private static Pet MapToPet(PetFormsViewModel model)
        {
            return new Pet
            {
                PetId = model.PetId,
                PetName = model.PetName,
                Species = model.Species,
                Breed = model.Breed,
                Sex = model.Sex,
                BirthDate = model.BirthDate,
                AgeYears = model.AgeYears,
                Color = model.Color,
                Weight = model.Weight,
                OwnerName = model.OwnerName,
                ContactNumber = model.ContactNumber,
                Notes = model.Notes,
                IsActive = true,
                CreatedAt = DateTime.Now
            };
        }

        private static PetFormsViewModel MapToPetFormsViewModel(Pet pet)
        {
            return new PetFormsViewModel
            {
                PetId = pet.PetId,
                PetName = pet.PetName,
                Species = pet.Species,
                Breed = pet.Breed,
                Sex = pet.Sex,
                BirthDate = pet.BirthDate,
                AgeYears = pet.AgeYears,
                Color = pet.Color,
                Weight = pet.Weight,
                OwnerName = pet.OwnerName,
                ContactNumber = pet.ContactNumber,
                Notes = pet.Notes
            };
        }
    }
}
