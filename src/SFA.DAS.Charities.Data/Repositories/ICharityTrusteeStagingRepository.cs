using SFA.DAS.Charities.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Data.Repositories
{
    public interface ICharityTrusteeStagingRepository
    {
        Task BulkInsert(List<CharityTrusteeStaging> data);
    }
}