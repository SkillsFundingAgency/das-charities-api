using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Data.Repositories;

public interface ICharitiesImportRepository
{
    Task BulkInsert<T>(IEnumerable<T> data, CancellationToken cancellationToken) where T : class;
    Task DeleteStagingData(string tableName, CancellationToken cancellationToken);
    Task LoadDataFromStagingInToLive(CancellationToken cancellationToken);
}