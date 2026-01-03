namespace UnambitiousFx.Functional.ValueTasks;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Ensures a successful string result value (from ValueTask) is neither null nor empty.
    /// </summary>
    /// <param name="awaitableResult">The ValueTask of result to await.</param>
    /// <param name="message">Validation error message.</param>
    /// <param name="field">Optional field name for the error message.</param>
    /// <returns>A ValueTask with the validated result.</returns>
    public static async ValueTask<Result<string>> EnsureNotEmptyAsync(this ValueTask<Result<string>> awaitableResult,
        string message = "Value must not be empty.", string? field = null)
    {
        var result = await awaitableResult;
        return result.EnsureNotEmpty(message, field);
    }

    /// <summary>
    ///     Ensures a successful enumerable result (from ValueTask) is not empty.
    /// </summary>
    /// <typeparam name="TCollection">The collection type implementing IEnumerable&lt;TItem&gt;.</typeparam>
    /// <typeparam name="TItem">The item type in the collection.</typeparam>
    /// <param name="awaitableResult">The ValueTask of result to await.</param>
    /// <param name="message">Validation error message.</param>
    /// <param name="field">Optional field name for the error message.</param>
    /// <returns>A ValueTask with the validated result.</returns>
    public static async ValueTask<Result<TCollection>> EnsureNotEmptyAsync<TCollection, TItem>(
        this ValueTask<Result<TCollection>> awaitableResult, string message = "Collection must not be empty.",
        string? field = null) where TCollection : IEnumerable<TItem>
    {
        var result = await awaitableResult;
        return result.EnsureNotEmpty<TCollection, TItem>(message, field);
    }
}