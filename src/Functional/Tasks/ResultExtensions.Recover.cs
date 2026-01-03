using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.Tasks;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Asynchronously recovers from a failed result by providing fallback values through a recovery function.
    /// </summary>
    /// <typeparam name="TValue1">The type of the first value.</typeparam>
    /// <param name="awaitableResult">The awaitable result to recover from.</param>
    /// <param name="recover">The async function that takes the errors and returns fallback values.</param>
    /// <returns>
    ///     A task with a successful result containing the fallback values if the original result failed; otherwise, the
    ///     original result.
    /// </returns>
    public static async Task<Result<TValue1>> RecoverAsync<TValue1>(
        this Task<Result<TValue1>> awaitableResult, Func<Error, Task<TValue1>> recover)
        where TValue1 : notnull
    {
        var result = await awaitableResult;

        if (result.TryGet(out _, out var error))
        {
            return result;
        }

        var fallback = await recover(error);
        return Result.Success(fallback);
    }
}
