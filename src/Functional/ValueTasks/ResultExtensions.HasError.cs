using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.ValueTasks;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Asynchronously determines whether the result contains an error of the specified type.
    /// </summary>
    /// <typeparam name="TError">The type of error to check for. Can be an error type or exception type.</typeparam>
    /// <param name="awaitableResult">The awaitable result to check for errors.</param>
    /// <returns>A task with true if the result contains an error of the specified type; otherwise, false.</returns>
    public static async ValueTask<bool> HasErrorAsync<TError>(this ValueTask<Result> awaitableResult)
        where TError : Error
    {
        var result = await awaitableResult;
        return result.HasError<TError>();
    }
}