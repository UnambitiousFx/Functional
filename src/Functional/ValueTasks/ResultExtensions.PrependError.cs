namespace UnambitiousFx.Functional.ValueTasks;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Asynchronously prepends a prefix to the error message of the first error in the result.
    /// </summary>
    /// <param name="result">The result to prepend error message to.</param>
    /// <param name="prefix">The prefix to prepend to the error message.</param>
    /// <returns>
    ///     A task with a new result containing the prepended error message if the original result failed, otherwise the
    ///     original successful result.
    /// </returns>
    public static ValueTask<Result> PrependErrorAsync(this Result result, string prefix)
    {
        if (string.IsNullOrEmpty(prefix) || result.IsSuccess)
        {
            return ValueTask.FromResult(result);
        }

        return ValueTask.FromResult(result.MapError(error => error with { Message = prefix + error.Message }));
    }

    /// <summary>
    ///     Asynchronously prepends a prefix to the error message of the first error in the result.
    /// </summary>
    /// <param name="awaitableResult">The awaitable result to prepend error message to.</param>
    /// <param name="prefix">The prefix to prepend to the error message.</param>
    /// <returns>
    ///     A task with a new result containing the prepended error message if the original result failed, otherwise the
    ///     original successful result.
    /// </returns>
    public static async ValueTask<Result> PrependErrorAsync(this ValueTask<Result> awaitableResult, string prefix)
    {
        var result = await awaitableResult;
        return result.PrependError(prefix);
    }

    /// <summary>
    ///     Asynchronously prepends a prefix to the error message of the first error in the result.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <param name="result">The result to prepend error message to.</param>
    /// <param name="prefix">The prefix to prepend to the error message.</param>
    /// <returns>
    ///     A task with a new result containing the prepended error message if the original result failed, otherwise the
    ///     original successful result.
    /// </returns>
    public static ValueTask<Result<T1>> PrependErrorAsync<T1>(this Result<T1> result, string prefix) where T1 : notnull
    {
        if (string.IsNullOrEmpty(prefix) || result.IsSuccess)
        {
            return ValueTask.FromResult(result);
        }

        return ValueTask.FromResult(result.MapError(error => error with { Message = prefix + error.Message }));
    }

    /// <summary>
    ///     Asynchronously prepends a prefix to the error message of the first error in the result.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <param name="awaitableResult">The awaitable result to prepend error message to.</param>
    /// <param name="prefix">The prefix to prepend to the error message.</param>
    /// <returns>
    ///     A task with a new result containing the prepended error message if the original result failed, otherwise the
    ///     original successful result.
    /// </returns>
    public static async ValueTask<Result<T1>> PrependErrorAsync<T1>(this ValueTask<Result<T1>> awaitableResult,
        string prefix) where T1 : notnull
    {
        var result = await awaitableResult;
        return result.PrependError(prefix);
    }
}
