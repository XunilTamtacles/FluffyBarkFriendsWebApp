using Microsoft.EntityFrameworkCore;
using FluffyBarkFriendsWebApp.Models.Database;
using FluffyBarkFriendsWebApp.Views.Repositories.Interface;

namespace FluffyBarkFriendsWebApp.Views.Repositories.Implementation;

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
            .Include(m => m.Pet)
            .Include(m => m.Appointment)
            .Include(m => m.RecordedByUser)
            .Where(m => m.PetId == petId && !m.IsDeleted)
            .OrderByDescending(m => m.VisitDate)
            .ToListAsync();
    }


    public async Task<List<MedicalHistory>> SearchByDiagnosisAsync(string keyword)
    {
        return await _context.MedicalHistories
            .Include(m => m.Pet)
            .Where(m => !m.IsDeleted &&
                        m.Diagnosis.Contains(keyword))
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
        var lastWeek = today.AddDays(-7);

        return await _context.MedicalHistories
            .Include(m => m.Pet)
            .Where(m => !m.IsDeleted &&
                        m.VisitDate >= lastWeek)
            .OrderByDescending(m => m.VisitDate)
            .ToListAsync();
    }

    public async Task AddAsync(MedicalHistory medicalHistory)
    {
        await _context.MedicalHistories.AddAsync(medicalHistory);
    }

    public void Update(MedicalHistory medicalHistory)
    {
        _context.MedicalHistories.Update(medicalHistory);
    }

    public void Delete(MedicalHistory medicalHistory)
    {
        medicalHistory.IsDeleted = true;
        _context.MedicalHistories.Update(medicalHistory);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}