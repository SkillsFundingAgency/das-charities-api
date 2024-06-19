using SFA.DAS.Charities.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Data.Repositories
{
    public interface ICharitiesReadRepository
    {
        Task<Charity> GetCharityById(int registrationNumber);
        Task<List<Charity>> SearchCharities(string searchTerm, int maximumResults);
    }
}
