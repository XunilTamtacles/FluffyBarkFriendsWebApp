using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using FluffyBarkFriendsWebApp.Models.Database;
using FluffyBarkFriendsWebApp.Models.ViewModels;
using FluffyBarkFriendsWebApp.Views.Service.Interface;

namespace FluffyBarkFriendsWebApp.Controllers
{
    [Authorize(Roles = "Admin,Staff,Client")]
    public class MedicalHistoryController : Controller
    {
        private readonly IMedicalHistoryService _medicalHistoryService;
        private readonly IPetService _petService;

        public MedicalHistoryController(
            IMedicalHistoryService medicalHistoryService,
            IPetService petService)
        {
            _medicalHistoryService = medicalHistoryService;
            _petService = petService;
        }

        public async Task<IActionResult> Index()
        {
            var histories = await _medicalHistoryService.GetAllAsync();

            if (User.IsInRole("Client"))
            {
                int userId = GetCurrentUserId();

                histories = histories
                    .Where(h => h.IsSent &&
                                h.Pet != null &&
                                h.Pet.OwnerUserId == userId)
                    .ToList();
            }

            return View(histories);
        }

        public async Task<IActionResult> Details(int id)
        {
            var history = await _medicalHistoryService.GetByIdAsync(id);

            if (history == null)
                return NotFound();

            if (User.IsInRole("Client") &&
                (!history.IsSent ||
                 history.Pet == null ||
                 history.Pet.OwnerUserId != GetCurrentUserId()))
            {
                return Forbid();
            }

            return View(history);
        }

        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Create()
        {
            await LoadPetsAsync();

            return View(new MedicalHistoryFormsViewModel
            {
                VisitDate = DateOnly.FromDateTime(DateTime.Today),
                VisitTime = TimeOnly.FromDateTime(DateTime.Now)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Create(MedicalHistoryFormsViewModel model, string submitAction)
        {
            model.CreatedByUserId = GetCurrentUserId();

            if (model.PetId <= 0)
            {
                ModelState.AddModelError(nameof(model.PetId), "Please select a pet.");
            }

            if (!ModelState.IsValid)
            {
                await LoadPetsAsync();
                return View(model);
            }

            var history = MapToMedicalHistory(model);

            history.IsSent = submitAction == "send";

            await _medicalHistoryService.CreateAsync(history);

            TempData["SuccessMessage"] = history.IsSent
                ? "Medical record saved and sent to client."
                : "Medical record saved as draft.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> SendRecord(int id)
        {
            var history = await _medicalHistoryService.GetByIdAsync(id);

            if (history == null)
                return NotFound();

            history.IsSent = true;

            await _medicalHistoryService.UpdateAsync(history);

            TempData["SuccessMessage"] = "Medical record sent to client.";

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Edit(int id)
        {
            var history = await _medicalHistoryService.GetByIdAsync(id);

            if (history == null)
                return NotFound();

            await LoadPetsAsync();

            return View(MapToMedicalHistoryFormsViewModel(history));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Edit(int id, MedicalHistoryFormsViewModel model)
        {
            if (id != model.MedicalHistoryId)
                return NotFound();

            if (model.PetId <= 0)
            {
                ModelState.AddModelError(nameof(model.PetId), "Please select a pet.");
            }

            if (!ModelState.IsValid)
            {
                await LoadPetsAsync();
                return View(model);
            }

            var existingHistory = await _medicalHistoryService.GetByIdAsync(id);

            if (existingHistory == null)
                return NotFound();

            existingHistory.PetId = model.PetId;
            existingHistory.VisitDate = model.VisitDate;
            existingHistory.VisitTime = model.VisitTime;
            existingHistory.Condition = model.Condition ?? string.Empty;
            existingHistory.Diagnosis = model.Diagnosis ?? string.Empty;
            existingHistory.Treatment = model.Treatment ?? string.Empty;
            existingHistory.Dosage = model.Dosage ?? string.Empty;
            existingHistory.Notes = model.Notes ?? string.Empty;
            existingHistory.Medication = model.Medication ?? string.Empty;

            await _medicalHistoryService.UpdateAsync(existingHistory);

            TempData["SuccessMessage"] = "Medical history updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Delete(int id)
        {
            var history = await _medicalHistoryService.GetByIdAsync(id);

            if (history == null)
                return NotFound();

            return View(history);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _medicalHistoryService.DeleteAsync(id);

            TempData["SuccessMessage"] = "Medical history deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadPetsAsync()
        {
            var pets = await _petService.GetAllAsync();

            ViewBag.Pets = pets
                .Where(p => p.IsActive)
                .Select(p => new SelectListItem
                {
                    Value = p.PetId.ToString(),
                    Text = p.PetName + " - " +
                           (p.OwnerUser != null
                                ? p.OwnerUser.FullName
                                : (p.OwnerName ?? "No Owner"))
                })
                .ToList();
        }

        private int GetCurrentUserId()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out int userId))
                throw new InvalidOperationException("Logged-in user id was not found.");

            return userId;
        }

        private static MedicalHistory MapToMedicalHistory(MedicalHistoryFormsViewModel model)
        {
            return new MedicalHistory
            {
                MedicalHistoryId = model.MedicalHistoryId,
                PetId = model.PetId,
                VisitDate = model.VisitDate,
                VisitTime = model.VisitTime,
                Condition = model.Condition ?? string.Empty,
                Diagnosis = model.Diagnosis ?? string.Empty,
                Treatment = model.Treatment ?? string.Empty,
                Dosage = model.Dosage ?? string.Empty,
                Notes = model.Notes ?? string.Empty,
                Medication = model.Medication ?? string.Empty,
                CreatedByUserId = model.CreatedByUserId,
                CreatedAt = DateTime.Now,
                IsDeleted = false,
                IsSent = false
            };
        }

        private static MedicalHistoryFormsViewModel MapToMedicalHistoryFormsViewModel(MedicalHistory history)
        {
            return new MedicalHistoryFormsViewModel
            {
                MedicalHistoryId = history.MedicalHistoryId,
                PetId = history.PetId,
                VisitDate = history.VisitDate,
                VisitTime = history.VisitTime,
                Condition = history.Condition ?? string.Empty,
                Diagnosis = history.Diagnosis ?? string.Empty,
                Treatment = history.Treatment ?? string.Empty,
                Dosage = history.Dosage ?? string.Empty,
                Notes = history.Notes ?? string.Empty,
                Medication = history.Medication ?? string.Empty,
                CreatedByUserId = history.CreatedByUserId
            };
        }
    }
}