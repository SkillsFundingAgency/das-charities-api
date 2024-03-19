using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using SFA.DAS.Charities.Data.Extensions;
using SFA.DAS.Charities.Import;
using SFA.DAS.Charities.Import.Extensions;

var nLogConfiguration = new NLogConfiguration();

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(
        builder =>
        {
            var config = builder.AddConfiguration();
            /// how to inject this config into context.Configuration??
        })
    .ConfigureServices(
        (context, services) =>
        {
            services
                .Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), context.Configuration))  //MFCMFC last thing added
                .AddOptions()
                .AddCharityDataContext(context.Configuration)
                .AddApplicationRegistrations()
                .RegisterCharityCommissionsHttpClient(context.Configuration)
                .AddLogging(options =>
                {
                    options.AddFilter(typeof(Program).Namespace, LogLevel.Information);
                    options.SetMinimumLevel(LogLevel.Trace);
                    options.AddNLog(new NLogProviderOptions
                    {
                        CaptureMessageTemplates = true,
                        CaptureMessageProperties = true
                    });
                    options.AddConsole();

                    nLogConfiguration.ConfigureNLog();
                })
                .AddAzureClients(clientBuilder =>
                    clientBuilder.AddBlobServiceClient(context.Configuration.GetConnectionString("CharitiesStorageConnectionString"))
                    );
        })
    .Build();

await host.RunAsync();