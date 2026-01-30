using AggregateFailure = UnambitiousFx.Functional.Failures.AggregateFailure;
using ConflictFailure = UnambitiousFx.Functional.Failures.ConflictFailure;
using ExceptionalFailure = UnambitiousFx.Functional.Failures.ExceptionalFailure;
using IFailure = UnambitiousFx.Functional.Failures.IFailure;
using NotFoundFailure = UnambitiousFx.Functional.Failures.NotFoundFailure;
using UnauthenticatedFailure = UnambitiousFx.Functional.Failures.UnauthenticatedFailure;
using UnauthorizedFailure = UnambitiousFx.Functional.Failures.UnauthorizedFailure;
using ValidationFailure = UnambitiousFx.Functional.Failures.ValidationFailure;

namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for instances implementing the <see cref="Result" /> interface.
/// </summary>
public static partial class ResultExtensions
{
    internal static Maybe<TError> GetError<TError>(this IResult result)
        where TError : class, IFailure
    {
        if (result.IsSuccess)
        {
            return Maybe<TError>.None();
        }

        if (result.TryGetError(out var error))
        {
            return Maybe<TError>.None();
        }

        if (error is AggregateFailure aggregateError)
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

    private static bool ContainsException<TException>(Failures.Failure failure)
        where TException : Exception
    {
        return failure switch
        {
            ExceptionalFailure exErr => exErr.Exception is TException,
            AggregateFailure aggregate => aggregate.Errors.Any(ContainsException<TException>),
            _ => false
        };
    }

    private static bool ContainsException(Failures.Failure failure, Type errorType)
    {
        return failure switch
        {
            ExceptionalFailure exErr => errorType.IsInstanceOfType(exErr.Exception),
            AggregateFailure aggregate => aggregate.Errors.Any(err => ContainsException(err, errorType)),
            _ => false
        };
    }

    private static bool ContainsError<TError>(Failures.Failure failure)
        where TError : Failures.Failure
    {
        if (failure is AggregateFailure aggregateError)
        {
            return aggregateError.Errors.Any(err => ContainsError<TError>(err));
        }

        return failure is TError;
    }

    extension(Result result)
    {
        /// <summary>
        ///     Creates a failure <see cref="Result" /> instance representing a "Not Found" error
        ///     for a specific resource and identifier, with an optional custom error message.
        /// </summary>
        /// <param name="resource">
        ///     The name of the resource that was not found. This is used to describe the nature
        ///     of the error in the default error message.
        /// </param>
        /// <param name="identifier">
        ///     The identifier of the resource that was not found. This is used to further specify
        ///     the error details in the default error message.
        /// </param>
        /// <param name="messageOverride">
        ///     An optional custom error message to override the generated default message.
        ///     If null, a default message is constructed based on the provided resource and identifier.
        /// </param>
        /// <returns>
        ///     A <see cref="Result" /> instance representing the failure, encapsulating a
        ///     <see cref="Failures.NotFoundFailure" /> that contains the error details.
        /// </returns>
        public static Result FailNotFound(string resource, string identifier, string? messageOverride = null)
        {
            return Result.Failure(new NotFoundFailure(resource, identifier, messageOverride));
        }

        /// <summary>
        ///     Creates a failure <see cref="Result{T}" /> instance representing a "Not Found" error
        ///     for a specific resource and identifier, with an optional custom error message.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the value that would have been encapsulated in the result if it succeeded.
        /// </typeparam>
        /// <param name="resource">
        ///     The name of the resource that was not found. This is used to describe the nature
        ///     of the error in the default error message.
        /// </param>
        /// <param name="identifier">
        ///     The identifier of the resource that was not found. This is used to further specify
        ///     the error details in the default error message.
        /// </param>
        /// <param name="messageOverride">
        ///     An optional custom error message to override the generated default message.
        ///     If null, a default message is constructed based on the provided resource and identifier.
        /// </param>
        /// <returns>
        ///     A <see cref="Result{T}" /> instance representing the failure, encapsulating a
        ///     <see cref="NotFoundFailure" /> that contains the error details.
        /// </returns>
        public static Result<T> FailNotFound<T>(string resource, string identifier, string? messageOverride = null)
            where T : notnull
        {
            return Result.Failure<T>(new NotFoundFailure(resource, identifier, messageOverride));
        }

        /// <summary>
        ///     Creates a failure <see cref="Result" /> instance representing a validation error
        ///     with a specific error message.
        /// </summary>
        /// <param name="message">
        ///     The message detailing the validation error. This provides the explanation
        ///     or description of what caused the validation to fail.
        /// </param>
        /// <returns>
        ///     A <see cref="Result" /> instance representing the failure, encapsulating a
        ///     <see cref="Failures.ValidationFailure" /> that includes the provided validation error message.
        /// </returns>
        public static Result FailValidation(string message)
        {
            return Result.Failure(new ValidationFailure([message]));
        }

        /// <summary>
        ///     Creates a failure <see cref="Result" /> instance indicating a validation error
        ///     with a specified error message.
        /// </summary>
        /// <param name="message">
        ///     The validation error message describing the reason for the failure.
        /// </param>
        /// <returns>
        ///     A <see cref="Result" /> instance representing the failure, encapsulating a
        ///     <see cref="ValidationFailure" /> containing the provided error message.
        /// </returns>
        public static Result<T> FailValidation<T>(string message) where T : notnull
        {
            return Result.Failure<T>(new ValidationFailure([message]));
        }

        /// <summary>
        ///     Creates a failure <see cref="Result" /> instance representing an unauthenticated error,
        ///     with an optional reason describing the context or details of the error.
        /// </summary>
        /// <param name="reason">
        ///     An optional reason string that provides additional context or description
        ///     about why authentication has failed. If null, a default reason is used.
        /// </param>
        /// <returns>
        ///     A <see cref="Result" /> instance representing the failure, containing an
        ///     <see cref="Failures.UnauthenticatedFailure" /> with the provided or default reason.
        /// </returns>
        public static Result FailUnauthenticated(string? reason)
        {
            return Result.Failure(new UnauthenticatedFailure(reason));
        }

        /// <summary>
        ///     Creates a failure <see cref="Result" /> instance representing an "Unauthenticated" error,
        ///     with an optional reason to describe the cause of the authentication failure.
        /// </summary>
        /// <param name="reason">
        ///     An optional reason that provides additional context for the unauthenticated error.
        ///     If null, a default reason is used.
        /// </param>
        /// <returns>
        ///     A <see cref="Result" /> instance encapsulating an <see cref="UnauthenticatedFailure" /> with the
        ///     specified reason or a default reason.
        /// </returns>
        public static Result<T> FailUnauthenticated<T>(string? reason) where T : notnull
        {
            return Result.Failure<T>(new UnauthenticatedFailure(reason));
        }

        /// <summary>
        ///     Creates a failure <see cref="Result" /> instance representing an "Unauthorized" error
        ///     with an optional reason describing the failure.
        /// </summary>
        /// <param name="reason">
        ///     An optional reason that explains why the operation is considered unauthorized.
        ///     If null, a default reason of "Unauthorized" is used.
        /// </param>
        /// <returns>
        ///     A <see cref="Result" /> instance representing the failure, encapsulating an
        ///     <see cref="Failures.UnauthorizedFailure" /> with the provided or default reason.
        /// </returns>
        public static Result FailUnauthorized(string? reason)
        {
            return Result.Failure(new UnauthorizedFailure(reason));
        }

        /// <summary>
        ///     Creates a failure <see cref="Result" /> instance representing an "Unauthorized" error
        ///     with an optional reason describing the unauthorized condition.
        /// </summary>
        /// <param name="reason">
        ///     An optional descriptive reason for the unauthorized error. If null, a default
        ///     error message is used.
        /// </param>
        /// <returns>
        ///     A <see cref="Result" /> instance encapsulating a <see cref="UnauthorizedFailure" /> that
        ///     conveys the unauthorized condition and its details.
        /// </returns>
        public static Result<T> FailUnauthorized<T>(string? reason) where T : notnull
        {
            return Result.Failure<T>(new UnauthorizedFailure(reason));
        }

        /// <summary>
        ///     Creates a failure <see cref="Result" /> instance representing a "Conflict" error
        ///     with a specified error message.
        /// </summary>
        /// <param name="message">
        ///     The error message describing the conflict. This message is typically used to convey
        ///     the specific reason for the conflict to the caller.
        /// </param>
        /// <returns>
        ///     A <see cref="Result" /> instance representing the failure, encapsulating a
        ///     <see cref="Failures.ConflictFailure" /> that contains the specified error message.
        /// </returns>
        public static Result FailConflict(string message)
        {
            return Result.Failure(new ConflictFailure(message));
        }

        /// <summary>
        ///     Creates a failure <see cref="Result" /> instance representing a "Conflict" error
        ///     with a specified error message.
        /// </summary>
        /// <param name="message">
        ///     A description of the conflict error. This message provides more details
        ///     about what caused the conflict.
        /// </param>
        /// <returns>
        ///     A <see cref="Result" /> instance representing the failure, encapsulating a
        ///     <see cref="ConflictFailure" /> that contains the error details.
        /// </returns>
        public static Result<T> FailConflict<T>(string message) where T : notnull
        {
            return Result.Failure<T>(new ConflictFailure(message));
        }
    }
}