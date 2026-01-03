using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional;

public static partial class MaybeExtensions
{
    /// <summary>
    ///     Converts a Maybe to a Result. If the Maybe is Some, returns Success with the value.
    ///     If the Maybe is None, returns Failure with the provided error.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="maybe">The maybe instance.</param>
    /// <param name="error">The error to use when the maybe is None.</param>
    /// <returns>A Result that is successful if the maybe is Some; otherwise a failure with the provided error.</returns>
    public static Result<TValue> ToResult<TValue>(this Maybe<TValue> maybe, Error error)
        where TValue : notnull
    {
        return maybe.Match(
            some: Result.Success,
            none: () => Result.Failure<TValue>(error));
    }

    /// <summary>
    ///     Converts a Maybe to a Result. If the Maybe is Some, returns Success with the value.
    ///     If the Maybe is None, returns Failure with an error created by the factory function.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="maybe">The maybe instance.</param>
    /// <param name="errorFactory">Factory function to create an error when the maybe is None.</param>
    /// <returns>A Result that is successful if the maybe is Some; otherwise a failure with the error from the factory.</returns>
    public static Result<TValue> ToResult<TValue>(this Maybe<TValue> maybe, Func<Error> errorFactory)
        where TValue : notnull
    {
        return maybe.Match(
            some: Result.Success,
            none: () => Result.Failure<TValue>(errorFactory()));
    }

    /// <summary>
    ///     Converts a Maybe to a Result. If the Maybe is Some, returns Success with the value.
    ///     If the Maybe is None, returns Failure with an error containing the provided message.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="maybe">The maybe instance.</param>
    /// <param name="message">The error message to use when the maybe is None.</param>
    /// <returns>A Result that is successful if the maybe is Some; otherwise a failure with the provided message.</returns>
    public static Result<TValue> ToResult<TValue>(this Maybe<TValue> maybe, string message)
        where TValue : notnull
    {
        return maybe.Match(
            some: Result.Success,
            none: () => Result.Failure<TValue>(message));
    }
}
