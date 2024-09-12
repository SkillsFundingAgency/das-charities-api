using System;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.Charities.Data.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddCharityDataContext(this IServiceCollection services, string connectionString, string environmentName)
        {
            services.AddDbContext<CharitiesDataContext>((serviceProvider, options) =>
            {
                var connection = new SqlConnection(connectionString);

                if (!environmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
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
}
