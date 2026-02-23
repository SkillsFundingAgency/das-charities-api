using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Charities.Import.Functions;

[ExcludeFromCodeCoverage]
public class PingFunction
{
    private readonly ILogger<PingFunction> _logger;

    public PingFunction(ILogger<PingFunction> logger)
    {
        _logger = logger;
    }

    [Function("PingFunction")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}