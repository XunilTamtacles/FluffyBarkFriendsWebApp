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
    public class VaccinationController : Controller
    {
        private readonly IVaccinationService _vaccinationService;
        private readonly IPetService _petService;
        private readonly IAppointmentService _appointmentService;

        public VaccinationController(
            IVaccinationService vaccinationService,
            IPetService petService,
            IAppointmentService appointmentService)
        {
            _vaccinationService = vaccinationService;
            _petService = petService;
            _appointmentService = appointmentService;
        }

        public async Task<IActionResult> Index()
        {
            var vaccinations = await _vaccinationService.GetAllAsync();

            if (User.IsInRole("Client"))
            {
                int userId = GetCurrentUserId();

                vaccinations = vaccinations
                    .Where(v => v.IsSent &&
                                v.Pet != null &&
                                v.Pet.OwnerUserId == userId)
                    .ToList();
            }

            return View(vaccinations);
        }

        public async Task<IActionResult> Details(int id)
        {
            var vaccination = await _vaccinationService.GetByIdAsync(id);

            if (vaccination == null)
                return NotFound();

            if (User.IsInRole("Client") &&
                (!vaccination.IsSent ||
                 vaccination.Pet == null ||
                 vaccination.Pet.OwnerUserId != GetCurrentUserId()))
            {
                return Forbid();
            }

            return View(vaccination);
        }

        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Create()
        {
            var model = new VaccinationRecordFormsViewModel
            {
                DateGiven = DateOnly.FromDateTime(DateTime.Today),
                NextDueDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1))
            };

            await LoadFormOptionsAsync(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Create(VaccinationRecordFormsViewModel model, string submitAction)
        {
            model.RecordedByUserId = GetCurrentUserId();

            if (model.PetId <= 0)
            {
                ModelState.AddModelError(nameof(model.PetId), "Please select a pet.");
            }

            if (model.NextDueDate.HasValue && model.NextDueDate.Value < model.DateGiven)
            {
                ModelState.AddModelError(nameof(model.NextDueDate), "Next due date cannot be earlier than date given.");
            }

            if (!ModelState.IsValid)
            {
                await LoadFormOptionsAsync(model);
                return View(model);
            }

            var vaccination = new Vaccination
            {
                PetId = model.PetId,
                AppointmentId = model.AppointmentId ?? 0,
                VaccineName = model.VaccineName,
                DateGiven = model.DateGiven,
                NextDueDate = model.NextDueDate,
                Dose = model.Dose,
                Remarks = model.Remarks,
                RecordedByUserId = model.RecordedByUserId,
                CreatedAt = DateTime.Now,
                IsDeleted = false,
                IsSent = submitAction == "send"
            };

            await _vaccinationService.CreateAsync(vaccination);

            TempData["SuccessMessage"] = vaccination.IsSent
                ? "Vaccination record saved and sent to client."
                : "Vaccination record saved as draft.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> SendRecord(int id)
        {
            var vaccination = await _vaccinationService.GetByIdAsync(id);

            if (vaccination == null)
                return NotFound();

            vaccination.IsSent = true;

            await _vaccinationService.UpdateAsync(vaccination);

            TempData["SuccessMessage"] = "Vaccination record sent to client.";

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Edit(int id)
        {
            var vaccination = await _vaccinationService.GetByIdAsync(id);

            if (vaccination == null)
                return NotFound();

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

            await LoadFormOptionsAsync(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Edit(int id, VaccinationRecordFormsViewModel model)
        {
            if (id != model.VaccinationId)
                return NotFound();

            model.RecordedByUserId = GetCurrentUserId();

            if (model.PetId <= 0)
            {
                ModelState.AddModelError(nameof(model.PetId), "Please select a pet.");
            }

            if (model.NextDueDate.HasValue && model.NextDueDate.Value < model.DateGiven)
            {
                ModelState.AddModelError(nameof(model.NextDueDate), "Next due date cannot be earlier than date given.");
            }

            if (!ModelState.IsValid)
            {
                await LoadFormOptionsAsync(model);
                return View(model);
            }

            var vaccination = await _vaccinationService.GetByIdAsync(id);

            if (vaccination == null)
                return NotFound();

            vaccination.PetId = model.PetId;
            vaccination.AppointmentId = model.AppointmentId ?? 0;
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
                return NotFound();

            return View(vaccination);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _vaccinationService.DeleteAsync(id);

            TempData["SuccessMessage"] = "Vaccination record successfully deleted.";

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadFormOptionsAsync(VaccinationRecordFormsViewModel model)
        {
            var pets = await _petService.GetAllAsync();
            var appointments = await _appointmentService.GetAllAsync();

            model.PetOptions = pets
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

            model.AppointmentOptions = appointments
                .Where(a => !a.IsDeleted)
                .Select(a => new SelectListItem
                {
                    Value = a.AppointmentId.ToString(),
                    Text = (a.Pet != null ? a.Pet.PetName : "Pet #" + a.PetId)
                           + " - "
                           + a.AppointmentDate.ToString("MMM dd, yyyy")
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
    }
}