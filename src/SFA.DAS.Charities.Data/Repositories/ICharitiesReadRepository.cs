using SFA.DAS.Charities.Domain.Entities;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Data.Repositories
{
    public interface ICharitiesReadRepository
    {
        Task<Charity> GetCharityById(int registrationNumber);
    }
}
