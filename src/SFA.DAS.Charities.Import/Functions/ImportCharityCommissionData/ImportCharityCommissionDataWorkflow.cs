using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SFA.DAS.Charities.Import.Functions.ImportCharityCommissionData.Activities;

namespace SFA.DAS.Charities.Import.Functions.ImportCharityCommissionData;

public class ImportCharityCommissionDataWorkflow
{
    private readonly IEnumerable<string> filenames;

    public ImportCharityCommissionDataWorkflow(IConfiguration configuration)
    {
        filenames = configuration["CharitiesDownloadFileNames"].Split(',', System.StringSplitOptions.RemoveEmptyEntries);
    }

    [Function(nameof(ImportCharityCommissionDataWorkflow))]
    public async Task ImportCharityCommissionData([OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var logger = context.CreateReplaySafeLogger(nameof(ImportCharityCommissionDataWorkflow));
        logger.LogInformation($"Downloading charity data files");

        var tasks = filenames.Select(f => context.CallActivityAsync(nameof(ImportCharityCommissionDataActivity), f.Trim()));

        await Task.WhenAll(tasks);
        logger.LogInformation($"Finished Downloading charity data files");
    }
}
