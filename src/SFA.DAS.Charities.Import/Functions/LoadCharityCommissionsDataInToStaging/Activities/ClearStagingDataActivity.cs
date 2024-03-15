using Microsoft.Extensions.Logging;
using SFA.DAS.Charities.Data.Repositories;
using SFA.DAS.Charities.Domain;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;

namespace SFA.DAS.Charities.Import.Functions.LoadCharityCommissionsDataInToStaging.Activities
{
    public class ClearStagingDataActivity
    {
        private readonly ICharitiesImportRepository _charityImportRepository;

        public ClearStagingDataActivity(ICharitiesImportRepository charityImportRepository)
        {
            _charityImportRepository = charityImportRepository;
        }

        [Function(nameof(ClearStagingDataActivity))]
        public async Task Run([ActivityTrigger] IDurableActivityContext context, ILogger logger)
        {
            using var performanceLogger = new PerformanceLogger($"Clear staging data", logger);
            await _charityImportRepository.ClearStagingData();
        }
    }
}