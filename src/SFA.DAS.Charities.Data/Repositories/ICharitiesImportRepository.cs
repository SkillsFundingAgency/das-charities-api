using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Data.Repositories
{
    public interface ICharitiesImportRepository
    {
        Task BulkInsert<T>(IList<T> data) where T : class;
        Task ClearStagingData();
        Task LoadDataFromStagingInToLive();
    }
}