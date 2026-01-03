namespace UnambitiousFx.Functional.ValueTasks;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Ensures a projected inner reference value (from ValueTask) is not null.
    /// </summary>
    /// <typeparam name="T">The result value type.</typeparam>
    /// <typeparam name="TInner">The inner reference type to check for null.</typeparam>
    /// <param name="awaitableResult">The ValueTask of result to await.</param>
    /// <param name="selector">Function to select the inner value to check.</param>
    /// <param name="message">Validation error message.</param>
    /// <param name="field">Optional field name for the error message.</param>
    /// <returns>A ValueTask with the validated result.</returns>
    public static async ValueTask<Result<T>> EnsureNotNullAsync<T, TInner>(this ValueTask<Result<T>> awaitableResult,
        Func<T, TInner?> selector, string message, string? field = null) where T : notnull where TInner : class
    {
        var result = await awaitableResult;
        return result.EnsureNotNull(selector, message, field);
    }
}