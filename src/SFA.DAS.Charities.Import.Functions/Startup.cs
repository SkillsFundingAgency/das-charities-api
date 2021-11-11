using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Charities.Import.Functions;
using SFA.DAS.Charities.Import.Functions.Infrastructure;

[assembly: FunctionsStartup(typeof(Startup))]

namespace SFA.DAS.Charities.Import.Functions
{
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddTransient<ITimeProvider, TimeProvider>();
        }
    }
}
