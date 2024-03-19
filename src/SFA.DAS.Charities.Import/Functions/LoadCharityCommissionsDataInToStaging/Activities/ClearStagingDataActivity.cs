using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using SFA.DAS.Charities.Data.Repositories;
using SFA.DAS.Charities.Domain;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Import.Functions.LoadCharityCommissionsDataInToStaging.Activities;

public class ClearStagingDataActivity
{
    private readonly ICharitiesImportRepository _charityImportRepository;

    public ClearStagingDataActivity(ICharitiesImportRepository charityImportRepository)
    {
        _charityImportRepository = charityImportRepository;
    }

    [Function(nameof(ClearStagingDataActivity))]
    // public async Task Run([Microsoft.Azure.Functions.Worker.ActivityTrigger] IDurableActivityContext context, ILogger logger)
    public async Task Run([ActivityTrigger] TaskOrchestrationContext context, ILogger logger)
    {
        // Does this need?
        //     var log = context.CreateReplaySafeLogger<ILogger>();
        //     using var performanceLogger = new PerformanceLogger($"Load staging data to live", log);
        using var performanceLogger = new PerformanceLogger($"Clear staging data", logger);
        await _charityImportRepository.ClearStagingData();
    }
}