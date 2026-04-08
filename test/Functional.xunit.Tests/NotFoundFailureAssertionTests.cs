using UnambitiousFx.Functional.Failures;
using Xunit.Sdk;

namespace UnambitiousFx.Functional.xunit.Tests;

public class NotFoundFailureAssertionTests
{
    [Fact]
    public void And_ExecutesCustomAssertion_ReturnsSelf()
    {
        // Arrange (Given)
        var error     = new NotFoundFailure("User", "123");
        var assertion = new NotFoundFailureAssertion(error);

        // Act (When)
        var result = assertion.And(e => Assert.NotNull(e));

        // Assert (Then)
        Assert.Equal(error, result.Failure);
    }

    [Fact]
    public void WithResource_WhenResourceMatches_Succeeds()
    {
        // Arrange (Given)
        var error     = new NotFoundFailure("User", "123");
        var assertion = new NotFoundFailureAssertion(error);

        // Act (When)
        var result = assertion.WithResource("User");

        // Assert (Then)
        Assert.Equal(error, result.Failure);
    }

    [Fact]
    public void WithResource_WhenResourceDoesNotMatch_Throws()
    {
        // Arrange (Given)
        var error     = new NotFoundFailure("User", "123");
        var assertion = new NotFoundFailureAssertion(error);

        // Act (When) & Assert (Then)
        Assert.Throws<EqualException>(() => assertion.WithResource("Product"));
    }

    [Fact]
    public void WithIdentifier_WhenIdentifierMatches_Succeeds()
    {
        // Arrange (Given)
        var error     = new NotFoundFailure("User", "123");
        var assertion = new NotFoundFailureAssertion(error);

        // Act (When)
        var result = assertion.WithIdentifier("123");

        // Assert (Then)
        Assert.Equal(error, result.Failure);
    }

    [Fact]
    public void WithIdentifier_WhenIdentifierDoesNotMatch_Throws()
    {
        // Arrange (Given)
        var error     = new NotFoundFailure("User", "123");
        var assertion = new NotFoundFailureAssertion(error);

        // Act (When) & Assert (Then)
        Assert.Throws<EqualException>(() => assertion.WithIdentifier("456"));
    }

    [Fact]
    public void AndMessage_WhenMessageMatches_Succeeds()
    {
        // Arrange (Given)
        var error     = new NotFoundFailure("User", "123");
        var assertion = new NotFoundFailureAssertion(error);

        // Act (When)
        var result = assertion.AndMessage("Resource 'User' with id '123' was not found.");

        // Assert (Then)
        Assert.Equal(error, result.Failure);
    }

    [Fact]
    public void AndMessage_WhenMessageDoesNotMatch_Throws()
    {
        // Arrange (Given)
        var error     = new NotFoundFailure("User", "123");
        var assertion = new NotFoundFailureAssertion(error);

        // Act (When) & Assert (Then)
        Assert.Throws<EqualException>(() => assertion.AndMessage("Wrong message"));
    }

    [Fact]
    public void AndCode_WhenCodeMatches_Succeeds()
    {
        // Arrange (Given)
        var error     = new NotFoundFailure("User", "123");
        var assertion = new NotFoundFailureAssertion(error);

        // Act (When)
        var result = assertion.AndCode("NOT_FOUND");

        // Assert (Then)
        Assert.Equal(error, result.Failure);
    }

    [Fact]
    public void AndCode_WhenCodeDoesNotMatch_Throws()
    {
        // Arrange (Given)
        var error     = new NotFoundFailure("User", "123");
        var assertion = new NotFoundFailureAssertion(error);

        // Act (When) & Assert (Then)
        Assert.Throws<EqualException>(() => assertion.AndCode("WRONG_CODE"));
    }

    [Fact]
    public void FluentChaining_WithMultipleAssertions_Works()
    {
        // Arrange (Given)
        var error     = new NotFoundFailure("User", "123");
        var assertion = new NotFoundFailureAssertion(error);

        // Act (When) & Assert (Then)
        assertion
           .WithResource("User")
           .WithIdentifier("123")
           .AndCode("NOT_FOUND")
           .AndMessage("Resource 'User' with id '123' was not found.");
    }

    [Fact]
    public void And_AllowsCustomAssertions_OnErrorProperties()
    {
        // Arrange (Given)
        var error     = new NotFoundFailure("User", "123");
        var assertion = new NotFoundFailureAssertion(error);

        // Act (When) & Assert (Then)
        assertion.And(e =>
        {
            Assert.Equal("User", e.Resource);
            Assert.Equal("123",  e.Identifier);
            Assert.Contains("123", e.Message);
        });
    }
}
