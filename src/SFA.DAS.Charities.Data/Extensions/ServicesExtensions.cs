using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.Charities.Data.Extensions;

[ExcludeFromCodeCoverage]
public static class ServicesExtensions
{
    public static IServiceCollection AddCharityDataContext(this IServiceCollection services, string connectionString, string environmentName)
    {
        services.AddDbContext<CharitiesDataContext>((serviceProvider, options) =>
        {
            var connection = new SqlConnection(connectionString);

            options.UseSqlServer(
                connection,
                options => options
                    .CommandTimeout((int)TimeSpan.FromMinutes(5).TotalSeconds)
                    .EnableRetryOnFailure(3, TimeSpan.FromSeconds(10), null));
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
