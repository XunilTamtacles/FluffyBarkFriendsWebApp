using Microsoft.EntityFrameworkCore;
using FluffyBarkFriendsWebApp.Models.Database;
using FluffyBarkFriendsWebApp.Views.Repositories.Interface;

namespace FluffyBarkFriendsWebApp.Views.Repositories.Implementation
{
    public class VaccinationRepository : IVaccinationRepository
    {
        private readonly FluffyBarkFriendsWebAppContext _context;

        public VaccinationRepository(FluffyBarkFriendsWebAppContext context)
        {
            _context = context;
        }

        public async Task<List<Vaccination>> GetAllAsync()
        {
            return await _context.Vaccinations
                .Include(v => v.Pet)
                .Include(v => v.Appointment)
                .Include(v => v.RecordedByUser)
                .Where(v => !v.IsDeleted)
                .OrderByDescending(v => v.DateGiven)
                .ToListAsync();
        }

        public async Task<List<Vaccination>> GetByPetIdAsync(int petId)
        {
            return await _context.Vaccinations
                .Include(v => v.Appointment)
                .Include(v => v.RecordedByUser)
                .Where(v => v.PetId == petId && !v.IsDeleted)
                .OrderByDescending(v => v.DateGiven)
                .ToListAsync();
        }

        
        public async Task<List<Vaccination>> GetUpcomingAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            return await _context.Vaccinations
                .Include(v => v.Pet)
                .Where(v => !v.IsDeleted &&
                            v.NextDueDate != null &&
                            v.NextDueDate >= today)
                .OrderBy(v => v.NextDueDate)
                .ToListAsync();
        }

        public async Task AddAsync(Vaccination vaccination)
        {
            await _context.Vaccinations.AddAsync(vaccination);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Vaccination vaccination)
        {
            _context.Vaccinations.Update(vaccination);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Vaccination vaccination)
        {
            vaccination.IsDeleted = true;
            _context.Vaccinations.Update(vaccination);
            await _context.SaveChangesAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        Task<Vaccination?> IVaccinationRepository.GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        Task<List<Vaccination>> IVaccinationRepository.GetByAppointmentIdAsync(int appointmentId)
        {
            throw new NotImplementedException();
        }

        Task<List<Vaccination>> IVaccinationRepository.GetOverdueAsync()
        {
            throw new NotImplementedException();
        }
    }
}