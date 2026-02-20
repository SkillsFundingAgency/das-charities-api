using System.Threading;
using System.Threading.Tasks;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;

namespace SFA.DAS.Charities.Import.Services;

public interface IDurableTaskClientWrapper
{
    Task<string> ScheduleNewOrchestrationInstanceAsync(TaskName orchestratorName, object input = null, StartOrchestrationOptions options = null,
        CancellationToken cancellation = new());

    Task<OrchestrationMetadata> GetInstanceAsync(string instanceId, bool getInputsAndOutputs = false,
        CancellationToken cancellation = new());

    DurableTaskClient Client { get; set; }
}

public class DurableTaskClientWrapper : IDurableTaskClientWrapper
{
    public DurableTaskClient Client { get; set; }

    public Task<OrchestrationMetadata> GetInstanceAsync(string instanceId, bool getInputsAndOutputs = false, CancellationToken cancellation = default)
    {
        return Client.GetInstanceAsync(instanceId, getInputsAndOutputs, cancellation);
    }

    public Task<string> ScheduleNewOrchestrationInstanceAsync(TaskName orchestratorName, object input = null, StartOrchestrationOptions options = null, CancellationToken cancellation = default)
    {
        return Client.ScheduleNewOrchestrationInstanceAsync(orchestratorName, input, options, cancellation);
    }
}