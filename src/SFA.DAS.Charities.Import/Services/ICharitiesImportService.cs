using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Charities.Domain.Entities;
using SFA.DAS.Charities.Import.CharityCommissionModels;

namespace SFA.DAS.Charities.Import.Services;

public interface ICharitiesImportService
{
    Task<Stream> DownloadFile(string fileName, CancellationToken cancellationToken);
    Task<Stream> ClearStagingData<T>(Stream stream, CancellationToken cancellationToken);
    Task<List<TModel>> ExtractDataFromStream<TModel>(Stream stream, CancellationToken cancellationToken) where TModel : class;
    Task<List<CharityStaging>> MapToCharity(List<CharityModel> models, CancellationToken cancellationToken);
    Task<List<CharityTrusteeStaging>> MapToTrustee(List<CharityTrusteeModel> models, CancellationToken cancellationToken);
    Task BulkInsert<T>(IEnumerable<T> data, CancellationToken cancellationToken) where T : class;
    Task LoadDataFromStagingToLive(CancellationToken cancellationToken);
}
