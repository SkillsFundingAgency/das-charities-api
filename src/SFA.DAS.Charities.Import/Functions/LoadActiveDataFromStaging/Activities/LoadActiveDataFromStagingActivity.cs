using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using SFA.DAS.Charities.Data.Repositories;
using SFA.DAS.Charities.Domain;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Import.Functions.LoadActiveDataFromStaging.Activities
{
    public class LoadActiveDataFromStagingActivity
    {
        private readonly ICharitiesImportRepository _charityImportRepository;

        public LoadActiveDataFromStagingActivity(ICharitiesImportRepository charityImportRepository)
        {
            _charityImportRepository = charityImportRepository;
        }
        [FunctionName(nameof(LoadActiveDataFromStagingActivity))]
        public async Task LoadActiveDataFromStaging([ActivityTrigger] IDurableActivityContext context, ILogger logger)
        {
            using var performanceLogger = new PerformanceLogger($"Load staging data to live", logger);
            logger.LogInformation("Starting LoadActiveDataFromStagingActivity");
            await _charityImportRepository.LoadDataFromStagingInToLive();
            logger.LogInformation("Finishing LoadActiveDataFromStagingActivity");
        }

    }
}
