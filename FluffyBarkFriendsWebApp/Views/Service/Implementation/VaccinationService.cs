using FluffyBarkFriendsWebApp.Models;
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

        public async Task<List<Vaccination>> GetByPetIdAsync(int petId)
        {
            return await _vaccinationRepository.GetByPetIdAsync(petId);
        }

        public async Task<Vaccination?> GetByIdAsync(int id)
        {
            return await _vaccinationRepository.GetByIdAsync(id);
        }

        public async Task<List<Vaccination>> GetByPetIdAsync(int petId)
        {
            return await _vaccinationRepository.GetByPetIdAsync(petId);
        }

        public async Task<List<Vaccination>> Get
}
