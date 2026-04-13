using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Import.Extensions;

[ExcludeFromCodeCoverage]
public static class TaskExtensions
{
    public static async Task<TResult> Then<T, TResult>(this Task<T> task, Func<T, Task<TResult>> next)
    {
        var result = await task;
        return await next(result);
    }

    public static async Task Then<T>(this Task<T> task, Func<T, Task> next)
    {
        var result = await task;
        await next(result);
    }
}
