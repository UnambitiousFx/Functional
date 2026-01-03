namespace UnambitiousFx.Functional.Tasks;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Ensures a successful string result value (from Task) is neither null nor empty.
    /// </summary>
    /// <param name="awaitableResult">The Task of result to await.</param>
    /// <param name="message">Validation error message.</param>
    /// <param name="field">Optional field name for the error message.</param>
    /// <returns>A Task with the validated result.</returns>
    public static async Task<Result<string>> EnsureNotEmptyAsync(this Task<Result<string>> awaitableResult,
        string message = "Value must not be empty.", string? field = null)
    {
        var result = await awaitableResult;
        return result.EnsureNotEmpty(message, field);
    }

    /// <summary>
    ///     Ensures a successful enumerable result (from Task) is not empty.
    /// </summary>
    /// <typeparam name="TCollection">The collection type implementing IEnumerable&lt;TItem&gt;.</typeparam>
    /// <typeparam name="TItem">The item type in the collection.</typeparam>
    /// <param name="awaitableResult">The Task of result to await.</param>
    /// <param name="message">Validation error message.</param>
    /// <param name="field">Optional field name for the error message.</param>
    /// <returns>A Task with the validated result.</returns>
    public static async Task<Result<TCollection>> EnsureNotEmptyAsync<TCollection, TItem>(
        this Task<Result<TCollection>> awaitableResult, string message = "Collection must not be empty.",
        string? field = null) where TCollection : IEnumerable<TItem>
    {
        var result = await awaitableResult;
        return result.EnsureNotEmpty<TCollection, TItem>(message, field);
    }
}