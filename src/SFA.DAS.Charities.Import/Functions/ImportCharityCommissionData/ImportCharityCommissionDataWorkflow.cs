using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SFA.DAS.Charities.Import.Functions.ImportCharityCommissionData.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Import.Functions.ImportCharityCommissionData
{
    public class ImportCharityCommissionDataWorkflow
    {
        private readonly IEnumerable<string> filenames; 

        public ImportCharityCommissionDataWorkflow(IConfiguration configuration)
        {
            filenames = configuration["CharityDownloadFileNames"].Split(',', System.StringSplitOptions.RemoveEmptyEntries);
        }

        [FunctionName(nameof(ImportCharityCommissionDataWorkflow))]
        public async Task ImportCharityCommissionData([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger logger)
        {
            logger = context.CreateReplaySafeLogger(logger);
            logger.LogDebug($"Downloading charity data files");

            var tasks = filenames.Select(f => context.CallActivityAsync(nameof(ImportCharityCommissionDataActivity), f.Trim()));

            await Task.WhenAll(tasks);
            logger.LogDebug($"Finished Downloading charity data files");
        }
    }
}
