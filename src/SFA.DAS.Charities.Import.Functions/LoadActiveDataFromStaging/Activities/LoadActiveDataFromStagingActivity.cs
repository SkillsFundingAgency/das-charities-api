using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Import.Functions.LoadActiveDataFromStaging.Activities
{
    public class LoadActiveDataFromStagingActivity
    {
        [FunctionName(nameof(LoadActiveDataFromStagingActivity))]
        public static async Task LoadActiveDataFromStaging([ActivityTrigger] IDurableActivityContext context, ILogger logger)
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
        }

    }
}
