using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Data.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddCharityDataContext(this IServiceCollection services, IConfiguration configuration) // string connectionString, string environmentName)
    {
        var connectionString = configuration["SqlDatabaseConnectionString"]!;
        var environment = configuration["EnvironmentName"]!;

        services.AddDbContext<CharitiesDataContext>((serviceProvider, options) =>
        {
            var connection = new SqlConnection(connectionString);

            if (!environment.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
            {
                var generateTokenTask = GenerateTokenAsync();
                connection.AccessToken = generateTokenTask.GetAwaiter().GetResult();
            }

            options.UseSqlServer(
                connection,
                options => options.CommandTimeout((int)TimeSpan.FromMinutes(5).TotalSeconds));
        });
        return services;
    }

    public static async Task<string> GenerateTokenAsync()
    {
        const string AzureResource = "https://database.windows.net/";
        var azureServiceTokenProvider = new AzureServiceTokenProvider();
        var accessToken = await azureServiceTokenProvider.GetAccessTokenAsync(AzureResource);

        return accessToken;
    }
}
