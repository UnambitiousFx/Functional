using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Ensures a successful string result value is neither null nor empty.
    /// </summary>
    /// <param name="result">The result instance.</param>
    /// <param name="message">Validation error message.</param>
    /// <param name="field">Optional field name for the error message.</param>
    /// <returns>The original result if the string is not empty; otherwise a failure with ValidationError.</returns>
    public static Result<string> EnsureNotEmpty(this Result<string> result, string message = "Value must not be empty.",
        string? field = null)
    {
        return result.Then(value =>
        {
            if (string.IsNullOrEmpty(value))
            {
                var finalMessage = field is null
                    ? message
                    : $"{field}: {message}";
                return Result.Failure<string>(new ValidationError([finalMessage]));
            }

            return Result.Success(value);
        });
    }

    /// <summary>
    ///     Ensures a successful enumerable result is not empty.
    /// </summary>
    /// <typeparam name="TCollection">The collection type implementing IEnumerable&lt;TItem&gt;.</typeparam>
    /// <typeparam name="TItem">The item type in the collection.</typeparam>
    /// <param name="result">The result instance.</param>
    /// <param name="message">Validation error message.</param>
    /// <param name="field">Optional field name for the error message.</param>
    /// <returns>The original result if the collection is not empty; otherwise a failure with ValidationError.</returns>
    public static Result<TCollection> EnsureNotEmpty<TCollection, TItem>(this Result<TCollection> result,
        string message = "Collection must not be empty.", string? field = null) where TCollection : IEnumerable<TItem>
    {
        return result.Then(collection =>
        {
            if (!collection.Any())
            {
                var finalMessage = field is null
                    ? message
                    : $"{field}: {message}";
                return Result.Failure<TCollection>(new ValidationError([finalMessage]));
            }

            return Result.Success(collection);
        });
    }
}