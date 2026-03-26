using UnambitiousFx.Functional.Failures;
using Xunit;

namespace UnambitiousFx.Functional.xunit;

/// <summary>
///     Provides extension methods for asserting success or failure of result objects
///     within the FluentAssertions framework.
/// </summary>
public static class FluentAssertionExtensions
{
    /// <summary>
    ///     Asserts that the failure is a ValidationError and returns a ValidationFailureAssertion for further validation checks.
    /// </summary>
    /// <param name="assertion">The failure assertion to validate.</param>
    /// <returns>A <see cref="ValidationFailureAssertion" /> for the ValidationError.</returns>
    public static ValidationFailureAssertion WhichIsValidationError(this FailureAssertion assertion)
    {
        if (assertion.Failure is ValidationFailure validationError) {
            return new ValidationFailureAssertion(validationError);
        }

        Assert.Fail($"Expected ValidationError but was {assertion.Failure.GetType().Name}.");
        throw new InvalidOperationException("Unreachable");
    }

    /// <summary>
    ///     Asserts that the failure is a NotFoundError and returns a NotFoundFailureAssertion for further validation checks.
    /// </summary>
    /// <param name="assertion">The failure assertion to validate.</param>
    /// <returns>A <see cref="NotFoundFailureAssertion" /> for the NotFoundError.</returns>
    public static NotFoundFailureAssertion WhichIsNotFoundError(this FailureAssertion assertion)
    {
        if (assertion.Failure is NotFoundFailure notFoundError) {
            return new NotFoundFailureAssertion(notFoundError);
        }

        Assert.Fail($"Expected NotFoundError but was {assertion.Failure.GetType().Name}.");
        throw new InvalidOperationException("Unreachable");
    }

    /// <summary>
    ///     Asserts that the failure is a ConflictError and returns a ConflictFailureAssertion for further validation checks.
    /// </summary>
    /// <param name="assertion">The failure assertion to validate.</param>
    /// <returns>A <see cref="ConflictFailureAssertion" /> for the ConflictError.</returns>
    public static ConflictFailureAssertion WhichIsConflictError(this FailureAssertion assertion)
    {
        if (assertion.Failure is ConflictFailure conflictError) {
            return new ConflictFailureAssertion(conflictError);
        }

        Assert.Fail($"Expected ConflictError but was {assertion.Failure.GetType().Name}.");
        throw new InvalidOperationException("Unreachable");
    }

    /// <summary>
    ///     Asserts that the failure is of a specific error type and returns a strongly-typed assertion for further validation.
    /// </summary>
    /// <typeparam name="TError">The expected error type that implements IError.</typeparam>
    /// <param name="assertion">The failure assertion to validate.</param>
    /// <returns>A <see cref="TypedFailureAssertion{TError}" /> for the strongly-typed error.</returns>
    public static TypedFailureAssertion<TError> WhichIs<TError>(this FailureAssertion assertion)
        where TError : IFailure
    {
        if (assertion.Failure is TError typedError) {
            return new TypedFailureAssertion<TError>(typedError);
        }

        Assert.Fail($"Expected {typeof(TError).Name} but was {assertion.Failure.GetType().Name}.");
        throw new InvalidOperationException("Unreachable");
    }

    /// <summary>
    ///     Asserts that the failure is a TimeoutError and returns a strongly-typed assertion for further validation.
    /// </summary>
    /// <param name="assertion">The failure assertion to validate.</param>
    /// <returns>A <see cref="TypedFailureAssertion{TimeoutFailure}" /> for the TimeoutError.</returns>
    public static TypedFailureAssertion<TimeoutFailure> WhichIsTimeoutError(this FailureAssertion assertion)
    {
        if (assertion.Failure is TimeoutFailure timeoutError) {
            return new TypedFailureAssertion<TimeoutFailure>(timeoutError);
        }

        Assert.Fail($"Expected TimeoutError but was {assertion.Failure.GetType().Name}.");
        throw new InvalidOperationException("Unreachable");
    }

    /// <summary>
    ///     Asserts that the failure is an UnauthenticatedError and returns a strongly-typed assertion for further validation.
    /// </summary>
    /// <param name="assertion">The failure assertion to validate.</param>
    /// <returns>A <see cref="TypedFailureAssertion{UnauthenticatedFailure}" /> for the UnauthenticatedError.</returns>
    public static TypedFailureAssertion<UnauthenticatedFailure> WhichIsUnauthenticatedError(this FailureAssertion assertion)
    {
        if (assertion.Failure is UnauthenticatedFailure unauthenticatedError) {
            return new TypedFailureAssertion<UnauthenticatedFailure>(unauthenticatedError);
        }

        Assert.Fail($"Expected UnauthenticatedError but was {assertion.Failure.GetType().Name}.");
        throw new InvalidOperationException("Unreachable");
    }

    /// <summary>
    ///     Asserts that the failure is an UnauthorizedError and returns a strongly-typed assertion for further validation.
    /// </summary>
    /// <param name="assertion">The failure assertion to validate.</param>
    /// <returns>A <see cref="TypedFailureAssertion{UnauthorizedFailure}" /> for the UnauthorizedError.</returns>
    public static TypedFailureAssertion<UnauthorizedFailure> WhichIsUnauthorizedError(this FailureAssertion assertion)
    {
        if (assertion.Failure is UnauthorizedFailure unauthorizedError) {
            return new TypedFailureAssertion<UnauthorizedFailure>(unauthorizedError);
        }

        Assert.Fail($"Expected UnauthorizedError but was {assertion.Failure.GetType().Name}.");
        throw new InvalidOperationException("Unreachable");
    }
}
