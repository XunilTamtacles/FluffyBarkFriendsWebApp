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

            return View(pets);
        }



        public async Task<IActionResult> Details(int id)
        {
            var pet = await _petService.GetByIdAsync(id);

            if (pet == null)
            {
                return NotFound();
            }

            return View(pet);
        }


        [Authorize(Roles = "Admin,Staff")]
        public IActionResult Create()
        {
            return View(new PetFormsViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Create(PetFormsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var pet = MapToPet(model);

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


        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Edit(int id)
        {
            var pet = await _petService.GetByIdAsync(id);

            if (pet == null)
            {
                return NotFound();
            }

            var model = MapToPetFormsViewModel(pet);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
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



        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Delete(int id)
        {
            var pet = await _petService.GetByIdAsync(id);

            if (pet == null)
            {
                return NotFound();
            }

            return View(pet);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pet = await _petService.GetByIdAsync(id);

            if (pet == null)
            {
                return NotFound();
            }

            await _petService.DeleteAsync(id);

            TempData["SuccessMessage"] = "Pet successfully deleted.";

            return RedirectToAction(nameof(Index));
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