using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional;

public static partial class ResultTaskExtensions
{
    /// <summary>
    ///     Validates the result values with a predicate and returns a failure with the provided exception if validation fails.
    /// </summary>
    /// <typeparam name="TValue">The type of the result value.</typeparam>
    /// <param name="result">The result instance to validate.</param>
    /// <param name="predicate">The asynchronous validation predicate to evaluate the result.</param>
    /// <param name="errorFactory">The asynchronous factory function to create an error when validation fails.</param>
    /// <returns>The original result if validation succeeds; otherwise a failure result.</returns>
    public static ResultTask<TValue> Ensure<TValue>(this ResultTask<TValue> result,
        Func<TValue, bool> predicate,
        Func<TValue, Failure> errorFactory) where TValue : notnull
    {
        return EnsureCore(result, predicate, errorFactory).AsAsync();

        static async ValueTask<Result<TValue>> EnsureCore(ResultTask<TValue> self,
            Func<TValue, bool> predicate,
            Func<TValue, Failure> errorFactory)
        {
            var source = await self;
            if (!source.TryGetValue(out var value))
            {
                return source;
            }

            if (predicate(value))
            {
                return source;
            }

            return Result.Failure<TValue>(errorFactory(value)).WithMetadata(source.Metadata);
        }
    }

    /// <summary>
    ///     Ensures that the result value satisfies a specified predicate, and applies an error factory function when it does
    ///     not.
    /// </summary>
    /// <typeparam name="TValue">The type of the result value.</typeparam>
    /// <param name="result">The result to validate.</param>
    /// <param name="predicate">The asynchronous predicate to evaluate the value.</param>
    /// <param name="errorFactory">An asynchronous function to create an error if validation fails.</param>
    /// <returns>
    ///     A result that contains the original value if validation succeeds; otherwise, a failure with the generated
    ///     error.
    /// </returns>
    public static ResultTask<TValue> Ensure<TValue>(this ResultTask<TValue> result,
        Func<TValue, ValueTask<bool>> predicate,
        Func<TValue, ValueTask<Failure>> errorFactory) where TValue : notnull
    {
        return EnsureCore(result, predicate, errorFactory).AsAsync();

        static async ValueTask<Result<TValue>> EnsureCore(ResultTask<TValue> self,
            Func<TValue, ValueTask<bool>> predicate,
            Func<TValue, ValueTask<Failure>> errorFactory)
        {
            var source = await self;
            if (!source.TryGetValue(out var value))
            {
                return source;
            }

            if (await predicate(value))
            {
                return source;
            }

            var error = await errorFactory(value);
            return Result.Failure<TValue>(error).WithMetadata(source.Metadata);
        }
    }
}