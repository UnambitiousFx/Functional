using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.Tests.Failures;

/// <summary>
///     Tests for FailureExtensions.
/// </summary>
public class FailureExtensionsTests
{
    [Fact]
    public void ToException_WithExceptionalFailure_ReturnsWrappedException()
    {
        // Arrange (Given)
        var innerException = new InvalidOperationException("Inner exception");
        var error = new ExceptionalFailure(innerException);

        // Act (When)
        var exception = error.ToException();

        // Assert (Then)
        Assert.Same(innerException, exception);
    }

    [Fact]
    public void ToException_WithAggregateFailure_ReturnsAggregateException()
    {
        // Arrange (Given)
        var error1 = new Failure("Error1", "First error");
        var error2 = new Failure("Error2", "Second error");
        var error3 = new ExceptionalFailure(new InvalidOperationException("Third error"));
        var aggregateError = new AggregateFailure([error1, error2, error3]);

        // Act (When)
        var exception = aggregateError.ToException();

        // Assert (Then)
        Assert.IsType<AggregateException>(exception);
        var aggregateException = (AggregateException)exception;
        Assert.Equal(3, aggregateException.InnerExceptions.Count);
        Assert.IsType<FunctionalException>(aggregateException.InnerExceptions[0]);
        Assert.IsType<FunctionalException>(aggregateException.InnerExceptions[1]);
        Assert.IsType<InvalidOperationException>(aggregateException.InnerExceptions[2]);
    }

    [Fact]
    public void ToException_WithValidationFailure_ReturnsFunctionalException()
    {
        // Arrange (Given)
        var validationError = new ValidationFailure(["Field1 is required"]);

        // Act (When)
        var exception = validationError.ToException();

        // Assert (Then)
        Assert.IsType<FunctionalException>(exception);
    }

    [Fact]
    public void ToException_WithNotFoundFailure_ReturnsFunctionalException()
    {
        // Arrange (Given)
        var notFoundError = new NotFoundFailure("Resource", "123");

        // Act (When)
        var exception = notFoundError.ToException();

        // Assert (Then)
        Assert.IsType<FunctionalException>(exception);
    }

    [Fact]
    public void ToException_WithConflictFailure_ReturnsFunctionalException()
    {
        // Arrange (Given)
        var conflictError = new ConflictFailure("Resource already exists");

        // Act (When)
        var exception = conflictError.ToException();

        // Assert (Then)
        Assert.IsType<FunctionalException>(exception);
    }

    [Fact]
    public void ToException_WithUnauthenticatedFailure_ReturnsFunctionalException()
    {
        // Arrange (Given)
        var unauthenticatedError = new UnauthenticatedFailure("Token expired");

        // Act (When)
        var exception = unauthenticatedError.ToException();

        // Assert (Then)
        Assert.IsType<FunctionalException>(exception);
    }

    [Fact]
    public void ToException_WithUnauthorizedFailure_ReturnsFunctionalException()
    {
        // Arrange (Given)
        var unauthorizedError = new UnauthorizedFailure("Insufficient permissions");

        // Act (When)
        var exception = unauthorizedError.ToException();

        // Assert (Then)
        Assert.IsType<FunctionalException>(exception);
    }

    [Fact]
    public void ToException_WithTimeoutFailure_ReturnsFunctionalException()
    {
        // Arrange (Given)
        var timeoutError = new TimeoutFailure(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(45));

        // Act (When)
        var exception = timeoutError.ToException();

        // Assert (Then)
        Assert.IsType<FunctionalException>(exception);
    }

    [Fact]
    public void ToException_WithCustomFailure_ReturnsFunctionalException()
    {
        // Arrange (Given)
        var customError = new Failure("CUSTOM_ERROR", "Custom error message");

        // Act (When)
        var exception = customError.ToException();

        // Assert (Then)
        Assert.IsType<FunctionalException>(exception);
    }

    [Fact]
    public void ToException_WithNestedAggregateFailure_ConvertsAllErrors()
    {
        // Arrange (Given)
        var error1 = new Failure("Error1", "First error");
        var error2 = new Failure("Error2", "Second error");
        var innerAggregate = new AggregateFailure([error1, error2]);
        var error3 = new Failure("Error3", "Third error");
        var outerAggregate = new AggregateFailure([innerAggregate, error3]);

        // Act (When)
        var exception = outerAggregate.ToException();

        // Assert (Then)
        Assert.IsType<AggregateException>(exception);
        var aggregateException = (AggregateException)exception;
        Assert.Equal(2, aggregateException.InnerExceptions.Count);
        Assert.IsType<AggregateException>(aggregateException.InnerExceptions[0]);
        Assert.IsType<FunctionalException>(aggregateException.InnerExceptions[1]);
    }
}
