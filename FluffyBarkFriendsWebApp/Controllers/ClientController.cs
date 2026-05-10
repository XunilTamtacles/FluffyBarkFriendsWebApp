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
            var pets = await _petService.GetAllAsync();
            var appointments = await _appointmentService.GetAllAsync();
            var histories = await _medicalHistoryService.GetAllAsync();
            var vaccinations = await _vaccinationService.GetAllAsync();

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
            var pets = await _petService.GetAllAsync();
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

            pet.IsActive = true;
            pet.CreatedAt = DateTime.Now;

            await _petService.CreateAsync(pet);

            TempData["Success"] = "Pet added successfully.";

            return RedirectToAction(nameof(MyPet));
        }


        public async Task<IActionResult> MyAppointments()
        {
            var appointments = await _appointmentService.GetAllAsync();
            return View(appointments);
        }

        

        public async Task<IActionResult> MedicalHistory()
        {
            var histories = await _medicalHistoryService.GetAllAsync();
            return View(histories);
        }

        public IActionResult MyMedicalHistory()
        {
            return RedirectToAction(nameof(MedicalHistory));
        }

    

        public async Task<IActionResult> Vaccination()
        {
            var vaccinations = await _vaccinationService.GetAllAsync();
            return View(vaccinations);
        }

        
        public IActionResult MyVaccinations()
        {
            return RedirectToAction(nameof(Vaccination));
        }
    }
}