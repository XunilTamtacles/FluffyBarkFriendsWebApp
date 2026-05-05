using FluffyBarkFriendsWebApp.Models.Database;
using FluffyBarkFriendsWebApp.Views.Repositories.Interface;
using FluffyBarkFriendsWebApp.Views.Service.Interface;

namespace FluffyBarkFriendsWebApp.Views.Service.Implementation
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public AppointmentService(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<List<Appointment>> GetAllAsync()
        {
            return await _appointmentRepository.GetAllAsync();
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await _appointmentRepository.GetByIdAsync(id);
        }

        public async Task<List<Appointment>> GetByPetIdAsync(int petId)
        {
            return await _appointmentRepository.GetByPetIdAsync(petId);
        }

        public async Task<List<Appointment>> GetByDateAsync(DateOnly appointmentDate)
        {
            return await _appointmentRepository.GetByDateAsync(appointmentDate);
        }

        public async Task<List<Appointment>> GetByStatusAsync(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return await _appointmentRepository.GetAllAsync();
            }

            return await _appointmentRepository.GetByStatusAsync(status.Trim());
        }

        public async Task CreateAsync(Appointment appointment)
        {
            CheckAppointment(appointment);
            NormalizeAppointment(appointment);

            appointment.IsDeleted = false;

            if (appointment.CreatedAt == default)
            {
                appointment.CreatedAt = DateTime.Now;
            }

            await _appointmentRepository.AddAsync(appointment);
        }

        public async Task UpdateAsync(Appointment appointment)
        {
            var existingAppointment = await _appointmentRepository.GetByIdAsync(appointment.AppointmentId);

            if (existingAppointment == null)
            {
                throw new InvalidOperationException("Appointment record was not found.");
            }

            CheckAppointment(appointment);
            NormalizeAppointment(appointment);

            existingAppointment.PetId = appointment.PetId;
            existingAppointment.AppointmentDate = appointment.AppointmentDate;
            existingAppointment.AppointmentTime = appointment.AppointmentTime;
            existingAppointment.ReasonVisit = appointment.ReasonVisit;
            existingAppointment.Status = appointment.Status;
            existingAppointment.Remarks = appointment.Remarks;
            existingAppointment.CreatedByUserId = appointment.CreatedByUserId;

            await _appointmentRepository.UpdateAsync(existingAppointment);
        }

        public async Task DeleteAsync(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);

            if (appointment == null)
            {
                return;
            }

            await _appointmentRepository.DeleteAsync(appointment);
        }

        private static void CheckAppointment(Appointment appointment)
        {
            if (appointment.PetId <= 0)
            {
                throw new ArgumentException("Pet is required.");
            }

            if (appointment.AppointmentDate == default)
            {
                throw new ArgumentException("Appointment date is required.");
            }

            if (appointment.AppointmentTime == default)
            {
                throw new ArgumentException("Appointment time is required.");
            }

            if (string.IsNullOrWhiteSpace(appointment.Status))
            {
                throw new ArgumentException("Status is required.");
            }

            if (appointment.CreatedByUserId <= 0)
            {
                throw new ArgumentException("Created by user is required.");
            }
        }

        private static void NormalizeAppointment(Appointment appointment)
        {
            appointment.Status = appointment.Status.Trim();

            if (string.IsNullOrWhiteSpace(appointment.ReasonVisit))
            {
                appointment.ReasonVisit = null;
            }
            else
            {
                appointment.ReasonVisit = appointment.ReasonVisit.Trim();
            }

            if (string.IsNullOrWhiteSpace(appointment.Remarks))
            {
                appointment.Remarks = null;
            }
            else
            {
                appointment.Remarks = appointment.Remarks.Trim();
            }
        }
    }
}
