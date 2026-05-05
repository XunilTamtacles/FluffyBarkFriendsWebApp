using FluffyBarkFriendsWebApp.Models.Database;
using FluffyBarkFriendsWebApp.Models.ViewModels;
using FluffyBarkFriendsWebApp.Views.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FluffyBarkFriendsWebApp.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        public async Task<IActionResult> Index()
        {
            var appointments = await _appointmentService.GetAllAsync();
            return View(appointments);
        }

        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

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
        public async Task<IActionResult> Edit(int id, AppointmentFormsViewModel model)
        {
            if (id != model.AppointmentId)
            {
                return NotFound();
            }

            var existingAppointment = await _appointmentService.GetByIdAsync(id);

            if (existingAppointment == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var appointment = MapToAppointment(model);

            try
            {
                await _appointmentService.UpdateAsync(appointment);
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
            var appointment = await _appointmentService.GetByIdAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            await _appointmentService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
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
