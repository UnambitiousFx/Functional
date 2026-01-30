using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.xunit.Tests;

public class TypedErrorAssertionTests
{
    [Fact(DisplayName = "And executes custom assertion and returns self for chaining")]
    public void And_ExecutesCustomAssertion_ReturnsSelf()
    {
        // Arrange (Given)
        var error = new UnauthorizedFailure();
        var assertion = new TypedErrorAssertion<UnauthorizedFailure>(error);

        // Act (When)
        var result = assertion.And(e => Assert.NotNull(e));

        // Assert (Then)
        Assert.Equal(error, result.Error);
    }

    [Fact(DisplayName = "Where succeeds when predicate is satisfied")]
    public void Where_WhenPredicateSatisfied_Succeeds()
    {
        // Arrange (Given)
        var error = new UnauthorizedFailure();
        var assertion = new TypedErrorAssertion<UnauthorizedFailure>(error);

        // Act (When)
        var result = assertion.Where(e => e.Code == "UNAUTHORIZED");

        // Assert (Then)
        Assert.Equal(error, result.Error);
    }

    [Fact(DisplayName = "Where throws when predicate is not satisfied")]
    public void Where_WhenPredicateNotSatisfied_Throws()
    {
        // Arrange (Given)
        var error = new UnauthorizedFailure();
        var assertion = new TypedErrorAssertion<UnauthorizedFailure>(error);

        // Act (When) & Assert (Then)
        var exception = Assert.Throws<Xunit.Sdk.FailException>(() => assertion.Where(e => e.Code == "WRONG_CODE"));
        Assert.Contains("Error does not satisfy predicate", exception.Message);
    }

    [Fact(DisplayName = "Where includes custom because message in failure")]
    public void Where_WithBecauseMessage_IncludesInFailure()
    {
        // Arrange (Given)
        var error = new UnauthorizedFailure();
        var assertion = new TypedErrorAssertion<UnauthorizedFailure>(error);

        // Act (When) & Assert (Then)
        var exception = Assert.Throws<Xunit.Sdk.FailException>(() =>
            assertion.Where(e => e.Code == "WRONG_CODE", "Custom reason"));
        Assert.Contains("Custom reason", exception.Message);
    }

    [Fact(DisplayName = "Where throws ArgumentNullException when predicate is null")]
    public void Where_WhenPredicateNull_ThrowsArgumentNullException()
    {
        // Arrange (Given)
        var error = new UnauthorizedFailure();
        var assertion = new TypedErrorAssertion<UnauthorizedFailure>(error);

        // Act (When) & Assert (Then)
        Assert.Throws<ArgumentNullException>(() => assertion.Where(null!));
    }

    [Fact(DisplayName = "AndMessage succeeds when message matches")]
    public void AndMessage_WhenMessageMatches_Succeeds()
    {
        // Arrange (Given)
        var error = new UnauthorizedFailure();
        var assertion = new TypedErrorAssertion<UnauthorizedFailure>(error);

        // Act (When)
        var result = assertion.AndMessage("Unauthorized");

        // Assert (Then)
        Assert.Equal(error, result.Error);
    }

    [Fact(DisplayName = "AndMessage throws when message does not match")]
    public void AndMessage_WhenMessageDoesNotMatch_Throws()
    {
        // Arrange (Given)
        var error = new UnauthorizedFailure();
        var assertion = new TypedErrorAssertion<UnauthorizedFailure>(error);

        // Act (When) & Assert (Then)
        Assert.Throws<Xunit.Sdk.EqualException>(() => assertion.AndMessage("Wrong message"));
    }

    [Fact(DisplayName = "AndCode succeeds when code matches")]
    public void AndCode_WhenCodeMatches_Succeeds()
    {
        // Arrange (Given)
        var error = new UnauthorizedFailure();
        var assertion = new TypedErrorAssertion<UnauthorizedFailure>(error);

        // Act (When)
        var result = assertion.AndCode("UNAUTHORIZED");

        // Assert (Then)
        Assert.Equal(error, result.Error);
    }

    [Fact(DisplayName = "AndCode throws when code does not match")]
    public void AndCode_WhenCodeDoesNotMatch_Throws()
    {
        // Arrange (Given)
        var error = new UnauthorizedFailure();
        var assertion = new TypedErrorAssertion<UnauthorizedFailure>(error);

        // Act (When) & Assert (Then)
        Assert.Throws<Xunit.Sdk.EqualException>(() => assertion.AndCode("WRONG_CODE"));
    }

    [Fact(DisplayName = "Fluent chaining works with multiple assertions")]
    public void FluentChaining_WithMultipleAssertions_Works()
    {
        // Arrange (Given)
        var error = new UnauthorizedFailure();
        var assertion = new TypedErrorAssertion<UnauthorizedFailure>(error);

        // Act (When) & Assert (Then)
        assertion
            .Where(e => e.Code == "UNAUTHORIZED")
            .AndCode("UNAUTHORIZED")
            .AndMessage("Unauthorized");
    }

    [Fact(DisplayName = "Works with custom error types")]
    public void WorksWithCustomErrorTypes_Correctly()
    {
        // Arrange (Given)
        var customError = new Failure("CUSTOM", "Custom error message");
        var assertion = new TypedErrorAssertion<Failure>(customError);

        // Act (When) & Assert (Then)
        assertion
            .AndCode("CUSTOM")
            .AndMessage("Custom error message")
            .Where(e => e.Code.StartsWith("CUST"));
    }

    [Fact(DisplayName = "And allows complex custom assertions")]
    public void And_AllowsComplexCustomAssertions()
    {
        // Arrange (Given)
        var error = new ValidationFailure(["Field1 is required", "Field2 is invalid"]);
        var assertion = new TypedErrorAssertion<ValidationFailure>(error);

        // Act (When) & Assert (Then)
        assertion.And(e =>
        {
            Assert.Equal(2, e.Failures.Count);
            Assert.Contains("required", e.Failures[0]);
            Assert.Contains("invalid", e.Failures[1]);
        });
    }
}
