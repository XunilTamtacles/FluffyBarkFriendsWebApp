using FluffyBarkFriendsWebApp.Models.Database;

namespace FluffyBarkFriendsWebApp.Views.Service.Interface
{
    public interface IVaccinationService
    {
        Task<List<Vaccination>> GetAllAsync();

        Task<List<Vaccination?>> GetByIdAsync(int id);

        Task<List<Vaccination>> GetUpComingDueAsAsync();

        Task<List<Vaccination>> GetOverDueAsAsync();

        Task<List<Vaccination>> GetByPetIdAsync();

        Task UpdateAsync (Vaccination vaccination);

        Task DeleteAsync (Vaccination vaccination);

        Task CreateAsync (Vaccination vaccination);

    }
}
