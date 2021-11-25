using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using SFA.DAS.Charities.Data.Repositories;
using SFA.DAS.Charities.Domain;
using SFA.DAS.Charities.Domain.Entities;
using SFA.DAS.Charities.Import.Functions.LoadCharityCommissionsDataInToStaging.CharityCommissionModels;
using SFA.DAS.Charities.Import.Infrastructure;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Import.Functions.LoadCharityCommissionsDataInToStaging.Activities
{
    public class LoadCharityTrusteeDataInToStagingActivity
    {
        private readonly ICharityImportRepository _charityTrusteeStagingRepository;

        public LoadCharityTrusteeDataInToStagingActivity(ICharityImportRepository charityTrusteeStagingRepository)
        {
            _charityTrusteeStagingRepository = charityTrusteeStagingRepository;
        }

        [FunctionName(nameof(LoadCharityTrusteeDataInToStagingActivity))]
        public async Task Run(
            [ActivityTrigger] string fileName,
            [Blob("charity-files/{fileName}", FileAccess.Read, Connection = "CharitiesStorageConnectionString")] Stream fileStream,
            ILogger logger)
        {
            using var performanceLogger = new PerformanceLogger($"Load trustees in staging", logger);

            var trusteeData = CharityCommissionDataHelper.ExtractData<CharityTrusteeModel>(fileStream);

            var data = trusteeData.Select(x => (CharityTrusteeStaging)x).ToList();

            await _charityTrusteeStagingRepository.BulkInsert(data);

            logger.LogInformation("Total trustees {trusteesCount}", trusteeData.Count);
        }
    }
}
