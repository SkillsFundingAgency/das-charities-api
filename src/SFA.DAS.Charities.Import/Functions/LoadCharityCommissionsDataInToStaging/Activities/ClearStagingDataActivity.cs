using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
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
    public async Task Run([ActivityTrigger] TaskOrchestrationContext context, FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger(nameof(ClearStagingDataActivity));

        using var performanceLogger = new PerformanceLogger($"Clear staging data", logger);
        await _charityImportRepository.ClearStagingData();
    }
}