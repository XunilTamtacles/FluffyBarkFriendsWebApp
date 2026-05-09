using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluffyBarkFriendsWebApp.Models.Database;
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
            var vaccination = new Vaccination
            {
                DateGiven = DateOnly.FromDateTime(DateTime.Today),
                NextDueDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1))
            };

            return View(vaccination);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Create(Vaccination vaccination)
        {
            if (!ModelState.IsValid)
            {
                return View(vaccination);
            }

            if (vaccination.NextDueDate.HasValue &&
                vaccination.NextDueDate.Value < vaccination.DateGiven)
            {
                ModelState.AddModelError(
                    "NextDueDate",
                    "Next due date cannot be earlier than date given.");

                return View(vaccination);
            }

            try
            {
                await _vaccinationService.CreateAsync(vaccination);

                TempData["SuccessMessage"] = "Vaccination record successfully added.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                return View(vaccination);
            }
        }

   

        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Edit(int id)
        {
            var vaccination = await _vaccinationService.GetByIdAsync(id);

            if (vaccination == null)
            {
                return NotFound();
            }

            return View(vaccination);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Edit(int id, Vaccination vaccination)
        {
            if (id != vaccination.VaccinationId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(vaccination);
            }

            if (vaccination.NextDueDate.HasValue &&
                vaccination.NextDueDate.Value < vaccination.DateGiven)
            {
                ModelState.AddModelError(
                    "NextDueDate",
                    "Next due date cannot be earlier than date given.");

                return View(vaccination);
            }

            var existingVaccination = await _vaccinationService.GetByIdAsync(id);

            if (existingVaccination == null)
            {
                return NotFound();
            }

            existingVaccination.PetId = vaccination.PetId;
            existingVaccination.AppointmentId = vaccination.AppointmentId;
            existingVaccination.VaccineName = vaccination.VaccineName;
            existingVaccination.DateGiven = vaccination.DateGiven;
            existingVaccination.NextDueDate = vaccination.NextDueDate;
            existingVaccination.Dose = vaccination.Dose;
            existingVaccination.Remarks = vaccination.Remarks;
            existingVaccination.RecordedByUserId = vaccination.RecordedByUserId;

            try
            {
                await _vaccinationService.UpdateAsync(existingVaccination);

                TempData["SuccessMessage"] = "Vaccination record successfully updated.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                return View(vaccination);
            }
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