using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using SFA.DAS.Charities.Import.Functions.ImportCharityCommissionData;
using SFA.DAS.Charities.Import.Infrastructure;
using SFA.DAS.Charities.Import.Functions.LoadActiveDataFromStaging.Activities;
using SFA.DAS.Charities.Import.Functions.LoadChairtyCommissionsDataInToStaging;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Import.Functions
{
    public class CharityDataRefreshWorkflow
    {
        private readonly IDateTimeProvider _timeProvider;

        public CharityDataRefreshWorkflow(IDateTimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
        }

        [FunctionName(nameof(RefreshCharityDataTimerTrigger))]
        public async Task RefreshCharityDataTimerTrigger(
            [TimerTrigger("%TimerInterval%")] TimerInfo myTimer, 
            [DurableClient] IDurableOrchestrationClient  starter,
            ILogger log)
        {
            var instanceId = $"charity-data-refresh-instance-{_timeProvider.Today:yyyy-MM-dd}";
            var existingInstance = await starter.GetStatusAsync(instanceId);
            if (existingInstance?.RuntimeStatus == OrchestrationRuntimeStatus.Running)
            {
                log.LogError($"An instance with ID '{instanceId}' already exists.");
                return;
            }

            await starter.StartNewAsync(nameof(CharityDataRefreshWorkflow), instanceId);
            log.LogInformation($"Started charity data workflow. Orchestration id: {instanceId}");
        }

        [FunctionName(nameof(CharityDataRefreshWorkflow))]
        public async Task RefreshCharityDataOrchestrationTrigger(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger logger)
        {
            var log = context.CreateReplaySafeLogger(logger);
            log.LogInformation($"Starting refresh of charity data: {context.CurrentUtcDateTime:F}");
            await context.CallSubOrchestratorAsync<Task>(nameof(ImportCharityCommissionDataWorkflow), null);
            log.LogDebug($"Finished download orchestration, now performing import to staging");
            await context.CallSubOrchestratorAsync<Task>(nameof(LoadChairtyCommissionsDataInToStagingWorkflow), null);
            log.LogDebug("Finished populating staging tables workflow");
            await context.CallActivityAsync<Task>(nameof(LoadActiveDataFromStagingActivity), null);
            log.LogInformation($"Finished refreshing charity data: {context.CurrentUtcDateTime:F}");
        }
    }
}
