using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.ValueTasks;

public static partial class MaybeExtensions
{
    /// <summary>
    ///     Converts a ValueTask-wrapped Maybe to a Result. If the Maybe is Some, returns Success with the value.
    ///     If the Maybe is None, returns Failure with the provided error.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="maybeTask">The ValueTask-wrapped maybe instance.</param>
    /// <param name="error">The error to use when the maybe is None.</param>
    /// <returns>A Result that is successful if the maybe is Some; otherwise a failure with the provided error.</returns>
    public static async ValueTask<Result<TValue>> ToResult<TValue>(this ValueTask<Maybe<TValue>> maybeTask,
        Error error)
        where TValue : notnull
    {
        var maybe = await maybeTask;
        return maybe.ToResult(error);
    }

    /// <summary>
    ///     Converts a ValueTask-wrapped Maybe to a Result. If the Maybe is Some, returns Success with the value.
    ///     If the Maybe is None, returns Failure with an error created by the factory function.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="maybeTask">The ValueTask-wrapped maybe instance.</param>
    /// <param name="errorFactory">Factory function to create an error when the maybe is None.</param>
    /// <returns>A Result that is successful if the maybe is Some; otherwise a failure with the error from the factory.</returns>
    public static async ValueTask<Result<TValue>> ToResult<TValue>(this ValueTask<Maybe<TValue>> maybeTask,
        Func<Error> errorFactory)
        where TValue : notnull
    {
        var maybe = await maybeTask;
        return maybe.ToResult(errorFactory);
    }

    /// <summary>
    ///     Converts a ValueTask-wrapped Maybe to a Result. If the Maybe is Some, returns Success with the value.
    ///     If the Maybe is None, returns Failure with an error containing the provided message.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="maybeTask">The ValueTask-wrapped maybe instance.</param>
    /// <param name="message">The error message to use when the maybe is None.</param>
    /// <returns>A Result that is successful if the maybe is Some; otherwise a failure with the provided message.</returns>
    public static async ValueTask<Result<TValue>> ToResult<TValue>(this ValueTask<Maybe<TValue>> maybeTask,
        string message)
        where TValue : notnull
    {
        var maybe = await maybeTask;
        return maybe.ToResult(message);
    }
}
