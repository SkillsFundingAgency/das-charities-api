using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Charities.Data.Repositories;
using SFA.DAS.Charities.Domain;
using SFA.DAS.Charities.Domain.Entities;
using SFA.DAS.Charities.Import.Functions.LoadCharityCommissionsDataInToStaging.CharityCommissionModels;
using SFA.DAS.Charities.Import.Infrastructure;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Import.Functions.LoadCharityCommissionsDataInToStaging.Activities;

public class LoadCharityTrusteeDataInToStagingActivity
{
    private readonly ICharitiesImportRepository _charityTrusteeStagingRepository;

    public LoadCharityTrusteeDataInToStagingActivity(ICharitiesImportRepository charityTrusteeStagingRepository)
    {
        _charityTrusteeStagingRepository = charityTrusteeStagingRepository;
    }

    [Function(nameof(LoadCharityTrusteeDataInToStagingActivity))]
    public async Task Run(
        [ActivityTrigger] string fileName,
        [BlobInput("charity-files/{fileName}", Connection = "CharitiesStorageConnectionString")] Stream fileStream,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger(nameof(LoadCharityTrusteeDataInToStagingActivity));

        using var performanceLogger = new PerformanceLogger($"Load trustees in staging", logger);

        var trusteeData = CharityCommissionDataHelper.ExtractData<CharityTrusteeModel>(fileStream);

        var data = trusteeData.Select(x => (CharityTrusteeStaging)x).ToList();

        await _charityTrusteeStagingRepository.BulkInsert(data);

        logger.LogInformation("Total trustees {trusteesCount}", trusteeData.Count);
    }
}
