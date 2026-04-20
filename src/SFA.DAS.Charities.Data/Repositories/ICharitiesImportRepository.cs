using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Data.Repositories;

public interface ICharitiesImportRepository
{
    Task BulkInsert<T>(IEnumerable<T> data) where T : class;
    Task DeleteStagingData(string tableName);
    Task LoadDataFromStagingInToLive();
}