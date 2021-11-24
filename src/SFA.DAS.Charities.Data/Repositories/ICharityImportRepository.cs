using SFA.DAS.Charities.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Data.Repositories
{
    public interface ICharityImportRepository
    {
        Task BulkInsert<T>(IList<T> data) where T : class;
        Task ClearStagingData();
    }
}