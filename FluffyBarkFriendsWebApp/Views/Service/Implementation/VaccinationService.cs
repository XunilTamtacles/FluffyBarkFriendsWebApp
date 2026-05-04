using FluffyBarkFriendsWebApp.Models.Database;
using FluffyBarkFriendsWebApp.Views.Repositories.Interface;
using FluffyBarkFriendsWebApp.Views.Service.Interface;

namespace FluffyBarkFriendsWebApp.Views.Service.Implementation
{
    public class VaccinationService : IVaccinationService
    {
        private readonly IVaccinationRepository _vaccinationRepository;
        private readonly IPetRepository _petRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUserRepository _userRepository;

        public VaccinationService(
            IVaccinationRepository vaccinationRepository,
            IPetRepository petRepository,
            IAppointmentRepository appointmentRepository,
            IUserRepository userRepository)
        {
            _vaccinationRepository = vaccinationRepository;
            _petRepository = petRepository;
            _appointmentRepository = appointmentRepository;
            _userRepository = userRepository;
        }

        public async Task<List<Vaccination>> GetAllAsync()
        {
            return await _vaccinationRepository.GetAllAsync();
        }

        public async Task<Vaccination?> GetByIdAsync(int id)
        {
            return await _vaccinationRepository.GetByIdAsync(id);
        }

        public async Task<List<Vaccination>> GetByPetIdAsync(int petId)
        {
            return await _vaccinationRepository.GetByPetIdAsync(petId);
        }

        public async Task<List<Vaccination>> GetByAppointmentIdAsync(int appointmentId)
        {
            return await _vaccinationRepository.GetByAppointmentIdAsync(appointmentId);
        }

        public async Task<List<Vaccination>> GetUpcomingAsync()
        {
            return await _vaccinationRepository.GetUpcomingAsync();
        }

        public async Task<List<Vaccination>> GetOverdueAsync()
        {
            return await _vaccinationRepository.GetOverdueAsync();
        }

        public async Task CreateAsync(Vaccination vaccination)
        {
            await ValidateVaccination(vaccination);
            NormalizeVaccination(vaccination);

            vaccination.CreatedAt = DateTime.Now;
            vaccination.IsDeleted = false;

            await _vaccinationRepository.AddAsync(vaccination);
        }

        public async Task UpdateAsync(Vaccination vaccination)
        {
            var existingVaccination = await _vaccinationRepository.GetByIdAsync(vaccination.VaccinationId);

            if (existingVaccination == null)
            {
                throw new InvalidOperationException("Vaccination record was not found.");
            }

            await ValidateVaccination(vaccination);
            NormalizeVaccination(vaccination);

            existingVaccination.PetId = vaccination.PetId;
            existingVaccination.AppointmentId = vaccination.AppointmentId;
            existingVaccination.VaccineName = vaccination.VaccineName;
            existingVaccination.DateGiven = vaccination.DateGiven;
            existingVaccination.NextDueDate = vaccination.NextDueDate;
            existingVaccination.Dose = vaccination.Dose;
            existingVaccination.Remarks = vaccination.Remarks;
            existingVaccination.RecordedByUserId = vaccination.RecordedByUserId;

            await _vaccinationRepository.UpdateAsync(existingVaccination);
        }

        public async Task DeleteAsync(int id)
        {
            var vaccination = await _vaccinationRepository.GetByIdAsync(id);

            if (vaccination == null)
            {
                return;
            }

            await _vaccinationRepository.DeleteAsync(vaccination);
        }

        private async Task ValidateVaccination(Vaccination vaccination)
        {
            if (string.IsNullOrWhiteSpace(vaccination.VaccineName))
            {
                throw new ArgumentException("Vaccine name is required.");
            }

            if (vaccination.DateGiven == default)
            {
                throw new ArgumentException("Date given is required.");
            }

            if (vaccination.NextDueDate.HasValue &&
                vaccination.NextDueDate.Value < vaccination.DateGiven)
            {
                throw new ArgumentException("The next due date must be the same day or after the date given.");
            }

            var pet = await _petRepository.GetByIdAsync(vaccination.PetId);

            if (pet == null)
            {
                throw new InvalidOperationException("The selected pet record was not found.");
            }

            var appointment = await _appointmentRepository.GetByIdAsync(vaccination.AppointmentId);

            if (appointment == null)
            {
                throw new InvalidOperationException("The selected appointment record was not found.");
            }

            var user = await _userRepository.GetByIdAsync(vaccination.RecordedByUserId);

            if (user == null)
            {
                throw new InvalidOperationException("The user who recorded this vaccination was not found.");
            }
        }

        private static void NormalizeVaccination(Vaccination vaccination)
        {
            vaccination.VaccineName = vaccination.VaccineName.Trim();

            if (string.IsNullOrWhiteSpace(vaccination.Dose))
            {
                vaccination.Dose = null;
            }
            else
            {
                vaccination.Dose = vaccination.Dose.Trim();
            }

            if (string.IsNullOrWhiteSpace(vaccination.Remarks))
            {
                vaccination.Remarks = null;
            }
            else
            {
                vaccination.Remarks = vaccination.Remarks.Trim();
            }
        }
    }
}