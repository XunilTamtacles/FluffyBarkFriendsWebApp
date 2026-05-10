using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluffyBarkFriendsWebApp.Models.Database;
using FluffyBarkFriendsWebApp.Models.ViewModels;
using FluffyBarkFriendsWebApp.Views.Service.Interface;

namespace FluffyBarkFriendsWebApp.Controllers
{
    [Authorize(Roles = "Admin,Staff,Client")]
    public class VaccinationController : Controller
    {
        private readonly IVaccinationService _vaccinationService;

        public VaccinationController(IVaccinationService vaccinationService)
        {
            _vaccinationService = vaccinationService;
        }

        public async Task<IActionResult> Index()
        {
            var vaccinations = await _vaccinationService.GetAllAsync();
            return View(vaccinations);
        }

        public async Task<IActionResult> Details(int id)
        {
            var vaccination = await _vaccinationService.GetByIdAsync(id);

            if (vaccination == null)
            {
                return NotFound();
            }

            return View(vaccination);
        }

        [Authorize(Roles = "Admin,Staff")]
        public IActionResult Create()
        {
            var model = new VaccinationRecordFormsViewModel
            {
                DateGiven = DateOnly.FromDateTime(DateTime.Today),
                NextDueDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1))
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Create(VaccinationRecordFormsViewModel model)
        {
            if (model.NextDueDate.HasValue &&
                model.NextDueDate.Value < model.DateGiven)
            {
                ModelState.AddModelError(
                    "NextDueDate",
                    "Next due date cannot be earlier than date given.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var vaccination = new Vaccination
            {
                PetId = model.PetId,
                AppointmentId = (int)model.AppointmentId,
                VaccineName = model.VaccineName,
                DateGiven = model.DateGiven,
                NextDueDate = model.NextDueDate,
                Dose = model.Dose,
                Remarks = model.Remarks,
                RecordedByUserId = model.RecordedByUserId,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            await _vaccinationService.CreateAsync(vaccination);

            TempData["SuccessMessage"] = "Vaccination record successfully added.";

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Edit(int id)
        {
            var vaccination = await _vaccinationService.GetByIdAsync(id);

            if (vaccination == null)
            {
                return NotFound();
            }

            var model = new VaccinationRecordFormsViewModel
            {
                VaccinationId = vaccination.VaccinationId,
                PetId = vaccination.PetId,
                AppointmentId = vaccination.AppointmentId,
                VaccineName = vaccination.VaccineName,
                DateGiven = vaccination.DateGiven,
                NextDueDate = vaccination.NextDueDate,
                Dose = vaccination.Dose,
                Remarks = vaccination.Remarks,
                RecordedByUserId = vaccination.RecordedByUserId
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Edit(int id, VaccinationRecordFormsViewModel model)
        {
            if (id != model.VaccinationId)
            {
                return NotFound();
            }

            if (model.NextDueDate.HasValue &&
                model.NextDueDate.Value < model.DateGiven)
            {
                ModelState.AddModelError(
                    "NextDueDate",
                    "Next due date cannot be earlier than date given.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var vaccination = await _vaccinationService.GetByIdAsync(id);

            if (vaccination == null)
            {
                return NotFound();
            }

            vaccination.PetId = model.PetId;
            vaccination.AppointmentId = (int)model.AppointmentId;
            vaccination.VaccineName = model.VaccineName;
            vaccination.DateGiven = model.DateGiven;
            vaccination.NextDueDate = model.NextDueDate;
            vaccination.Dose = model.Dose;
            vaccination.Remarks = model.Remarks;
            vaccination.RecordedByUserId = model.RecordedByUserId;

            await _vaccinationService.UpdateAsync(vaccination);

            TempData["SuccessMessage"] = "Vaccination record successfully updated.";

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Delete(int id)
        {
            var vaccination = await _vaccinationService.GetByIdAsync(id);

            if (vaccination == null)
            {
                return NotFound();
            }

            return View(vaccination);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vaccination = await _vaccinationService.GetByIdAsync(id);

            if (vaccination == null)
            {
                return NotFound();
            }

            await _vaccinationService.DeleteAsync(id);

            TempData["SuccessMessage"] = "Vaccination record successfully deleted.";

            return RedirectToAction(nameof(Index));
        }
    }
}