using FluffyBarkFriendsWebApp.Models.Database;
using FluffyBarkFriendsWebApp.Models.ViewModels;
using FluffyBarkFriendsWebApp.Views.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FluffyBarkFriendsWebApp.Controllers
{
    public class MedicalHistoryController : Controller
    {
        private readonly IMedicalHistoryService _medicalHistoryService;

        public MedicalHistoryController(IMedicalHistoryService medicalHistoryService)
        {
            _medicalHistoryService = medicalHistoryService;
        }

        public async Task<IActionResult> Index()
        {
            var histories = await _medicalHistoryService.GetAllAsync();
            return View(histories);
        }

        public async Task<IActionResult> Details(int id)
        {
            var history = await _medicalHistoryService.GetByIdAsync(id);

            if (history == null)
            {
                return NotFound();
            }

            return View(history);
        }

        public IActionResult Create()
        {
            var model = new MedicalHistoryFormsViewModel
            {
                VisitDate = DateOnly.FromDateTime(DateTime.Today),
                VisitTime = TimeOnly.FromDateTime(DateTime.Now)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicalHistoryFormsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var history = MapToMedicalHistory(model);

            try
            {
                await _medicalHistoryService.CreateAsync(history);
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
            var history = await _medicalHistoryService.GetByIdAsync(id);

            if (history == null)
            {
                return NotFound();
            }

            var model = MapToMedicalHistoryFormsViewModel(history);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MedicalHistoryFormsViewModel model)
        {
            if (id != model.MedicalHistoryId)
            {
                return NotFound();
            }

            var existingHistory = await _medicalHistoryService.GetByIdAsync(id);

            if (existingHistory == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var history = MapToMedicalHistory(model);

            try
            {
                await _medicalHistoryService.UpdateAsync(history);
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
            var history = await _medicalHistoryService.GetByIdAsync(id);

            if (history == null)
            {
                return NotFound();
            }

            return View(history);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var history = await _medicalHistoryService.GetByIdAsync(id);

            if (history == null)
            {
                return NotFound();
            }

            await _medicalHistoryService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
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
                CreatedByUserId = model.CreatedByUserId
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
