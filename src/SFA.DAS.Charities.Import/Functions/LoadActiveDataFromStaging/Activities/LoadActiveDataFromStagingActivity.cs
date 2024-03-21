using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using SFA.DAS.Charities.Data.Repositories;
using SFA.DAS.Charities.Domain;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Import.Functions.LoadActiveDataFromStaging.Activities;

public class LoadActiveDataFromStagingActivity
{
    private readonly ICharitiesImportRepository _charityImportRepository;

    public LoadActiveDataFromStagingActivity(ICharitiesImportRepository charityImportRepository)
    {
        _charityImportRepository = charityImportRepository;
    }
    [Function(nameof(LoadActiveDataFromStagingActivity))]
    public async Task LoadActiveDataFromStaging([ActivityTrigger] TaskOrchestrationContext context, FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger(nameof(LoadActiveDataFromStagingActivity));
        using var performanceLogger = new PerformanceLogger("Load staging data to live", logger);
        await _charityImportRepository.LoadDataFromStagingInToLive();
    }

}
