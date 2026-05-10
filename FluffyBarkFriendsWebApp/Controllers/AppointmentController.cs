using System.Security.Claims;
using FluffyBarkFriendsWebApp.Models.Database;
using FluffyBarkFriendsWebApp.Models.ViewModels;
using FluffyBarkFriendsWebApp.Views.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FluffyBarkFriendsWebApp.Controllers
{
    [Authorize(Roles = "Admin,Staff,Client")]
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [Authorize(Roles = "Admin,Staff,Client")]
        public async Task<IActionResult> Index()
        {
            var appointments = await _appointmentService.GetAllAsync();
            return View(appointments);
        }

        [Authorize(Roles = "Admin,Staff,Client")]
        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        [Authorize(Roles = "Admin,Staff")]
        public IActionResult Create()
        {
            var model = new AppointmentFormsViewModel
            {
                AppointmentDate = DateOnly.FromDateTime(DateTime.Today),
                AppointmentTime = new TimeOnly(9, 0),
                Status = "Pending"
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Create(AppointmentFormsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var appointment = MapToAppointment(model);

            try
            {
                await _appointmentService.CreateAsync(appointment);
                TempData["Success"] = "Appointment created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [Authorize(Roles = "Client")]
        public IActionResult Book()
        {
            var userId = GetCurrentUserId();

            var model = new AppointmentFormsViewModel
            {
                AppointmentDate = DateOnly.FromDateTime(DateTime.Today),
                AppointmentTime = new TimeOnly(9, 0),
                Status = "Pending",
                CreatedByUserId = userId
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Book(AppointmentFormsViewModel model)
        {
            model.Status = "Pending";
            model.CreatedByUserId = GetCurrentUserId();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var appointment = MapToAppointment(model);
            appointment.Status = "Pending";

            try
            {
                await _appointmentService.CreateAsync(appointment);
                TempData["Success"] = "Appointment request submitted successfully.";
                return RedirectToAction(nameof(Confirmation));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [Authorize(Roles = "Client")]
        public IActionResult Confirmation()
        {
            return View();
        }

        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Edit(int id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            var model = MapToAppointmentFormsViewModel(appointment);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Edit(int id, AppointmentFormsViewModel model)
        {
            if (id != model.AppointmentId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingAppointment = await _appointmentService.GetByIdAsync(id);

            if (existingAppointment == null)
            {
                return NotFound();
            }

            existingAppointment.PetId = model.PetId;
            existingAppointment.AppointmentDate = model.AppointmentDate;
            existingAppointment.AppointmentTime = model.AppointmentTime;
            existingAppointment.ReasonVisit = model.ReasonVisit;
            existingAppointment.Status = string.IsNullOrWhiteSpace(model.Status) ? "Pending" : model.Status;
            existingAppointment.Remarks = model.Remarks;
            existingAppointment.CreatedByUserId = model.CreatedByUserId;

            try
            {
                await _appointmentService.UpdateAsync(existingAppointment);
                TempData["Success"] = "Appointment updated successfully.";
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
            var appointment = await _appointmentService.GetByIdAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            await _appointmentService.DeleteAsync(id);

            TempData["Success"] = "Appointment deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        private int GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return 0;
            }

            return int.Parse(userId);
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
                Status = string.IsNullOrWhiteSpace(model.Status) ? "Pending" : model.Status,
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