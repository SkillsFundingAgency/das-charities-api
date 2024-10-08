﻿using System.Collections.Generic;
using System.IO;
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
    public class LoadCharityTrusteeDataInToStagingActivity
    {
        private readonly ICharitiesImportRepository _charityTrusteeStagingRepository;
        private readonly ICharityCommissionDataHelper _dataHelper;

        public LoadCharityTrusteeDataInToStagingActivity(ICharitiesImportRepository charityTrusteeStagingRepository, ICharityCommissionDataHelper dataHelper)
        {
            _charityTrusteeStagingRepository = charityTrusteeStagingRepository;
            _dataHelper = dataHelper;
        }

        [FunctionName(nameof(LoadCharityTrusteeDataInToStagingActivity))]
        public async Task Run(
            [ActivityTrigger] string fileName,
            [Blob("charity-files/{fileName}", FileAccess.Read, Connection = "CharitiesStorageConnectionString")] Stream fileStream,
            ILogger logger)
        {
            using var performanceLogger = new PerformanceLogger($"Load trustees in staging", logger);

            var trusteeData = _dataHelper.ExtractDataStream<CharityTrusteeModel>(fileStream);

            var batchSize = 1000;
            var batch = new List<CharityTrusteeStaging>();

            foreach (var trustee in trusteeData)
            {
                batch.Add((CharityTrusteeStaging)trustee);

                if (batch.Count >= batchSize)
                {
                    await _charityTrusteeStagingRepository.BulkInsert(batch);
                    batch.Clear();
                }
            }

            // Insert any remaining records
            if (batch.Count > 0)
            {
                await _charityTrusteeStagingRepository.BulkInsert(batch);
            }

            logger.LogInformation("Finished loading Trustees into Staging");
        }
    }
}
