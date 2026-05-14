using System.Security.Claims;
using FluffyBarkFriendsWebApp.Models.Database;
using FluffyBarkFriendsWebApp.Models.ViewModels;
using FluffyBarkFriendsWebApp.Views.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FluffyBarkFriendsWebApp.Controllers
{
    [Authorize(Roles = "Admin,Staff,Client")]
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IPetService _petService;

        public AppointmentController(
            IAppointmentService appointmentService,
            IPetService petService)
        {
            _appointmentService = appointmentService;
            _petService = petService;
        }

        public async Task<IActionResult> Index()
        {
            var appointments = await _appointmentService.GetAllAsync();

            if (User.IsInRole("Client"))
            {
                int userId = GetCurrentUserId();

                appointments = appointments
                    .Where(a => a.CreatedByUserId == userId)
                    .ToList();
            }

            return View(appointments);
        }

        public async Task<IActionResult> Book()
        {
            var model = new AppointmentFormsViewModel
            {
                AppointmentDate = DateOnly.FromDateTime(DateTime.Today),
                AppointmentTime = new TimeOnly(9, 0),
                Status = "Draft",
                CreatedByUserId = GetCurrentUserId()
            };

            await LoadPetsAsync(model);

            return View("Create", model);
        }

        public async Task<IActionResult> Create()
        {
            var model = new AppointmentFormsViewModel
            {
                AppointmentDate = DateOnly.FromDateTime(DateTime.Today),
                AppointmentTime = new TimeOnly(9, 0),
                Status = "Draft",
                CreatedByUserId = GetCurrentUserId()
            };

            await LoadPetsAsync(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentFormsViewModel model)
        {
            model.CreatedByUserId = GetCurrentUserId();
            model.Status = "Draft";

            if (model.PetId <= 0)
            {
                ModelState.AddModelError(nameof(model.PetId), "Please select a pet.");
            }

            if (!ModelState.IsValid)
            {
                await LoadPetsAsync(model);
                return View(model);
            }

            var appointment = MapToAppointment(model);

            try
            {
                await _appointmentService.CreateAsync(appointment);

                TempData["SuccessMessage"] = "Appointment request sent. Please wait for confirmation.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await LoadPetsAsync(model);
                ModelState.AddModelError(string.Empty, ex.Message);

                return View(model);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);

            if (appointment == null)
                return NotFound();

            if (!CanAccessAppointment(appointment))
                return Forbid();

            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Confirm(int id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);

            if (appointment == null)
                return NotFound();

            appointment.Status = "Confirmed";
            appointment.Remarks = "Your appointment has been confirmed.";

            await _appointmentService.UpdateAsync(appointment);

            TempData["SuccessMessage"] = "Appointment confirmed.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Cancel(int id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);

            if (appointment == null)
                return NotFound();

            appointment.Status = "Cancelled";
            appointment.Remarks = "Your appointment has been cancelled.";

            await _appointmentService.UpdateAsync(appointment);

            TempData["SuccessMessage"] = "Appointment cancelled.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);

            if (appointment == null)
                return NotFound();

            if (!CanAccessAppointment(appointment))
                return Forbid();

            var model = MapToAppointmentFormsViewModel(appointment);

            await LoadPetsAsync(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AppointmentFormsViewModel model)
        {
            if (id != model.AppointmentId)
                return NotFound();

            var existingAppointment = await _appointmentService.GetByIdAsync(id);

            if (existingAppointment == null)
                return NotFound();

            if (!CanAccessAppointment(existingAppointment))
                return Forbid();

            model.CreatedByUserId = existingAppointment.CreatedByUserId;

            if (User.IsInRole("Client"))
            {
                model.CreatedByUserId = GetCurrentUserId();
                model.Status = existingAppointment.Status;
            }

            if (model.PetId <= 0)
            {
                ModelState.AddModelError(nameof(model.PetId), "Please select a pet.");
            }

            if (!ModelState.IsValid)
            {
                await LoadPetsAsync(model);
                return View(model);
            }

            var appointment = MapToAppointment(model);

            try
            {
                await _appointmentService.UpdateAsync(appointment);

                TempData["SuccessMessage"] = "Appointment updated.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await LoadPetsAsync(model);
                ModelState.AddModelError(string.Empty, ex.Message);

                return View(model);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);

            if (appointment == null)
                return NotFound();

            return View(appointment);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);

            if (appointment == null)
                return NotFound();

            await _appointmentService.DeleteAsync(id);

            TempData["SuccessMessage"] = "Appointment deleted.";

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadPetsAsync(AppointmentFormsViewModel model)
        {
            var pets = await _petService.GetAllAsync();

            int currentUserId = GetCurrentUserId();

            if (User.IsInRole("Client"))
            {
                pets = pets
                    .Where(p => p.OwnerUserId == currentUserId && p.IsActive)
                    .ToList();
            }
            else
            {
                pets = pets
                    .Where(p => p.IsActive)
                    .ToList();
            }

            model.PetOptions = pets
                .Select(p => new SelectListItem
                {
                    Value = p.PetId.ToString(),
                    Text = $"PET-{p.PetId} - {p.PetName}"
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

        private bool CanAccessAppointment(Appointment appointment)
        {
            if (User.IsInRole("Admin") || User.IsInRole("Staff"))
                return true;

            if (User.IsInRole("Client"))
                return appointment.CreatedByUserId == GetCurrentUserId();

            return false;
        }

        private static Appointment MapToAppointment(AppointmentFormsViewModel model)
        {
            return new Appointment
            {
                AppointmentId = model.AppointmentId,
                PetId = model.PetId,
                AppointmentDate = model.AppointmentDate,
                AppointmentTime = model.AppointmentTime,
                ReasonVisit = model.ReasonVisit,
                Status = model.Status,
                Remarks = model.Remarks,
                CreatedByUserId = model.CreatedByUserId
            };
        }

        private static AppointmentFormsViewModel MapToAppointmentFormsViewModel(Appointment appointment)
        {
            return new AppointmentFormsViewModel
            {
                AppointmentId = appointment.AppointmentId,
                PetId = appointment.PetId,
                AppointmentDate = appointment.AppointmentDate,
                AppointmentTime = appointment.AppointmentTime,
                ReasonVisit = appointment.ReasonVisit,
                Status = appointment.Status,
                Remarks = appointment.Remarks,
                CreatedByUserId = appointment.CreatedByUserId
            };
        }
    }
}