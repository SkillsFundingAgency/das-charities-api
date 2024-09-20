using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using SFA.DAS.Charities.Data.Repositories;
using SFA.DAS.Charities.Domain;
using SFA.DAS.Charities.Domain.Entities;
using SFA.DAS.Charities.Import.Functions.LoadCharityCommissionsDataInToStaging.CharityCommissionModels;
using SFA.DAS.Charities.Import.Infrastructure;

namespace SFA.DAS.Charities.Import.Functions.LoadCharityCommissionsDataInToStaging.Activities
{
    public class LoadCharityDataInToStagingActivity
    {
        private readonly ICharitiesImportRepository _charityImportRepository;

        public LoadCharityDataInToStagingActivity(ICharitiesImportRepository charityImportRepository)
        {
            _charityImportRepository = charityImportRepository;
        }

        [FunctionName(nameof(LoadCharityDataInToStagingActivity))]
        public async Task Run(
            [ActivityTrigger] string fileName,
            [Blob("charity-files/{fileName}", FileAccess.Read, Connection = "CharitiesStorageConnectionString")] Stream fileStream,
            ILogger logger)
        {
            using var performanceLogger = new PerformanceLogger($"Load charities in staging", logger);

            var charityData = CharityCommissionDataHelper.ExtractDataStream<CharityModel>(fileStream);

            var batchSize = 1000;
            var batch = new List<CharityStaging>();

            foreach (var charity in charityData)
            {
                batch.Add((CharityStaging)charity);

                if (batch.Count >= batchSize)
                {
                    await _charityImportRepository.BulkInsert(batch);
                    batch.Clear();
                }
            }

            // Insert any remaining records
            if (batch.Count > 0)
            {
                await _charityImportRepository.BulkInsert(batch);
            }

            logger.LogInformation("Total charities {charitiesCount}", charityData.Count());
        }
    }
}
