using Microsoft.EntityFrameworkCore;
using SFA.DAS.Charities.Domain.Entities;
using System.Threading.Tasks;

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
                .FirstOrDefaultAsync(c => c.RegistrationNumber == registrationNumber && c.LinkedCharityId == 0);
        }
    }
}
