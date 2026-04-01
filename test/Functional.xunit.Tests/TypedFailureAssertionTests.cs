using UnambitiousFx.Functional.Failures;
using Xunit.Sdk;

namespace UnambitiousFx.Functional.xunit.Tests;

public class TypedFailureAssertionTests
{
    [Fact]
    public void And_ExecutesCustomAssertion_ReturnsSelf()
    {
        // Arrange (Given)
        var error     = new UnauthorizedFailure();
        var assertion = new TypedFailureAssertion<UnauthorizedFailure>(error);

        // Act (When)
        var result = assertion.And(e => Assert.NotNull(e));

        // Assert (Then)
        Assert.Equal(error, result.Failure);
    }

    [Fact]
    public void Where_WhenPredicateSatisfied_Succeeds()
    {
        // Arrange (Given)
        var error     = new UnauthorizedFailure();
        var assertion = new TypedFailureAssertion<UnauthorizedFailure>(error);

        // Act (When)
        var result = assertion.Where(e => e.Code == "UNAUTHORIZED");

        // Assert (Then)
        Assert.Equal(error, result.Failure);
    }

    [Fact]
    public void Where_WhenPredicateNotSatisfied_Throws()
    {
        // Arrange (Given)
        var error     = new UnauthorizedFailure();
        var assertion = new TypedFailureAssertion<UnauthorizedFailure>(error);

        // Act (When) & Assert (Then)
        var exception = Assert.Throws<FailException>(() => assertion.Where(e => e.Code == "WRONG_CODE"));
        Assert.Contains("Failure does not satisfy predicate", exception.Message);
    }

    [Fact]
    public void Where_WithBecauseMessage_IncludesInFailure()
    {
        // Arrange (Given)
        var error     = new UnauthorizedFailure();
        var assertion = new TypedFailureAssertion<UnauthorizedFailure>(error);

        // Act (When) & Assert (Then)
        var exception = Assert.Throws<FailException>(() =>
                                                         assertion.Where(e => e.Code == "WRONG_CODE", "Custom reason"));
        Assert.Contains("Custom reason", exception.Message);
    }

    [Fact]
    public void Where_WhenPredicateNull_ThrowsArgumentNullException()
    {
        // Arrange (Given)
        var error     = new UnauthorizedFailure();
        var assertion = new TypedFailureAssertion<UnauthorizedFailure>(error);

        // Act (When) & Assert (Then)
        Assert.Throws<ArgumentNullException>(() => assertion.Where(null!));
    }

    [Fact]
    public void AndMessage_WhenMessageMatches_Succeeds()
    {
        // Arrange (Given)
        var error     = new UnauthorizedFailure();
        var assertion = new TypedFailureAssertion<UnauthorizedFailure>(error);

        // Act (When)
        var result = assertion.AndMessage("Unauthorized");

        // Assert (Then)
        Assert.Equal(error, result.Failure);
    }

    [Fact]
    public void AndMessage_WhenMessageDoesNotMatch_Throws()
    {
        // Arrange (Given)
        var error     = new UnauthorizedFailure();
        var assertion = new TypedFailureAssertion<UnauthorizedFailure>(error);

        // Act (When) & Assert (Then)
        Assert.Throws<EqualException>(() => assertion.AndMessage("Wrong message"));
    }

    [Fact]
    public void AndCode_WhenCodeMatches_Succeeds()
    {
        // Arrange (Given)
        var error     = new UnauthorizedFailure();
        var assertion = new TypedFailureAssertion<UnauthorizedFailure>(error);

        // Act (When)
        var result = assertion.AndCode("UNAUTHORIZED");

        // Assert (Then)
        Assert.Equal(error, result.Failure);
    }

    [Fact]
    public void AndCode_WhenCodeDoesNotMatch_Throws()
    {
        // Arrange (Given)
        var error     = new UnauthorizedFailure();
        var assertion = new TypedFailureAssertion<UnauthorizedFailure>(error);

        // Act (When) & Assert (Then)
        Assert.Throws<EqualException>(() => assertion.AndCode("WRONG_CODE"));
    }

    [Fact]
    public void FluentChaining_WithMultipleAssertions_Works()
    {
        // Arrange (Given)
        var error     = new UnauthorizedFailure();
        var assertion = new TypedFailureAssertion<UnauthorizedFailure>(error);

        // Act (When) & Assert (Then)
        assertion
           .Where(e => e.Code == "UNAUTHORIZED")
           .AndCode("UNAUTHORIZED")
           .AndMessage("Unauthorized");
    }

    [Fact]
    public void WorksWithCustomErrorTypes_Correctly()
    {
        // Arrange (Given)
        var customError = new Failure("CUSTOM", "Custom error message");
        var assertion   = new TypedFailureAssertion<Failure>(customError);

        // Act (When) & Assert (Then)
        assertion
           .AndCode("CUSTOM")
           .AndMessage("Custom error message")
           .Where(e => e.Code.StartsWith("CUST"));
    }

    [Fact]
    public void And_AllowsComplexCustomAssertions()
    {
        // Arrange (Given)
        var error     = new ValidationFailure(["Field1 is required", "Field2 is invalid"]);
        var assertion = new TypedFailureAssertion<ValidationFailure>(error);

        // Act (When) & Assert (Then)
        assertion.And(e =>
        {
            Assert.Equal(2, e.Failures.Count);
            Assert.Contains("required", e.Failures[0]);
            Assert.Contains("invalid",  e.Failures[1]);
        });
    }
}
