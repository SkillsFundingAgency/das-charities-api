using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
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
    public async Task Run([Microsoft.Azure.Functions.Worker.ActivityTrigger] IDurableActivityContext context, ILogger logger)
    {
        using var performanceLogger = new PerformanceLogger($"Clear staging data", logger);
        await _charityImportRepository.ClearStagingData();
    }
}