﻿using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using SFA.DAS.Charities.Data.Repositories;
using SFA.DAS.Charities.Domain.Entities;
using SFA.DAS.Charities.Import.Functions.LoadChairtyCommissionsDataInToStaging.CharityCommissionModels;
using SFA.DAS.Charities.Import.Infrastructure;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Import.Functions.LoadChairtyCommissionsDataInToStaging.Activities
{
    internal class LoadCharityTrusteeDataInToStagingActivity
    {
        private readonly ICharityTrusteeStagingRepository _charityTrusteeStagingRepository;

        public LoadCharityTrusteeDataInToStagingActivity(ICharityTrusteeStagingRepository charityTrusteeStagingRepository)
        {
            _charityTrusteeStagingRepository = charityTrusteeStagingRepository;
        }

        [FunctionName(nameof(LoadCharityTrusteeDataInToStagingActivity))]
        public async Task Run(
            [ActivityTrigger] string fileName, 
            [Blob("charity-files/{fileName}", FileAccess.Read, Connection = "CharitiesStorageConnectionString")] Stream fileStream,
            ILogger logger)
        {

            var trusteeData = CharityCommissionDataHelper.ExtractData<CharityTrusteeModel>(fileStream);

            var data = trusteeData.Select(x => (CharityTrusteeStaging)x).ToList();

            await _charityTrusteeStagingRepository.BulkInsert(data);

            logger.LogWarning($"Total trustees {trusteeData.Count}");
        }
    }
}