using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.xunit.Tests;

public class ValidationErrorAssertionTests
{
    [Fact(DisplayName = "And executes custom assertion and returns self for chaining")]
    public void And_ExecutesCustomAssertion_ReturnsSelf()
    {
        // Arrange (Given)
        var error = new ValidationFailure(["Field is required"]);
        var assertion = new ValidationErrorAssertion(error);

        // Act (When)
        var result = assertion.And(e => Assert.NotNull(e));

        // Assert (Then)
        Assert.Equal(error, result.Failure);
    }

    [Fact(DisplayName = "WithFailure succeeds when failure exists")]
    public void WithFailure_WhenFailureExists_Succeeds()
    {
        // Arrange (Given)
        var error = new ValidationFailure(["Field is required", "Email is invalid"]);
        var assertion = new ValidationErrorAssertion(error);

        // Act (When)
        var result = assertion.WithFailure("Field is required");

        // Assert (Then)
        Assert.Equal(error, result.Failure);
    }

    [Fact(DisplayName = "WithFailure throws when failure does not exist")]
    public void WithFailure_WhenFailureDoesNotExist_Throws()
    {
        // Arrange (Given)
        var error = new ValidationFailure(["Field is required"]);
        var assertion = new ValidationErrorAssertion(error);

        // Act (When) & Assert (Then)
        Assert.Throws<Xunit.Sdk.ContainsException>(() => assertion.WithFailure("Email is invalid"));
    }

    [Fact(DisplayName = "WithFailureContaining succeeds when text is contained in a failure")]
    public void WithFailureContaining_WhenTextContained_Succeeds()
    {
        // Arrange (Given)
        var error = new ValidationFailure(["Field is required"]);
        var assertion = new ValidationErrorAssertion(error);

        // Act (When)
        var result = assertion.WithFailureContaining("required");

        // Assert (Then)
        Assert.Equal(error, result.Failure);
    }

    [Fact(DisplayName = "WithFailureContaining throws when text is not contained in any failure")]
    public void WithFailureContaining_WhenTextNotContained_Throws()
    {
        // Arrange (Given)
        var error = new ValidationFailure(["Field is required"]);
        var assertion = new ValidationErrorAssertion(error);

        // Act (When) & Assert (Then)
        Assert.Throws<Xunit.Sdk.ContainsException>(() => assertion.WithFailureContaining("invalid"));
    }

    [Fact(DisplayName = "WithFailureCount succeeds when count matches")]
    public void WithFailureCount_WhenCountMatches_Succeeds()
    {
        // Arrange (Given)
        var error = new ValidationFailure(["Field is required", "Email is invalid"]);
        var assertion = new ValidationErrorAssertion(error);

        // Act (When)
        var result = assertion.WithFailureCount(2);

        // Assert (Then)
        Assert.Equal(error, result.Failure);
    }

    [Fact(DisplayName = "WithFailureCount throws when count does not match")]
    public void WithFailureCount_WhenCountDoesNotMatch_Throws()
    {
        // Arrange (Given)
        var error = new ValidationFailure(["Field is required"]);
        var assertion = new ValidationErrorAssertion(error);

        // Act (When) & Assert (Then)
        Assert.Throws<Xunit.Sdk.EqualException>(() => assertion.WithFailureCount(2));
    }

    [Fact(DisplayName = "AndMessage succeeds when message matches")]
    public void AndMessage_WhenMessageMatches_Succeeds()
    {
        // Arrange (Given)
        var error = new ValidationFailure(["Field is required"]);
        var assertion = new ValidationErrorAssertion(error);

        // Act (When)
        var result = assertion.AndMessage("Field is required");

        // Assert (Then)
        Assert.Equal(error, result.Failure);
    }

    [Fact(DisplayName = "AndMessage throws when message does not match")]
    public void AndMessage_WhenMessageDoesNotMatch_Throws()
    {
        // Arrange (Given)
        var error = new ValidationFailure(["Field is required"]);
        var assertion = new ValidationErrorAssertion(error);

        // Act (When) & Assert (Then)
        Assert.Throws<Xunit.Sdk.EqualException>(() => assertion.AndMessage("Wrong message"));
    }

    [Fact(DisplayName = "AndCode succeeds when code matches")]
    public void AndCode_WhenCodeMatches_Succeeds()
    {
        // Arrange (Given)
        var error = new ValidationFailure(["Field is required"]);
        var assertion = new ValidationErrorAssertion(error);

        // Act (When)
        var result = assertion.AndCode("VALIDATION");

        // Assert (Then)
        Assert.Equal(error, result.Failure);
    }

    [Fact(DisplayName = "AndCode throws when code does not match")]
    public void AndCode_WhenCodeDoesNotMatch_Throws()
    {
        // Arrange (Given)
        var error = new ValidationFailure(["Field is required"]);
        var assertion = new ValidationErrorAssertion(error);

        // Act (When) & Assert (Then)
        Assert.Throws<Xunit.Sdk.EqualException>(() => assertion.AndCode("WRONG_CODE"));
    }

    [Fact(DisplayName = "Fluent chaining works with multiple assertions")]
    public void FluentChaining_WithMultipleAssertions_Works()
    {
        // Arrange (Given)
        var error = new ValidationFailure(["Field is required", "Email is invalid"]);
        var assertion = new ValidationErrorAssertion(error);

        // Act (When) & Assert (Then)
        assertion
            .WithFailureCount(2)
            .WithFailure("Field is required")
            .WithFailureContaining("Email")
            .AndCode("VALIDATION")
            .AndMessage("Field is required; Email is invalid");
    }
}
