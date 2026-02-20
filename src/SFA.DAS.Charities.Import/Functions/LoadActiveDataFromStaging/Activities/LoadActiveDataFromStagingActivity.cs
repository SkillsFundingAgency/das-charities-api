using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Charities.Data.Repositories;
using SFA.DAS.Charities.Domain;

namespace SFA.DAS.Charities.Import.Functions.LoadActiveDataFromStaging.Activities;

public class LoadActiveDataFromStagingActivity
{
    private readonly ICharitiesImportRepository _charityImportRepository;

    public LoadActiveDataFromStagingActivity(ICharitiesImportRepository charityImportRepository)
    {
        _charityImportRepository = charityImportRepository;
    }

    [Function(nameof(LoadActiveDataFromStagingActivity))]
    public async Task LoadActiveDataFromStaging([ActivityTrigger] FunctionContext context)
    {
        ILogger logger = context.GetLogger(nameof(LoadActiveDataFromStagingActivity));
        using var performanceLogger = new PerformanceLogger($"Load staging data to live", logger);
        logger.LogInformation("Starting LoadActiveDataFromStagingActivity");
        await _charityImportRepository.LoadDataFromStagingInToLive();
        logger.LogInformation("Finishing LoadActiveDataFromStagingActivity");
    }
}
