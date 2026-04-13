using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Charities.Import.Services;

namespace SFA.DAS.Charities.Import.Functions;

public class ImportCharityDataFunction(ILogger<ImportCharityDataFunction> _logger, ICharitiesImportService _charitiesImportService)
{
    [Function("ImportCharityDataFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req, CancellationToken cancellationToken)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        await _charitiesImportService.ImportData(cancellationToken);

        return new OkObjectResult("All done");
    }
}