using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Import.Functions.LoadChairtyCommissionsDataInToStaging
{
    public class LoadChairtyCommissionsDataInToStagingWorkflow
    {
        [FunctionName(nameof(LoadChairtyCommissionsDataInToStagingWorkflow))]
        public static Task LoadChairtyCommissionsData([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger logger)
        {
            return Task.CompletedTask;
        }
    }
}
