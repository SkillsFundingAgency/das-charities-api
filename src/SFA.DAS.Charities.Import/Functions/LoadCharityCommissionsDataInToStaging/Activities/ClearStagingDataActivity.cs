using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Charities.Data.Repositories;
using SFA.DAS.Charities.Domain;

namespace SFA.DAS.Charities.Import.Functions.LoadCharityCommissionsDataInToStaging.Activities;

public class ClearStagingDataActivity
{
    private readonly ICharitiesImportRepository _charityImportRepository;

    public ClearStagingDataActivity(ICharitiesImportRepository charityImportRepository)
    {
        _charityImportRepository = charityImportRepository;
    }

    [Function(nameof(ClearStagingDataActivity))]
    public async Task Run([ActivityTrigger] FunctionContext context)
    {
        ILogger logger = context.GetLogger(nameof(ClearStagingDataActivity));
        using var performanceLogger = new PerformanceLogger($"Clear staging data", logger);
        logger.LogInformation("Starting ClearStagingDataActivity");

        await _charityImportRepository.ClearStagingData();
        logger.LogInformation("Finishing ClearStagingDataActivity");

    }
}