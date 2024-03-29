﻿using Microsoft.Azure.WebJobs;
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

            var charityData = CharityCommissionDataHelper.ExtractData<CharityModel>(fileStream);

            var data = charityData.Select(x => (CharityStaging)x).ToList();

            await _charityImportRepository.BulkInsert(data);

            logger.LogInformation("Total charities {charitiesCount}", charityData.Count);
        }
    }
}
