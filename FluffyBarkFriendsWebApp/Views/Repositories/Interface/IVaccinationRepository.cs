using FluffyBarkFriendsWebApp.Models.Database;

public interface IVaccinationRepository
{
    Task<List<Vaccination>> GetAllAsync();
    Task<Vaccination?> GetByIdAsync(int id);

    Task<List<Vaccination>> GetByPetIdAsync(int petId);
    Task<List<Vaccination>> GetByAppointmentIdAsync(int appointmentId);

    Task<List<Vaccination>> GetUpcomingAsync();
    Task<List<Vaccination>> GetOverdueAsync();

    Task AddAsync(Vaccination vaccination);
    Task UpdateAsync(Vaccination vaccination);
    Task DeleteAsync(Vaccination vaccination);

    Task SaveAsync();
}