using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    internal static Maybe<TError> GetError<TError>(this IResult result)
        where TError : class, IError
    {
        if (result.IsSuccess)
        {
            return Maybe<TError>.None();
        }

        if (result.TryGet(out var error))
        {
            return Maybe<TError>.None();
        }

        if (error is AggregateError aggregateError)
        {
            foreach (var aggregateErrorError in aggregateError.Errors)
            {
                if (aggregateErrorError is TError typedError)
                {
                    return Maybe<TError>.Some(typedError);
                }
            }

            return Maybe<TError>.None();
        }

        {
            return error is TError typedError ? Maybe<TError>.Some(typedError) : Maybe<TError>.None();
        }
    }

    private static bool ContainsException<TException>(Error error)
        where TException : Exception
    {
        return error switch
        {
            ExceptionalError exErr => exErr.Exception is TException,
            AggregateError aggregate => aggregate.Errors.Any(ContainsException<TException>),
            _ => false
        };
    }

    private static bool ContainsException(Error error, Type errorType)
    {
        return error switch
        {
            ExceptionalError exErr => errorType.IsInstanceOfType(exErr.Exception),
            AggregateError aggregate => aggregate.Errors.Any(err => ContainsException(err, errorType)),
            _ => false
        };
    }

    private static bool ContainsError<TError>(Error error)
        where TError : Error
    {
        if (error is AggregateError aggregateError)
        {
            return aggregateError.Errors.Any(err => ContainsError<TError>(err));
        }

        return error is TError;
    }
}
