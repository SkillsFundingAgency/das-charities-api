using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Import.Functions.ImportCharityCommissionData
{
    public class ImportCharityCommissionDataWorkflow
    {
        [FunctionName(nameof(ImportCharityCommissionDataWorkflow))]
        public static Task ImportCharityCommissionData([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger logger)
        {
            return Task.CompletedTask;
        }
    }
}
