using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using SFA.DAS.Charities.Data.Repositories;
using SFA.DAS.Charities.Domain;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Import.Functions.LoadCharityCommissionsDataInToStaging.Activities
{
    public class ClearStagingDataActivity
    {
        private readonly ICharityImportRepository _charityImportRepository;

        public ClearStagingDataActivity(ICharityImportRepository charityImportRepository)
        {
            _charityImportRepository = charityImportRepository;
        }

        [FunctionName(nameof(ClearStagingDataActivity))]
        public async Task Run([ActivityTrigger] IDurableActivityContext context, ILogger logger)
        {
            using var performanceLogger = new PerformanceLogger($"Clear staging data", logger);
            await _charityImportRepository.ClearStagingData();
        }
    }
}