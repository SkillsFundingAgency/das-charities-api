using Microsoft.Extensions.Logging;
using SFA.DAS.Charities.Data.Repositories;
using SFA.DAS.Charities.Domain;
using SFA.DAS.Charities.Domain.Entities;
using SFA.DAS.Charities.Import.Functions.LoadCharityCommissionsDataInToStaging.CharityCommissionModels;
using SFA.DAS.Charities.Import.Infrastructure;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;

namespace SFA.DAS.Charities.Import.Functions.LoadCharityCommissionsDataInToStaging.Activities
{
    public class LoadCharityDataInToStagingActivity
    {
        private readonly ICharitiesImportRepository _charityImportRepository;

        public LoadCharityDataInToStagingActivity(ICharitiesImportRepository charityImportRepository)
        {
            _charityImportRepository = charityImportRepository;
        }

        [Function(nameof(LoadCharityDataInToStagingActivity))]
        public async Task Run(
            [ActivityTrigger] string fileName,
            [BlobInput("charity-files/{fileName}", Connection = "CharitiesStorageConnectionString")] Stream fileStream,
            ILogger logger)
        {
            using var performanceLogger = new PerformanceLogger($"Load charities in staging", logger);

            var charityData = CharityCommissionDataHelper.ExtractData<CharityModel>(fileStream);

            var data = charityData.Select(x => (CharityStaging)x).ToList();

            await _charityImportRepository.BulkInsert(data);

            logger.LogInformation("Total charities {charitiesCount}", charityData.Count);
        }
    }
}
