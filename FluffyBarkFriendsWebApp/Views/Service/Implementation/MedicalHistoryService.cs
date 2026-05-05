using FluffyBarkFriendsWebApp.Models.Database;
using FluffyBarkFriendsWebApp.Views.Repositories.Interface;
using FluffyBarkFriendsWebApp.Views.Service.Interface;

namespace FluffyBarkFriendsWebApp.Views.Service.Implementation
{
    public class MedicalHistoryService : IMedicalHistoryService
    {
        private readonly IMedicalHistoryRepository _medicalHistoryRepository;

        public MedicalHistoryService(IMedicalHistoryRepository medicalHistoryRepository)
        {
            _medicalHistoryRepository = medicalHistoryRepository;
        }

        public async Task<List<MedicalHistory>> GetAllAsync()
        {
            return await _medicalHistoryRepository.GetAllAsync();
        }

        public async Task<MedicalHistory?> GetByIdAsync(int id)
        {
            return await _medicalHistoryRepository.GetByIdAsync(id);
        }

        public async Task<List<MedicalHistory>> GetByPetIdAsync(int petId)
        {
            return await _medicalHistoryRepository.GetByPetIdAsync(petId);
        }

        public async Task<List<MedicalHistory>> GetRecentCasesAsync()
        {
            return await _medicalHistoryRepository.GetRecentCasesAsync();
        }

        public async Task CreateAsync(MedicalHistory history)
        {
            CheckMedicalHistory(history);
            NormalizeMedicalHistory(history);

            history.IsDeleted = false;

            if (history.CreatedAt == default)
            {
                history.CreatedAt = DateTime.Now;
            }

            await _medicalHistoryRepository.AddAsync(history);
        }

        public async Task UpdateAsync(MedicalHistory history)
        {
            var existingHistory = await _medicalHistoryRepository.GetByIdAsync(history.MedicalHistoryId);

            if (existingHistory == null)
            {
                throw new InvalidOperationException("Medical history record was not found.");
            }

            CheckMedicalHistory(history);
            NormalizeMedicalHistory(history);

            existingHistory.PetId = history.PetId;
            existingHistory.VisitDate = history.VisitDate;
            existingHistory.VisitTime = history.VisitTime;
            existingHistory.Condition = history.Condition;
            existingHistory.Diagnosis = history.Diagnosis;
            existingHistory.Treatment = history.Treatment;
            existingHistory.Dosage = history.Dosage;
            existingHistory.Notes = history.Notes;
            existingHistory.Medication = history.Medication;
            existingHistory.CreatedByUserId = history.CreatedByUserId;

            await _medicalHistoryRepository.UpdateAsync(existingHistory);
        }

        public async Task DeleteAsync(int id)
        {
            var history = await _medicalHistoryRepository.GetByIdAsync(id);

            if (history == null)
            {
                return;
            }

            await _medicalHistoryRepository.DeleteAsync(history);
        }

        private static void CheckMedicalHistory(MedicalHistory history)
        {
            if (history.PetId <= 0)
            {
                throw new ArgumentException("Pet is required.");
            }

            if (history.VisitDate == default)
            {
                throw new ArgumentException("Visit date is required.");
            }

            if (string.IsNullOrWhiteSpace(history.Condition))
            {
                throw new ArgumentException("Condition is required.");
            }

            if (history.CreatedByUserId <= 0)
            {
                throw new ArgumentException("Created by user is required.");
            }
        }

        private static void NormalizeMedicalHistory(MedicalHistory history)
        {
            history.Condition = history.Condition.Trim();
            history.Diagnosis = CleanText(history.Diagnosis);
            history.Treatment = CleanText(history.Treatment);
            history.Dosage = CleanText(history.Dosage);
            history.Notes = CleanText(history.Notes);
            history.Medication = CleanText(history.Medication);
        }

        private static string? CleanText(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return value.Trim();
        }
    }
}
