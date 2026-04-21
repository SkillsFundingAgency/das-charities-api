using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SFA.DAS.Charities.Domain.Entities;
using SFA.DAS.Charities.Import.CharityCommissionModels;
using SFA.DAS.Charities.Import.Extensions;
using SFA.DAS.Charities.Import.Services;

namespace SFA.DAS.Charities.Import.Functions;

[ExcludeFromCodeCoverage]
public class ImportCharityDataFunction(
    ILogger<ImportCharityDataFunction> _logger,
    IConfiguration _configuration,
    ICharitiesImportService _charitiesImportService)
{
    [Function(nameof(ImportCharityDataFunction))]
    public async Task Run([TimerTrigger("%CharitiesDataImportTimerInterval%", RunOnStartup = false)] TimerInfo myTimer, CancellationToken cancellationToken)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var charityTrusteeFileName = _configuration["CharityTrusteeFileName"];
        var charityFileName = _configuration["CharityFileName"];

        await _charitiesImportService.DownloadFile(charityFileName, cancellationToken)
            .Then<Stream, Stream>(_charitiesImportService.ClearStagingData<CharityStaging>, cancellationToken)
            .Then<Stream, List<CharityModel>>(_charitiesImportService.ExtractDataFromStream<CharityModel>, cancellationToken)
            .Then<List<CharityModel>, List<CharityStaging>>(_charitiesImportService.MapToCharity, cancellationToken)
            .Then<List<CharityStaging>>(_charitiesImportService.BulkInsert, cancellationToken);

        await _charitiesImportService.DownloadFile(charityTrusteeFileName, cancellationToken)
            .Then<Stream, Stream>(_charitiesImportService.ClearStagingData<CharityTrusteeStaging>, cancellationToken)
            .Then<Stream, List<CharityTrusteeModel>>(_charitiesImportService.ExtractDataFromStream<CharityTrusteeModel>, cancellationToken)
            .Then<List<CharityTrusteeModel>, List<CharityTrusteeStaging>>(_charitiesImportService.MapToTrustee, cancellationToken)
            .Then<List<CharityTrusteeStaging>>(_charitiesImportService.BulkInsert, cancellationToken);

        await _charitiesImportService.LoadDataFromStagingToLive(cancellationToken);
    }
}
