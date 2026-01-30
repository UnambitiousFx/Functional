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
    ///     Asserts that the failure is a ValidationError and returns a ValidationErrorAssertion for further validation checks.
    /// </summary>
    /// <param name="assertion">The failure assertion to validate.</param>
    /// <returns>A <see cref="ValidationErrorAssertion" /> for the ValidationError.</returns>
    public static ValidationErrorAssertion WhichIsValidationError(this FailureAssertion assertion)
    {
        if (assertion.Failure is ValidationFailure validationError)
        {
            return new ValidationErrorAssertion(validationError);
        }

        Assert.Fail($"Expected ValidationError but was {assertion.Failure.GetType().Name}.");
        throw new InvalidOperationException("Unreachable");
    }

    /// <summary>
    ///     Asserts that the failure is a NotFoundError and returns a NotFoundErrorAssertion for further validation checks.
    /// </summary>
    /// <param name="assertion">The failure assertion to validate.</param>
    /// <returns>A <see cref="NotFoundErrorAssertion" /> for the NotFoundError.</returns>
    public static NotFoundErrorAssertion WhichIsNotFoundError(this FailureAssertion assertion)
    {
        if (assertion.Failure is NotFoundFailure notFoundError)
        {
            return new NotFoundErrorAssertion(notFoundError);
        }

        Assert.Fail($"Expected NotFoundError but was {assertion.Failure.GetType().Name}.");
        throw new InvalidOperationException("Unreachable");
    }

    /// <summary>
    ///     Asserts that the failure is a ConflictError and returns a ConflictErrorAssertion for further validation checks.
    /// </summary>
    /// <param name="assertion">The failure assertion to validate.</param>
    /// <returns>A <see cref="ConflictErrorAssertion" /> for the ConflictError.</returns>
    public static ConflictErrorAssertion WhichIsConflictError(this FailureAssertion assertion)
    {
        if (assertion.Failure is ConflictFailure conflictError)
        {
            return new ConflictErrorAssertion(conflictError);
        }

        Assert.Fail($"Expected ConflictError but was {assertion.Failure.GetType().Name}.");
        throw new InvalidOperationException("Unreachable");
    }

    /// <summary>
    ///     Asserts that the failure is of a specific error type and returns a strongly-typed assertion for further validation.
    /// </summary>
    /// <typeparam name="TError">The expected error type that implements IError.</typeparam>
    /// <param name="assertion">The failure assertion to validate.</param>
    /// <returns>A <see cref="TypedErrorAssertion{TError}" /> for the strongly-typed error.</returns>
    public static TypedErrorAssertion<TError> WhichIs<TError>(this FailureAssertion assertion)
        where TError : IFailure
    {
        if (assertion.Failure is TError typedError)
        {
            return new TypedErrorAssertion<TError>(typedError);
        }

        Assert.Fail($"Expected {typeof(TError).Name} but was {assertion.Failure.GetType().Name}.");
        throw new InvalidOperationException("Unreachable");
    }
}
