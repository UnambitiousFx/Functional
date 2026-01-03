using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Ensures at least one item in a collection result satisfies the given predicate.
    /// </summary>
    /// <typeparam name="TCollection">The collection type implementing IEnumerable&lt;TItem&gt;.</typeparam>
    /// <typeparam name="TItem">The item type in the collection.</typeparam>
    /// <param name="result">The result instance.</param>
    /// <param name="predicate">The predicate that at least one item must satisfy.</param>
    /// <param name="message">Optional validation error message.</param>
    /// <param name="field">Optional field name for the error message.</param>
    /// <returns>The original result if at least one item satisfies the predicate; otherwise a failure with ValidationError.</returns>
    public static Result<TCollection> EnsureAny<TCollection, TItem>(this Result<TCollection> result,
        Func<TItem, bool> predicate, string? message = null, string? field = null)
        where TCollection : IEnumerable<TItem>
    {
        return result.Then(collection =>
        {
            if (collection.Any(predicate))
            {
                return Result.Success(collection);
            }

            var defaultMessage = "At least one item must satisfy the validation condition.";
            var finalMessage = field is null
                ? message ?? defaultMessage
                : $"{field}: {message ?? defaultMessage}";

            return Result.Failure<TCollection>(new ValidationError([finalMessage]));
        });
    }

    /// <summary>
    ///     Ensures at least one item in a collection result satisfies the given predicate with custom error factory.
    /// </summary>
    /// <typeparam name="TCollection">The collection type implementing IEnumerable&lt;TItem&gt;.</typeparam>
    /// <typeparam name="TItem">The item type in the collection.</typeparam>
    /// <param name="result">The result instance.</param>
    /// <param name="predicate">The predicate that at least one item must satisfy.</param>
    /// <param name="errorFactory">Factory function to create an error when validation fails.</param>
    /// <returns>
    ///     The original result if at least one item satisfies the predicate; otherwise a failure with the error from the
    ///     factory.
    /// </returns>
    public static Result<TCollection> EnsureAny<TCollection, TItem>(this Result<TCollection> result,
        Func<TItem, bool> predicate, Func<TCollection, Error> errorFactory)
        where TCollection : IEnumerable<TItem>
    {
        return result.Then(collection =>
        {
            if (collection.Any(predicate))
            {
                return Result.Success(collection);
            }

            return Result.Failure<TCollection>(errorFactory(collection));
        });
    }

    /// <summary>
    ///     Ensures no items in a collection result satisfy the given predicate.
    /// </summary>
    /// <typeparam name="TCollection">The collection type implementing IEnumerable&lt;TItem&gt;.</typeparam>
    /// <typeparam name="TItem">The item type in the collection.</typeparam>
    /// <param name="result">The result instance.</param>
    /// <param name="predicate">The predicate that no items must satisfy.</param>
    /// <param name="message">Optional validation error message.</param>
    /// <param name="field">Optional field name for the error message.</param>
    /// <returns>The original result if no items satisfy the predicate; otherwise a failure with ValidationError.</returns>
    public static Result<TCollection> EnsureNone<TCollection, TItem>(this Result<TCollection> result,
        Func<TItem, bool> predicate, string? message = null, string? field = null)
        where TCollection : IEnumerable<TItem>
    {
        return result.Then(collection =>
        {
            if (!collection.Any(predicate))
            {
                return Result.Success(collection);
            }

            var defaultMessage = "No items must satisfy the validation condition.";
            var finalMessage = field is null
                ? message ?? defaultMessage
                : $"{field}: {message ?? defaultMessage}";

            return Result.Failure<TCollection>(new ValidationError([finalMessage]));
        });
    }
}
