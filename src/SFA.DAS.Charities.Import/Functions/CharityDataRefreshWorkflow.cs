using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using SFA.DAS.Charities.Import.Functions.ImportCharityCommissionData;
using SFA.DAS.Charities.Import.Functions.LoadActiveDataFromStaging.Activities;
using SFA.DAS.Charities.Import.Functions.LoadCharityCommissionsDataInToStaging;
using SFA.DAS.Charities.Import.Infrastructure;
using SFA.DAS.Charities.Import.Services;

namespace SFA.DAS.Charities.Import.Functions;

public class CharityDataRefreshWorkflow
{
    private readonly IDateTimeProvider _timeProvider;
    private readonly ILogger<CharityDataRefreshWorkflow> _logger;
    private readonly IDurableTaskClientWrapper _durableTaskClientWrapper;

    public CharityDataRefreshWorkflow(IDateTimeProvider timeProvider, ILogger<CharityDataRefreshWorkflow> logger, IDurableTaskClientWrapper durableTaskClientWrapper)
    {
        _timeProvider = timeProvider;
        _logger = logger;
        _durableTaskClientWrapper = durableTaskClientWrapper;
    }

    [Function(nameof(RefreshCharityDataTimerTrigger))]
    public async Task RefreshCharityDataTimerTrigger(
        [TimerTrigger("%CharitiesDataImportTimerInterval%")] TimerInfo myTimer,
        [DurableClient] DurableTaskClient orchestrationClient)
    {
        _durableTaskClientWrapper.Client = orchestrationClient;
        var instanceId = $"charity-data-refresh-instance-{_timeProvider.Today:yyyy-MM-dd}";
        OrchestrationMetadata existingInstance = await _durableTaskClientWrapper.GetInstanceAsync(instanceId);
        if (existingInstance?.RuntimeStatus == OrchestrationRuntimeStatus.Running)
        {
            _logger.LogWarning("An instance with ID {InstanceId} already exists.", instanceId);
            return;
        }

        await _durableTaskClientWrapper.ScheduleNewOrchestrationInstanceAsync(nameof(CharityDataRefreshWorkflow), instanceId);

        _logger.LogInformation("Started charity data workflow. Orchestration id: {InstanceId}", instanceId);
    }

    [Function(nameof(CharityDataRefreshWorkflow))]
    public async Task RefreshCharityDataOrchestrationTrigger([OrchestrationTrigger] TaskOrchestrationContext context)
    {
        ILogger log = context.CreateReplaySafeLogger(nameof(CharityDataRefreshWorkflow));

        log.LogInformation("{InstanceId}: Starting charities data import", context.InstanceId);

        await context.CallSubOrchestratorAsync<Task>(nameof(ImportCharityCommissionDataWorkflow), null);

        await context.CallSubOrchestratorAsync<Task>(nameof(LoadCharityCommissionsDataInToStagingWorkflow), null);

        await context.CallActivityAsync<Task>(nameof(LoadActiveDataFromStagingActivity), null);

        log.LogInformation("{InstanceId}: Finished charities data import", context.InstanceId);
    }
}
