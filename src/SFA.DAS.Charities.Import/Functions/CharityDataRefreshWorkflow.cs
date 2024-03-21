using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using SFA.DAS.Charities.Import.Functions.ImportCharityCommissionData;
using SFA.DAS.Charities.Import.Infrastructure;
using System.Threading.Tasks;
using OrchestrationRuntimeStatus = Microsoft.DurableTask.Client.OrchestrationRuntimeStatus;

namespace SFA.DAS.Charities.Import.Functions;

public class CharityDataRefreshWorkflow
{
    private readonly IDateTimeProvider _timeProvider;

    public CharityDataRefreshWorkflow(IDateTimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    [Function(nameof(RefreshCharityDataTimerTrigger))]
    public async Task RefreshCharityDataTimerTrigger(
        [TimerTrigger("%CharitiesDataImportTimerInterval%", RunOnStartup = true)] TimerInfo myTimer,
        [DurableClient] DurableTaskClient orchestrationClient,
        FunctionContext executionContext)
    {
        ILogger log = executionContext.GetLogger(nameof(RefreshCharityDataTimerTrigger));

        var instanceId = $"charity-data-refresh-instance-{_timeProvider.Today:yyyy-MM-dd}";

        var existingInstance = await orchestrationClient.GetInstanceAsync(instanceId);

        if (existingInstance?.RuntimeStatus == OrchestrationRuntimeStatus.Running)
        {
            log.LogError("An instance with ID {instanceId} already exists.", instanceId);
            return;
        }

        await orchestrationClient.ScheduleNewOrchestrationInstanceAsync(nameof(CharityDataRefreshWorkflow), instanceId);
        log.LogInformation("Started charity data workflow. Orchestration id: {instanceId}", instanceId);
    }

    [Function(nameof(CharityDataRefreshWorkflow))]
    public async Task RefreshCharityDataOrchestrationTrigger(
            [OrchestrationTrigger] TaskOrchestrationContext context, FunctionContext executionContext)
    {
        var log = executionContext.GetLogger(nameof(CharityDataRefreshWorkflow));

        log.LogInformation($"Starting refresh of charity data: {context.CurrentUtcDateTime:F}", context.InstanceId);
        await context.CallSubOrchestratorAsync<Task>(nameof(ImportCharityCommissionDataWorkflow), null);
        log.LogDebug($"Finished download orchestration, now performing import to staging", context.InstanceId);
        // MFCMFC fixing one at a time
        // await context.CallSubOrchestratorAsync<Task>(nameof(LoadCharityCommissionsDataInToStagingWorkflow), null);
        // log.LogDebug("Finished populating staging tables workflow", context.InstanceId);
        // await context.CallActivityAsync<Task>(nameof(LoadActiveDataFromStagingActivity), null);
        // log.LogInformation($"Finished refreshing charity data: {context.CurrentUtcDateTime:F}", context.InstanceId);
    }
}
