using Microsoft.EntityFrameworkCore;
using FluffyBarkFriendsWebApp.Models.Database;

namespace FluffyBarkFriendsWebApp.Views.Repositories.Implementation
{
    public class VaccinationRepository
    {
        private readonly FluffyBarkFriendsWebAppContext _context;

        public VaccinationRepository(FluffyBarkFriendsWebAppContext context)
        {
            _context = context;
        }

        public async Task<List<Vaccination>> GetAllAsync()
        {

        }

    }

    
}
