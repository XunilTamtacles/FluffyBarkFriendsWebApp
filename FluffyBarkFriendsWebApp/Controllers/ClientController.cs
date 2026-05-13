using System.Security.Claims;
using FluffyBarkFriendsWebApp.Models.Database;
using FluffyBarkFriendsWebApp.Models.ViewModels;
using FluffyBarkFriendsWebApp.Views.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FluffyBarkFriendsWebApp.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientController : Controller
    {
        private readonly IPetService _petService;
        private readonly IAppointmentService _appointmentService;
        private readonly IMedicalHistoryService _medicalHistoryService;
        private readonly IVaccinationService _vaccinationService;

        public ClientController(
            IPetService petService,
            IAppointmentService appointmentService,
            IMedicalHistoryService medicalHistoryService,
            IVaccinationService vaccinationService)
        {
            _petService = petService;
            _appointmentService = appointmentService;
            _medicalHistoryService = medicalHistoryService;
            _vaccinationService = vaccinationService;
        }

        public async Task<IActionResult> Index()
        {
            int userId = GetCurrentUserId();

            var pets = (await _petService.GetAllAsync())
                .Where(p => p.OwnerUserId == userId)
                .ToList();

            var appointments = (await _appointmentService.GetAllAsync())
                .Where(a => a.CreatedByUserId == userId)
                .ToList();

            var histories = (await _medicalHistoryService.GetAllAsync())
                .Where(h => h.Pet.OwnerUserId == userId)
                .ToList();

            var vaccinations = (await _vaccinationService.GetAllAsync())
                .Where(v => v.Pet.OwnerUserId == userId)
                .ToList();

            var model = new ClientPortalViewModel
            {
                PetsCount = pets.Count,
                AppointmentsCount = appointments.Count,
                MedicalHistoryCount = histories.Count,
                VaccinationsCount = vaccinations.Count,

                Pets = pets.Take(5).ToList(),
                Appointments = appointments.Take(5).ToList(),
                MedicalHistories = histories.Take(5).ToList(),
                Vaccinations = vaccinations.Take(5).ToList()
            };

            return View(model);
        }

        public async Task<IActionResult> MyPet()
        {
            int userId = GetCurrentUserId();

            var pets = (await _petService.GetAllAsync())
                .Where(p => p.OwnerUserId == userId)
                .ToList();

            return View(pets);
        }

        public IActionResult MyPets()
        {
            return RedirectToAction(nameof(MyPet));
        }

        public IActionResult AddPet()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPet(Pet pet)
        {
            if (!ModelState.IsValid)
            {
                return View(pet);
            }

            pet.OwnerUserId = GetCurrentUserId();
            pet.IsActive = true;
            pet.CreatedAt = DateTime.Now;

            await _petService.CreateAsync(pet);

            TempData["Success"] = "Pet added successfully.";

            return RedirectToAction(nameof(MyPet));
        }

        public async Task<IActionResult> MyAppointments()
        {
            int userId = GetCurrentUserId();

            var appointments = (await _appointmentService.GetAllAsync())
                .Where(a => a.CreatedByUserId == userId)
                .ToList();

            return View(appointments);
        }

        public async Task<IActionResult> MedicalHistory()
        {
            int userId = GetCurrentUserId();

            var histories = (await _medicalHistoryService.GetAllAsync())
                .Where(h => h.Pet.OwnerUserId == userId)
                .ToList();

            return View(histories);
        }

        public IActionResult MyMedicalHistory()
        {
            return RedirectToAction(nameof(MedicalHistory));
        }

        public async Task<IActionResult> Vaccination()
        {
            int userId = GetCurrentUserId();

            var vaccinations = (await _vaccinationService.GetAllAsync())
                .Where(v => v.Pet.OwnerUserId == userId)
                .ToList();

            return View(vaccinations);
        }

        public IActionResult MyVaccinations()
        {
            return RedirectToAction(nameof(Vaccination));
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
    }
}