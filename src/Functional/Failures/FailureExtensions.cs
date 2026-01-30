namespace UnambitiousFx.Functional.Failures;

/// <summary>
///     Extension methods for working with errors.
/// </summary>
public static class FailureExtensions
{
    /// <summary>
    ///     Converts an <see cref="IFailure" /> to an <see cref="Exception" />.
    ///     If the error is an <see cref="ExceptionalFailure" />, returns the wrapped exception.
    ///     If the error is an <see cref="AggregateFailure" />, returns an <see cref="AggregateException" />.
    ///     Otherwise, wraps the error in a <see cref="FunctionalException" />.
    /// </summary>
    /// <param name="failure">The error to convert.</param>
    /// <returns>An exception representing the error.</returns>
    public static Exception ToException(this IFailure failure)
    {
        if (failure is ExceptionalFailure exceptionalError)
        {
            return exceptionalError.Exception;
        }

        if (failure is AggregateFailure aggregateError)
        {
            return new AggregateException(aggregateError.Errors.Select(x => x.ToException()));
        }

        return new FunctionalException(failure);
    }
}
