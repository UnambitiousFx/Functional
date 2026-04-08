using UnambitiousFx.Functional.Failures;
using Xunit.Sdk;

namespace UnambitiousFx.Functional.xunit.Tests;

public class ValidationFailureAssertionTests
{
    [Fact]
    public void And_ExecutesCustomAssertion_ReturnsSelf()
    {
        // Arrange (Given)
        var error     = new ValidationFailure(["Field is required"]);
        var assertion = new ValidationFailureAssertion(error);

        // Act (When)
        var result = assertion.And(e => Assert.NotNull(e));

        // Assert (Then)
        Assert.Equal(error, result.Failure);
    }

    [Fact]
    public void WithFailure_WhenFailureExists_Succeeds()
    {
        // Arrange (Given)
        var error     = new ValidationFailure(["Field is required", "Email is invalid"]);
        var assertion = new ValidationFailureAssertion(error);

        // Act (When)
        var result = assertion.WithFailure("Field is required");

        // Assert (Then)
        Assert.Equal(error, result.Failure);
    }

    [Fact]
    public void WithFailure_WhenFailureDoesNotExist_Throws()
    {
        // Arrange (Given)
        var error     = new ValidationFailure(["Field is required"]);
        var assertion = new ValidationFailureAssertion(error);

        // Act (When) & Assert (Then)
        Assert.Throws<ContainsException>(() => assertion.WithFailure("Email is invalid"));
    }

    [Fact]
    public void WithFailureContaining_WhenTextContained_Succeeds()
    {
        // Arrange (Given)
        var error     = new ValidationFailure(["Field is required"]);
        var assertion = new ValidationFailureAssertion(error);

        // Act (When)
        var result = assertion.WithFailureContaining("required");

        // Assert (Then)
        Assert.Equal(error, result.Failure);
    }

    [Fact]
    public void WithFailureContaining_WhenTextNotContained_Throws()
    {
        // Arrange (Given)
        var error     = new ValidationFailure(["Field is required"]);
        var assertion = new ValidationFailureAssertion(error);

        // Act (When) & Assert (Then)
        Assert.Throws<ContainsException>(() => assertion.WithFailureContaining("invalid"));
    }

    [Fact]
    public void WithFailureCount_WhenCountMatches_Succeeds()
    {
        // Arrange (Given)
        var error     = new ValidationFailure(["Field is required", "Email is invalid"]);
        var assertion = new ValidationFailureAssertion(error);

        // Act (When)
        var result = assertion.WithFailureCount(2);

        // Assert (Then)
        Assert.Equal(error, result.Failure);
    }

    [Fact]
    public void WithFailureCount_WhenCountDoesNotMatch_Throws()
    {
        // Arrange (Given)
        var error     = new ValidationFailure(["Field is required"]);
        var assertion = new ValidationFailureAssertion(error);

        // Act (When) & Assert (Then)
        Assert.Throws<EqualException>(() => assertion.WithFailureCount(2));
    }

    [Fact]
    public void AndMessage_WhenMessageMatches_Succeeds()
    {
        // Arrange (Given)
        var error     = new ValidationFailure(["Field is required"]);
        var assertion = new ValidationFailureAssertion(error);

        // Act (When)
        var result = assertion.AndMessage("Field is required");

        // Assert (Then)
        Assert.Equal(error, result.Failure);
    }

    [Fact]
    public void AndMessage_WhenMessageDoesNotMatch_Throws()
    {
        // Arrange (Given)
        var error     = new ValidationFailure(["Field is required"]);
        var assertion = new ValidationFailureAssertion(error);

        // Act (When) & Assert (Then)
        Assert.Throws<EqualException>(() => assertion.AndMessage("Wrong message"));
    }

    [Fact]
    public void AndCode_WhenCodeMatches_Succeeds()
    {
        // Arrange (Given)
        var error     = new ValidationFailure(["Field is required"]);
        var assertion = new ValidationFailureAssertion(error);

        // Act (When)
        var result = assertion.AndCode("VALIDATION");

        // Assert (Then)
        Assert.Equal(error, result.Failure);
    }

    [Fact]
    public void AndCode_WhenCodeDoesNotMatch_Throws()
    {
        // Arrange (Given)
        var error     = new ValidationFailure(["Field is required"]);
        var assertion = new ValidationFailureAssertion(error);

        // Act (When) & Assert (Then)
        Assert.Throws<EqualException>(() => assertion.AndCode("WRONG_CODE"));
    }

    [Fact]
    public void FluentChaining_WithMultipleAssertions_Works()
    {
        // Arrange (Given)
        var error     = new ValidationFailure(["Field is required", "Email is invalid"]);
        var assertion = new ValidationFailureAssertion(error);

        // Act (When) & Assert (Then)
        assertion
           .WithFailureCount(2)
           .WithFailure("Field is required")
           .WithFailureContaining("Email")
           .AndCode("VALIDATION")
           .AndMessage("Field is required; Email is invalid");
    }
}
