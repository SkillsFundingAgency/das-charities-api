using System.Linq;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SFA.DAS.Charities.Data.Extensions;
using SFA.DAS.Charities.Import.Extensions;

var host = new HostBuilder().ConfigureFunctionsWebApplication();

host.ConfigureAppConfiguration(builder => builder.AddConfiguration());

host.ConfigureServices((context, services) =>
    {
        var connectionString = context.Configuration["SqlDatabaseConnectionString"];
        var environment = context.Configuration["EnvironmentName"];
        if (!context.HostingEnvironment.IsDevelopment())
        {
            services
                .AddOpenTelemetry()
                .UseAzureMonitor()
                .UseFunctionsWorkerDefaults();
        }

        services.AddCharityDataContext(connectionString, environment)
            .AddServiceRegistrations(context.Configuration);
    })
    .ConfigureLogging(logging =>
    {
        // This rule filters logs to capture only warnings and errors, removing this rule will allow Information logs to be captured
        logging.Services.Configure<LoggerFilterOptions>(options =>
        {
            LoggerFilterRule defaultRule = options.Rules.FirstOrDefault(rule => rule.ProviderName
                == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");
            if (defaultRule is not null)
            {
                options.Rules.Remove(defaultRule);
            }
        });
    });


await host.Build().RunAsync();
