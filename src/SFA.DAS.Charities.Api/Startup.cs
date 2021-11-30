using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Charities.Data.Extensions;
using SFA.DAS.Charities.Data.Repositories;
using SFA.DAS.Configuration.AzureTableStorage;

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
            var connectionString = _configuration["SqlDatabaseConnectionString"];
            var environment = _configuration["Environment"];

            services.AddApiVersioning(opt =>
            {
                opt.ApiVersionReader = new HeaderApiVersionReader("X-Version");
                opt.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
            });

            services
                .AddHealthChecks()
                .AddSqlServer(connectionString);

            services.AddCharityDataContext(connectionString, environment);

            services.AddMvc();

            services.AddTransient<ICharitiesReadRepository, CharitiesReadRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseHealthChecks("/health");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}