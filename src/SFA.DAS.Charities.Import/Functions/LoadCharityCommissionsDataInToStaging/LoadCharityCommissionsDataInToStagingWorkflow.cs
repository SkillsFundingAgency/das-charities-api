using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SFA.DAS.Charities.Import.Functions.LoadCharityCommissionsDataInToStaging.Activities;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Import.Functions.LoadCharityCommissionsDataInToStaging
{
    public class LoadCharityCommissionsDataInToStagingWorkflow
    {
        private readonly string _charityTrusteeFileName;
        private readonly string _charityFileName;
        public LoadCharityCommissionsDataInToStagingWorkflow(IConfiguration configuration)
        {
            _charityTrusteeFileName = configuration["CharityTrusteeFileName"];
            _charityFileName = configuration["CharityFileName"];
        }

        [FunctionName(nameof(LoadCharityCommissionsDataInToStagingWorkflow))]
        public async Task LoadCharityCommissionsData([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger logger)
        {
            await context.CallActivityAsync(nameof(ClearStagingDataActivity), null);

            var charityTask = context.CallActivityAsync(nameof(LoadCharityDataInToStagingActivity), _charityFileName);
            var trusteeTask = context.CallActivityAsync(nameof(LoadCharityTrusteeDataInToStagingActivity), _charityTrusteeFileName);
            await Task.WhenAll(charityTask, trusteeTask);
        }
    }
}
