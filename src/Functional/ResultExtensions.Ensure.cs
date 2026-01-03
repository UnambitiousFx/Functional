using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Validates the result values with a predicate and returns a failure with the provided exception if validation fails.
    /// </summary>
    /// <typeparam name="TValue1">Value type 1.</typeparam>
    /// <param name="result">The result instance.</param>
    /// <param name="predicate">The validation predicate.</param>
    /// <param name="errorFactory">Factory function to create an exception when validation fails.</param>
    /// <returns>The original result if validation succeeds; otherwise a failure result.</returns>
    public static Result<TValue1> Ensure<TValue1>(this Result<TValue1> result, Func<TValue1, bool> predicate,
        Func<TValue1, Error> errorFactory) where TValue1 : notnull
    {
        return result.Then(value => predicate(value)
            ? Result.Success(value)
            : Result.Failure<TValue1>(errorFactory(value)));
    }

}