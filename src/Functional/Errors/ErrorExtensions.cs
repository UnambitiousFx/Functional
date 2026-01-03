namespace UnambitiousFx.Functional.Errors;

/// <summary>
///     Extension methods for working with errors.
/// </summary>
public static class ErrorExtensions
{
    /// <summary>
    ///     Converts an <see cref="IError" /> to an <see cref="Exception" />.
    ///     If the error is an <see cref="ExceptionalError" />, returns the wrapped exception.
    ///     If the error is an <see cref="AggregateError" />, returns an <see cref="AggregateException" />.
    ///     Otherwise, wraps the error in a <see cref="FunctionalException" />.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    /// <returns>An exception representing the error.</returns>
    public static Exception ToException(this IError error)
    {
        if (error is ExceptionalError exceptionalError)
        {
            return exceptionalError.Exception;
        }

        if (error is AggregateError aggregateError)
        {
            return new AggregateException(aggregateError.Errors.Select(x => x.ToException()));
        }

        return new FunctionalException(error);
    }
}
