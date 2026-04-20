using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Asp.Versioning;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SFA.DAS.Api.Common.AppStart;
using SFA.DAS.Api.Common.Configuration;
using SFA.DAS.Api.Common.Infrastructure;
using SFA.DAS.Charities.Data;
using SFA.DAS.Charities.Data.Extensions;
using SFA.DAS.Charities.Data.Repositories;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.Charities.Api;

public class Startup
{
    public const string EnvironmentAppSettingName = "Environment";
    private readonly IConfigurationRoot _configuration;
    private readonly string _initialEnvironment;
    private bool _isRunningAcceptanceTests => _initialEnvironment.Equals("DEV", StringComparison.CurrentCultureIgnoreCase);
    private bool IsEnvironmentLocalOrDev =>
        _initialEnvironment.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase) ||
        _isRunningAcceptanceTests;

    public Startup(IConfiguration configuration)
    {
        var config = new ConfigurationBuilder().AddConfiguration(configuration);

        _initialEnvironment = configuration[EnvironmentAppSettingName];

        if (!_isRunningAcceptanceTests)
        {
            config.AddAzureTableStorage(options =>
                {
                    options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                    options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                    options.EnvironmentName = configuration[EnvironmentAppSettingName];
                    options.PreFixConfigurationKeys = false;
                });
        }

        _configuration = config.Build();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        if (!IsEnvironmentLocalOrDev)
        {
            var azureAdConfiguration = _configuration
                .GetSection("AzureAd")
                .Get<AzureActiveDirectoryConfiguration>();

            var policies = new Dictionary<string, string>
            {
                {PolicyNames.Default, "Default"}
            };

            services.AddAuthentication(azureAdConfiguration, policies);

            services
                .AddHealthChecks()
                .AddDbContextCheck<CharitiesDataContext>();
        }

        if (!_isRunningAcceptanceTests)
            services.AddCharityDataContext(_configuration["SqlDatabaseConnectionString"], _initialEnvironment);

        var connStr = _configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
        if (!string.IsNullOrWhiteSpace(connStr))
            services.AddOpenTelemetry().UseAzureMonitor(o => o.ConnectionString = connStr);

        services.AddApiVersioning(opt =>
        {
            opt.ApiVersionReader = new HeaderApiVersionReader("X-Version");
            opt.ReportApiVersions = true;
        });

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "CharitiesAPI", Version = "v1" });
        });

        services.AddTransient<ICharitiesReadRepository, CharitiesReadRepository>();

        services
            .AddControllers(o =>
            {
                if (!IsEnvironmentLocalOrDev)
                {
                    o.Conventions.Add(new AuthorizeControllerModelConvention(new List<string>()));
                }
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });


        services.AddLogging();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = string.Empty;
        });

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();

        if (!IsEnvironmentLocalOrDev)
        {
            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = HealthCheckResponseWriter.WriteJsonResponse
            });

            app.UseHealthChecks("/ping", new HealthCheckOptions
            {
                Predicate = (_) => false,
                ResponseWriter = (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync("");
                }
            });
        }

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
