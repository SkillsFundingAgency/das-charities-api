using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Charities.Import.Services;

namespace SFA.DAS.Charities.Import.Functions;

[ExcludeFromCodeCoverage]
public class ImportCharityDataFunction(ILogger<ImportCharityDataFunction> _logger, ICharitiesImportService _charitiesImportService)
{
    [Function(nameof(ImportCharityDataFunction))]
    public async Task Run([TimerTrigger("%CharitiesDataImportTimerInterval%")] TimerInfo myTimer, CancellationToken cancellationToken)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        await _charitiesImportService.ImportData(cancellationToken);
    }
}
