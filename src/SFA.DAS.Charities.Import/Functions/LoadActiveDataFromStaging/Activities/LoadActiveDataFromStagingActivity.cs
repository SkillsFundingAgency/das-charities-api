﻿using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using SFA.DAS.Charities.Data.Repositories;
using SFA.DAS.Charities.Domain;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Import.Functions.LoadActiveDataFromStaging.Activities;

public class LoadActiveDataFromStagingActivity
{
    private readonly ICharitiesImportRepository _charityImportRepository;

    public LoadActiveDataFromStagingActivity(ICharitiesImportRepository charityImportRepository)
    {
        _charityImportRepository = charityImportRepository;
    }
    [Function(nameof(LoadActiveDataFromStagingActivity))]
    //public async Task LoadActiveDataFromStaging([Microsoft.Azure.Functions.Worker.ActivityTrigger] IDurableActivityContext context, ILogger logger)
    public async Task LoadActiveDataFromStaging([ActivityTrigger] TaskOrchestrationContext context, ILogger logger)
    {
        // Does this need?
        //     var log = context.CreateReplaySafeLogger<ILogger>();
        //     using var performanceLogger = new PerformanceLogger($"Load staging data to live", log);
        using var performanceLogger = new PerformanceLogger("Load staging data to live", logger);
        await _charityImportRepository.LoadDataFromStagingInToLive();
    }

}
