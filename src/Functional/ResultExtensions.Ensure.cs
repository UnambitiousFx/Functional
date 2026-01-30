using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Validates the result values with a predicate and returns a failure with the provided exception if validation fails.
    /// </summary>
    /// <typeparam name="TValue">Value type 1.</typeparam>
    /// <param name="result">The result instance.</param>
    /// <param name="predicate">The validation predicate.</param>
    /// <param name="errorFactory">Factory function to create an exception when validation fails.</param>
    /// <returns>The original result if validation succeeds; otherwise a failure result.</returns>
    public static Result<TValue> Ensure<TValue>(this Result<TValue> result, Func<TValue, bool> predicate,
        Func<TValue, Failure> errorFactory) where TValue : notnull
    {
        return result.Then(value => predicate(value)
            ? Result.Success(value)
            : Result.Failure<TValue>(errorFactory(value)));
    }
}