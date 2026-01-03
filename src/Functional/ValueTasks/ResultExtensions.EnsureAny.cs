using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.ValueTasks;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Ensures at least one item in a ValueTask-wrapped collection result satisfies the given predicate.
    /// </summary>
    /// <typeparam name="TCollection">The collection type implementing IEnumerable&lt;TItem&gt;.</typeparam>
    /// <typeparam name="TItem">The item type in the collection.</typeparam>
    /// <param name="resultTask">The ValueTask-wrapped result instance.</param>
    /// <param name="predicate">The predicate that at least one item must satisfy.</param>
    /// <param name="message">Optional validation error message.</param>
    /// <param name="field">Optional field name for the error message.</param>
    /// <returns>The original result if at least one item satisfies the predicate; otherwise a failure with ValidationError.</returns>
    public static async ValueTask<Result<TCollection>> EnsureAny<TCollection, TItem>(
        this ValueTask<Result<TCollection>> resultTask,
        Func<TItem, bool> predicate,
        string? message = null,
        string? field = null)
        where TCollection : IEnumerable<TItem>
    {
        var result = await resultTask;
        return result.EnsureAny(predicate, message, field);
    }

    /// <summary>
    ///     Ensures at least one item in a ValueTask-wrapped collection result satisfies the given predicate with custom error
    ///     factory.
    /// </summary>
    /// <typeparam name="TCollection">The collection type implementing IEnumerable&lt;TItem&gt;.</typeparam>
    /// <typeparam name="TItem">The item type in the collection.</typeparam>
    /// <param name="resultTask">The ValueTask-wrapped result instance.</param>
    /// <param name="predicate">The predicate that at least one item must satisfy.</param>
    /// <param name="errorFactory">Factory function to create an error when validation fails.</param>
    /// <returns>
    ///     The original result if at least one item satisfies the predicate; otherwise a failure with the error from the
    ///     factory.
    /// </returns>
    public static async ValueTask<Result<TCollection>> EnsureAny<TCollection, TItem>(
        this ValueTask<Result<TCollection>> resultTask,
        Func<TItem, bool> predicate,
        Func<TCollection, Error> errorFactory)
        where TCollection : IEnumerable<TItem>
    {
        var result = await resultTask;
        return result.EnsureAny(predicate, errorFactory);
    }

    /// <summary>
    ///     Ensures no items in a ValueTask-wrapped collection result satisfy the given predicate.
    /// </summary>
    /// <typeparam name="TCollection">The collection type implementing IEnumerable&lt;TItem&gt;.</typeparam>
    /// <typeparam name="TItem">The item type in the collection.</typeparam>
    /// <param name="resultTask">The ValueTask-wrapped result instance.</param>
    /// <param name="predicate">The predicate that no items must satisfy.</param>
    /// <param name="message">Optional validation error message.</param>
    /// <param name="field">Optional field name for the error message.</param>
    /// <returns>The original result if no items satisfy the predicate; otherwise a failure with ValidationError.</returns>
    public static async ValueTask<Result<TCollection>> EnsureNone<TCollection, TItem>(
        this ValueTask<Result<TCollection>> resultTask, Func<TItem, bool> predicate, string? message = null,
        string? field = null)
        where TCollection : IEnumerable<TItem>
    {
        var result = await resultTask;
        return result.EnsureNone(predicate, message, field);
    }
}
