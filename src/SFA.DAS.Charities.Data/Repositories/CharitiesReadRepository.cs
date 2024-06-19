using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Charities.Domain.Entities;

namespace SFA.DAS.Charities.Data.Repositories
{
    public class CharitiesReadRepository : ICharitiesReadRepository
    {
        private readonly CharitiesDataContext _charitiesDataContext;

        public CharitiesReadRepository(CharitiesDataContext charitiesDataContext)
        {
            _charitiesDataContext = charitiesDataContext;
        }
        public Task<Charity> GetCharityById(int registrationNumber)
        {
            return _charitiesDataContext.Charities
                .Include(c => c.Trustees)
                .SingleOrDefaultAsync(c => c.RegistrationNumber == registrationNumber && c.LinkedCharityId == 0);
        }

        public async Task<List<Charity>> SearchCharities(string searchTerm, int maximumResults = 500)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<Charity>();
            }

            searchTerm = searchTerm.Trim();

            return await _charitiesDataContext.Charities
                .Include(c => c.Trustees)
                .Where(c => c.RegistrationStatus == RegistrationStatus.Registered
                    && c.LinkedCharityId == 0
                    && c.Name.Contains(searchTerm))
                .Take(maximumResults)
                    .ToListAsync();
        }

    }
}
