using Microsoft.EntityFrameworkCore;
using FluffyBarkFriendsWebApp.Models.Database;

using Microsoft.EntityFrameworkCore;
using FluffyBarkFriendsWebApp.Models.Database;

namespace FluffyBarkFriendsWebApp.Views.Repositories.Implementation
{
    public class AppointmentRepository
    {
        private readonly FluffyBarkFriendsWebAppContext _context;

        public AppointmentRepository(FluffyBarkFriendsWebAppContext context)
        {
            _context = context;
        }

        public async Task<List<Appointment>> GetAllAsync()
        {
            var result = await _context.Appointments
                .Include(x => x.Pet)
                .Include(x => x.CreatedByUser)
                .Where(x => !x.IsDeleted)
                .ToListAsync();

            return result
                .OrderByDescending(x => x.AppointmentDate)
                .ThenBy(x => x.AppointmentTime)
                .ToList();
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            var appointment = await _context.Appointments
                .Include(x => x.Pet)
                .Include(x => x.CreatedByUser)
                .FirstOrDefaultAsync(x => x.AppointmentId == id);

            if (appointment == null)
                return null;

            if (appointment.IsDeleted)
                return null;

            return appointment;
        }

        public async Task<List<Appointment>> GetByPetIdAsync(int petId)
        {
            var list = await _context.Appointments
                .Include(x => x.Pet)
                .Include(x => x.CreatedByUser)
                .Where(x => x.PetId == petId)
                .ToListAsync();

            return list
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.AppointmentDate)
                .ToList();
        }

        public async Task<List<Appointment>> GetByDateAsync(DateOnly date)
        {
            var list = await _context.Appointments
                .Include(x => x.Pet)
                .Include(x => x.CreatedByUser)
                .Where(x => x.AppointmentDate == date)
                .ToListAsync();

            return list
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.AppointmentTime)
                .ToList();
        }

        public async Task<List<Appointment>> GetByStatusAsync(string status)
        {
            var list = await _context.Appointments
                .Include(x => x.Pet)
                .Include(x => x.CreatedByUser)
                .Where(x => x.Status == status)
                .ToListAsync();

            return list
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.AppointmentDate)
                .ToList();
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
    }
}
   