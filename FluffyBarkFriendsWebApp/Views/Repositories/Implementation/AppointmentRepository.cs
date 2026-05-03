using Microsoft.EntityFrameworkCore;
using FluffyBarkFriendsWebApp.Models.Database;
using FluffyBarkFriendsWebApp.Views.Repositories.Interface;

namespace FluffyBarkFriendsWebApp.Views.Repositories.Implementation
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly FluffyBarkFriendsWebAppContext _context;

        public AppointmentRepository(FluffyBarkFriendsWebAppContext context)
        {
            _context = context;
        }

        public async Task<List<Appointment>> GetAllAsync()
        {
            return await _context.Appointments
                .Include(x => x.Pet)
                .Include(x => x.CreatedByUser)
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.AppointmentDate)
                .ThenBy(x => x.AppointmentTime)
                .ToListAsync();
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await _context.Appointments
                .Include(x => x.Pet)
                .Include(x => x.CreatedByUser)
                .FirstOrDefaultAsync(x => x.AppointmentId == id && !x.IsDeleted);
        }

        public async Task<List<Appointment>> GetByPetIdAsync(int petId)
        {
            return await _context.Appointments
                .Include(x => x.Pet)
                .Include(x => x.CreatedByUser)
                .Where(x => x.PetId == petId && !x.IsDeleted)
                .OrderByDescending(x => x.AppointmentDate)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetByDateAsync(DateOnly date)
        {
            return await _context.Appointments
                .Include(x => x.Pet)
                .Include(x => x.CreatedByUser)
                .Where(x => x.AppointmentDate == date && !x.IsDeleted)
                .OrderBy(x => x.AppointmentTime)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetByStatusAsync(string status)
        {
            return await _context.Appointments
                .Include(x => x.Pet)
                .Include(x => x.CreatedByUser)
                .Where(x => x.Status == status && !x.IsDeleted)
                .OrderByDescending(x => x.AppointmentDate)
                .ToListAsync();
        }

        public async Task AddAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Appointment appointment)
        {
            appointment.IsDeleted = true; 
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
        }

        
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public Task<List<Appointment>> GetIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        void IAppointmentRepository.UpdateAsync(Appointment appointment)
        {
            throw new NotImplementedException();
        }

        public void Delete(Appointment appointment)
        {
            throw new NotImplementedException();
        }
    }
}