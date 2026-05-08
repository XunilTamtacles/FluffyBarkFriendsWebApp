using Microsoft.AspNetCore.Mvc;
using FluffyBarkFriendsWebApp.Models.Database;
using FluffyBarkFriendsWebApp.Views.Service.Interface;

namespace FluffyBarkFriendsWebApp.Controllers
{
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

        public IActionResult Create()
        {
            return View();
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vaccination vaccination)
        {
            if (!ModelState.IsValid)
            {
                return View(vaccination);
            }

            if (vaccination.NextDueDate.HasValue &&
                vaccination.NextDueDate.Value < vaccination.DateGiven)
            {
                ModelState.AddModelError("NextDueDate",
                    "Next due date cannot be earlier than date given.");

                return View(vaccination);
            }

            await _vaccinationService.CreateAsync(vaccination);

            return RedirectToAction(nameof(Index));
        }

       
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
        public async Task<IActionResult> Edit(int id, Vaccination vaccination)
        {
            if (id != vaccination.VaccinationId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(vaccination);
            }

            if (vaccination.NextDueDate.HasValue &&
                vaccination.NextDueDate.Value < vaccination.DateGiven)
            {
                ModelState.AddModelError("NextDueDate",
                    "Next due date cannot be earlier than date given.");

                return View(vaccination);
            }

            await _vaccinationService.UpdateAsync(vaccination);

            return RedirectToAction(nameof(Index));
        }

       
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _vaccinationService.DeleteAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}