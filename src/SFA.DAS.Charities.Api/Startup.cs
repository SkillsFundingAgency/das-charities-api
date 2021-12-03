using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SFA.DAS.Api.Common.AppStart;
using SFA.DAS.Api.Common.Configuration;
using SFA.DAS.Api.Common.Infrastructure;
using SFA.DAS.Charities.Data;
using SFA.DAS.Charities.Data.Extensions;
using SFA.DAS.Charities.Data.Repositories;
using SFA.DAS.Configuration.AzureTableStorage;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SFA.DAS.Charities.Api
{
    public class Startup
    {
        private IConfigurationRoot _configuration;
        public Startup(IConfiguration configuration)
        {
            var config = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .AddAzureTableStorage(options => 
                {
                    options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                    options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                    options.EnvironmentName = configuration["Environment"];
                    options.PreFixConfigurationKeys = false;
                });

            _configuration = config.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            if (!ConfigurationIsLocalOrDev())
            {
                var azureAdConfiguration = _configuration
                    .GetSection("AzureAd")
                    .Get<AzureActiveDirectoryConfiguration>();

                var policies = new Dictionary<string, string>
                {
                    {PolicyNames.Default, "Default"}
                };

                services.AddAuthentication(azureAdConfiguration, policies);
            }

            services.AddApiVersioning(opt =>
            {
                opt.ApiVersionReader = new HeaderApiVersionReader("X-Version");
                opt.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
            });

            services.AddCharityDataContext(_configuration["SqlDatabaseConnectionString"], _configuration["Environment"]);

            services
                .AddHealthChecks()
                .AddDbContextCheck<CharitiesDataContext>();


            services
                .AddControllers(o => o.Conventions.Add(new AuthorizeControllerModelConvention(new List<string>())))
                .AddJsonOptions(options => 
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.AddApplicationInsightsTelemetry(_configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);

            services.AddTransient<ICharitiesReadRepository, CharitiesReadRepository>();

            services.AddSwaggerGen(options => 
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "CharitiesAPI", Version = "v1" });
                options.OperationFilter<SwaggerVersionHeaderFilter>();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseHealthChecks("/health");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private bool ConfigurationIsLocalOrDev()
        {
            return _configuration["Environment"].Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase) ||
                   _configuration["Environment"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase);
        }
    }
}