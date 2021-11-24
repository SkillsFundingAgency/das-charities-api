using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SFA.DAS.Charities.Import.Functions.LoadChairtyCommissionsDataInToStaging.Activities;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Import.Functions.LoadChairtyCommissionsDataInToStaging
{
    public class LoadChairtyCommissionsDataInToStagingWorkflow
    {
        private readonly string _charityTrusteeFileName;
        private readonly string _charityFileName;
        public LoadChairtyCommissionsDataInToStagingWorkflow(IConfiguration configuration)
        {
            _charityTrusteeFileName = configuration["CharityTrusteeFileName"];
            _charityFileName = configuration["CharityFileName"];
        }

        [FunctionName(nameof(LoadChairtyCommissionsDataInToStagingWorkflow))]
        public async Task LoadChairtyCommissionsData([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger logger)
        {

            var charityTask = context.CallActivityAsync(nameof(LoadCharityDataInToStagingActivity), _charityFileName);
            var trusteeTask = context.CallActivityAsync(nameof(LoadCharityTrusteeDataInToStagingActivity), _charityTrusteeFileName);
            await Task.WhenAll(charityTask, trusteeTask);
        }
    }
}
