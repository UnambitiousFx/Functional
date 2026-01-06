namespace UnambitiousFx.Functional.Tasks;

public static partial class ResultExtensions
{
 /// <summary>
    ///     Asynchronously appends a suffix to the error message of the first error in the result.
    /// </summary>
    /// <param name="result">The result to append error message to.</param>
    /// <param name="suffix">The suffix to append to the error message.</param>
    /// <returns>
    ///     A task with a new result containing the appended error message if the original result failed, otherwise the
    ///     original successful result.
    /// </returns>
    public static Task<Result> AppendErrorAsync(this Result result, string suffix)
    {
        if (string.IsNullOrEmpty(suffix) || result.IsSuccess)
        {
            return Task.FromResult(result);
        }

        return Task.FromResult(result.MapError(error => error with { Message = error.Message + suffix }));
    }

    /// <summary>
    ///     Asynchronously appends a suffix to the error message of the first error in the result.
    /// </summary>
    /// <param name="awaitableResult">The awaitable result to append error message to.</param>
    /// <param name="suffix">The suffix to append to the error message.</param>
    /// <returns>
    ///     A task with a new result containing the appended error message if the original result failed, otherwise the
    ///     original successful result.
    /// </returns>
    public static async Task<Result> AppendErrorAsync(this Task<Result> awaitableResult, string suffix)
    {
        var result = await awaitableResult;
        return result.AppendError(suffix);
    }

    /// <summary>
    ///     Asynchronously appends a suffix to the error message of the first error in the result.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <param name="result">The result to append error message to.</param>
    /// <param name="suffix">The suffix to append to the error message.</param>
    /// <returns>
    ///     A task with a new result containing the appended error message if the original result failed, otherwise the
    ///     original successful result.
    /// </returns>
    public static Task<Result<T1>> AppendErrorAsync<T1>(this Result<T1> result, string suffix) where T1 : notnull
    {
        if (string.IsNullOrEmpty(suffix) || result.IsSuccess)
        {
            return Task.FromResult(result);
        }

        return Task.FromResult(result.MapError(error => error with { Message = error.Message + suffix }));
    }

    /// <summary>
    ///     Asynchronously appends a suffix to the error message of the first error in the result.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <param name="awaitableResult">The awaitable result to append error message to.</param>
    /// <param name="suffix">The suffix to append to the error message.</param>
    /// <returns>
    ///     A task with a new result containing the appended error message if the original result failed, otherwise the
    ///     original successful result.
    /// </returns>
    public static async Task<Result<T1>> AppendErrorAsync<T1>(this Task<Result<T1>> awaitableResult,
        string suffix) where T1 : notnull
    {
        var result = await awaitableResult;
        return result.AppendError(suffix);
    }
}