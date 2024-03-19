using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using SFA.DAS.Charities.Import.Functions.ImportCharityCommissionData;
using SFA.DAS.Charities.Import.Functions.LoadActiveDataFromStaging.Activities;
using SFA.DAS.Charities.Import.Functions.LoadCharityCommissionsDataInToStaging;
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
        //        [TimerTrigger("%CharitiesDataImportTimerInterval%")] TimerInfo myTimer,
        [TimerTrigger("0 0 19 * * 1-5", RunOnStartup = true)] TimerInfo myTimer,
        //[DurableClient] IDurableOrchestrationClient orchestrationClient,
        [DurableClient] DurableTaskClient orchestrationClient,
        ILogger log)
    {

        var instanceId = $"charity-data-refresh-instance-{_timeProvider.Today:yyyy-MM-dd}";
        //  var existingInstance = await orchestrationClient.GetStatusAsync(instanceId);
        // https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-dotnet-isolated-overview#public-api-changes
        // https://stackoverflow.com/questions/77865212/durable-functions-get-status-in-isolated-functions
        var existingInstance = await orchestrationClient.GetInstanceAsync(instanceId);

        if (existingInstance?.RuntimeStatus == OrchestrationRuntimeStatus.Running)
        {
            log.LogError("An instance with ID {instanceId} already exists.", instanceId);
            return;
        }

        // await orchestrationClient.StartNewAsync(nameof(CharityDataRefreshWorkflow), instanceId);
        await orchestrationClient.ScheduleNewOrchestrationInstanceAsync(nameof(CharityDataRefreshWorkflow), instanceId);
        // https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-dotnet-isolated-overview#public-api-changes
        // MFCMFC taken out to check issues // log.LogInformation("Started charity data workflow. Orchestration id: {instanceId}", instanceId);
    }

    [Function(nameof(CharityDataRefreshWorkflow))]
    public async Task RefreshCharityDataOrchestrationTrigger(
        //[OrchestrationTrigger] IDurableOrchestrationContext context,
        [OrchestrationTrigger] TaskOrchestrationContext context)
    // ILogger logger)
    {
        //var log = context.CreateReplaySafeLogger<ILogger>();
        //log.LogInformation($"Starting refresh of charity data: {context.CurrentUtcDateTime:F}", context.InstanceId);
        await context.CallSubOrchestratorAsync<Task>(nameof(ImportCharityCommissionDataWorkflow), null);
        //log.LogDebug($"Finished download orchestration, now performing import to staging", context.InstanceId);
        await context.CallSubOrchestratorAsync<Task>(nameof(LoadCharityCommissionsDataInToStagingWorkflow), null);
        //log.LogDebug("Finished populating staging tables workflow", context.InstanceId);
        await context.CallActivityAsync<Task>(nameof(LoadActiveDataFromStagingActivity), null);
        //log.LogInformation($"Finished refreshing charity data: {context.CurrentUtcDateTime:F}", context.InstanceId);
    }
}
