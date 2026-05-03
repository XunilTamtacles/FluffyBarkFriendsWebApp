using Microsoft.EntityFrameworkCore;
using FluffyBarkFriendsWebApp.Models.Database;
using FluffyBarkFriendsWebApp.Views.Repositories.Interface;

namespace FluffyBarkFriendsWebApp.Views.Repositories.Implementation
{
    public class MedicalHistoryRepository : IMedicalHistoryRepository
    {
        private readonly FluffyBarkFriendsWebAppContext _context;

        public MedicalHistoryRepository(FluffyBarkFriendsWebAppContext context)
        {
            _context = context;
        }

        public async Task<List<MedicalHistory>> GetAllAsync()
        {
            return await _context.MedicalHistories
                .Include(m => m.Pet)
                .Include(m => m.Appointment)
                .Include(m => m.RecordedByUser)
                .Where(m => !m.IsDeleted)
                .OrderByDescending(m => m.VisitDate)
                .ThenByDescending(m => m.VisitTime)
                .ToListAsync();
        }

        public async Task<MedicalHistory?> GetByIdAsync(int id)
        {
            return await _context.MedicalHistories
                .Include(m => m.Pet)
                .Include(m => m.Appointment)
                .Include(m => m.RecordedByUser)
                .FirstOrDefaultAsync(m => m.MedicalHistoryId == id && !m.IsDeleted);
        }

        public async Task<List<MedicalHistory>> GetByPetIdAsync(int petId)
        {
            return await _context.MedicalHistories
                .Include(m => m.Appointment)
                .Include(m => m.RecordedByUser)
                .Where(m => m.PetId == petId && !m.IsDeleted)
                .OrderByDescending(m => m.VisitDate)
                .ToListAsync();
        }

        public async Task<List<MedicalHistory>> SearchByDiagnosisAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await GetAllAsync();

            return await _context.MedicalHistories
                .Include(m => m.Pet)
                .Where(m => !m.IsDeleted &&
                            m.Diagnosis != null &&
                            m.Diagnosis.Contains(keyword))
                .OrderByDescending(m => m.VisitDate)
                .ToListAsync();
        }

        public async Task<List<MedicalHistory>> GetByDateRangeAsync(DateOnly start, DateOnly end)
        {
            return await _context.MedicalHistories
                .Include(m => m.Pet)
                .Where(m => !m.IsDeleted &&
                            m.VisitDate >= start &&
                            m.VisitDate <= end)
                .OrderByDescending(m => m.VisitDate)
                .ToListAsync();
        }

        public async Task<List<MedicalHistory>> GetRecentCasesAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var last7Days = today.AddDays(-7);

            return await _context.MedicalHistories
                .Include(m => m.Pet)
                .Where(m => !m.IsDeleted &&
                            m.VisitDate >= last7Days)
                .OrderByDescending(m => m.VisitDate)
                .ToListAsync();
        }

        public async Task AddAsync(MedicalHistory medicalHistory)
        {
            await _context.MedicalHistories.AddAsync(medicalHistory);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MedicalHistory medicalHistory)
        {
            _context.MedicalHistories.Update(medicalHistory);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(MedicalHistory medicalHistory)
        {
            medicalHistory.IsDeleted = true; 
            _context.MedicalHistories.Update(medicalHistory);
            await _context.SaveChangesAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}