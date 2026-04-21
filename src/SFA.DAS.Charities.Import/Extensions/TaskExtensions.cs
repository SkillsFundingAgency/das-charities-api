using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Import.Extensions;

[ExcludeFromCodeCoverage]
public static class TaskExtensions
{
    /// <summary>
    /// Creates a continuation that executes asynchronously after the specified task completes, passing its result to
    /// the provided function.
    /// </summary>
    /// <remarks>If the initial task is canceled or faults, the continuation will not be executed and the
    /// returned task will reflect the same cancellation or exception.</remarks>
    /// <typeparam name="TResult1">The type of the result produced by the initial task.</typeparam>
    /// <typeparam name="TResult2">The type of the result produced by the continuation function.</typeparam>
    /// <param name="task">The task whose result is passed to the continuation function. Cannot be null.</param>
    /// <param name="next">A function to execute when the initial task completes, which receives the result of the initial task and a
    /// cancellation token, and returns a task representing the continuation operation. Cannot be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the continuation operation.</param>
    /// <returns>A task that represents the asynchronous operation of the continuation function, containing its result.</returns>
    public static async Task<TResult2> Then<TResult1, TResult2>(this Task<TResult1> task, Func<TResult1, CancellationToken, Task<TResult2>> next, CancellationToken cancellationToken)
    {
        var result = await task;
        return await next(result, cancellationToken);
    }

    /// <summary>
    /// Asynchronously invokes a continuation function after the specified task completes, passing the task's result and
    /// a cancellation token to the continuation.
    /// </summary>
    /// <remarks>If the initial task is canceled or faults, the continuation function is not invoked and the
    /// returned task reflects the same status. This method is useful for chaining asynchronous operations that require
    /// access to the result of a previous task and support cancellation.</remarks>
    /// <typeparam name="TInput">The type of the result produced by the initial task.</typeparam>
    /// <param name="task">The task whose result is passed to the continuation function. Must not be null.</param>
    /// <param name="next">A function to execute after the task completes, which receives the task's result and a cancellation token. Must
    /// not be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the continuation operation.</param>
    /// <returns>A task that represents the asynchronous execution of the continuation function.</returns>
    public static async Task Then<TInput>(this Task<TInput> task, Func<TInput, CancellationToken, Task> next, CancellationToken cancellationToken)
    {
        var result = await task;
        await next(result, cancellationToken);
    }
}
