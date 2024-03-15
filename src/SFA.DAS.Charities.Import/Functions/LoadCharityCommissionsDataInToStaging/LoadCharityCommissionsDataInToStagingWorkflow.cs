using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SFA.DAS.Charities.Import.Functions.LoadCharityCommissionsDataInToStaging.Activities;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;

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

        [Function(nameof(LoadCharityCommissionsDataInToStagingWorkflow))]
        public async Task LoadCharityCommissionsDataInToStaging([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger logger)
        {
            await context.CallActivityAsync(nameof(ClearStagingDataActivity), null);

            var charityTask = context.CallActivityAsync(nameof(LoadCharityDataInToStagingActivity), _charityFileName);
            var trusteeTask = context.CallActivityAsync(nameof(LoadCharityTrusteeDataInToStagingActivity), _charityTrusteeFileName);
            await Task.WhenAll(charityTask, trusteeTask);
        }
    }
}
