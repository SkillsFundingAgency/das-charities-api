using System.Diagnostics.CodeAnalysis;
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

/// <summary>
/// Provides a wrapper for a DurableTaskClient instance, enabling orchestration management operations through the
/// IDurableTaskClientWrapper interface.
/// </summary>
/// <remarks>This class is intended to abstract and encapsulate DurableTaskClient functionality, allowing for
/// easier testing and substitution in applications that interact with durable task orchestrations.</remarks>
[ExcludeFromCodeCoverage]
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