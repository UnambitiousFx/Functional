using UnambitiousFx.Functional.Errors;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.xunit.Tests;

public class NotFoundErrorAssertionTests
{
    [Fact(DisplayName = "And executes custom assertion and returns self for chaining")]
    public void And_ExecutesCustomAssertion_ReturnsSelf()
    {
        // Arrange (Given)
        var error = new NotFoundError("User", "123");
        var assertion = new NotFoundErrorAssertion(error);

        // Act (When)
        var result = assertion.And(e => Assert.NotNull(e));

        // Assert (Then)
        Assert.Equal(error, result.Error);
    }

    [Fact(DisplayName = "WithResource succeeds when resource matches")]
    public void WithResource_WhenResourceMatches_Succeeds()
    {
        // Arrange (Given)
        var error = new NotFoundError("User", "123");
        var assertion = new NotFoundErrorAssertion(error);

        // Act (When)
        var result = assertion.WithResource("User");

        // Assert (Then)
        Assert.Equal(error, result.Error);
    }

    [Fact(DisplayName = "WithResource throws when resource does not match")]
    public void WithResource_WhenResourceDoesNotMatch_Throws()
    {
        // Arrange (Given)
        var error = new NotFoundError("User", "123");
        var assertion = new NotFoundErrorAssertion(error);

        // Act (When) & Assert (Then)
        Assert.Throws<Xunit.Sdk.EqualException>(() => assertion.WithResource("Product"));
    }

    [Fact(DisplayName = "WithIdentifier succeeds when identifier matches")]
    public void WithIdentifier_WhenIdentifierMatches_Succeeds()
    {
        // Arrange (Given)
        var error = new NotFoundError("User", "123");
        var assertion = new NotFoundErrorAssertion(error);

        // Act (When)
        var result = assertion.WithIdentifier("123");

        // Assert (Then)
        Assert.Equal(error, result.Error);
    }

    [Fact(DisplayName = "WithIdentifier throws when identifier does not match")]
    public void WithIdentifier_WhenIdentifierDoesNotMatch_Throws()
    {
        // Arrange (Given)
        var error = new NotFoundError("User", "123");
        var assertion = new NotFoundErrorAssertion(error);

        // Act (When) & Assert (Then)
        Assert.Throws<Xunit.Sdk.EqualException>(() => assertion.WithIdentifier("456"));
    }

    [Fact(DisplayName = "AndMessage succeeds when message matches")]
    public void AndMessage_WhenMessageMatches_Succeeds()
    {
        // Arrange (Given)
        var error = new NotFoundError("User", "123");
        var assertion = new NotFoundErrorAssertion(error);

        // Act (When)
        var result = assertion.AndMessage("Resource 'User' with id '123' was not found.");

        // Assert (Then)
        Assert.Equal(error, result.Error);
    }

    [Fact(DisplayName = "AndMessage throws when message does not match")]
    public void AndMessage_WhenMessageDoesNotMatch_Throws()
    {
        // Arrange (Given)
        var error = new NotFoundError("User", "123");
        var assertion = new NotFoundErrorAssertion(error);

        // Act (When) & Assert (Then)
        Assert.Throws<Xunit.Sdk.EqualException>(() => assertion.AndMessage("Wrong message"));
    }

    [Fact(DisplayName = "AndCode succeeds when code matches")]
    public void AndCode_WhenCodeMatches_Succeeds()
    {
        // Arrange (Given)
        var error = new NotFoundError("User", "123");
        var assertion = new NotFoundErrorAssertion(error);

        // Act (When)
        var result = assertion.AndCode("NOT_FOUND");

        // Assert (Then)
        Assert.Equal(error, result.Error);
    }

    [Fact(DisplayName = "AndCode throws when code does not match")]
    public void AndCode_WhenCodeDoesNotMatch_Throws()
    {
        // Arrange (Given)
        var error = new NotFoundError("User", "123");
        var assertion = new NotFoundErrorAssertion(error);

        // Act (When) & Assert (Then)
        Assert.Throws<Xunit.Sdk.EqualException>(() => assertion.AndCode("WRONG_CODE"));
    }

    [Fact(DisplayName = "Fluent chaining works with multiple assertions")]
    public void FluentChaining_WithMultipleAssertions_Works()
    {
        // Arrange (Given)
        var error = new NotFoundError("User", "123");
        var assertion = new NotFoundErrorAssertion(error);

        // Act (When) & Assert (Then)
        assertion
            .WithResource("User")
            .WithIdentifier("123")
            .AndCode("NOT_FOUND")
            .AndMessage("Resource 'User' with id '123' was not found.");
    }

    [Fact(DisplayName = "And allows custom assertions on error properties")]
    public void And_AllowsCustomAssertions_OnErrorProperties()
    {
        // Arrange (Given)
        var error = new NotFoundError("User", "123");
        var assertion = new NotFoundErrorAssertion(error);

        // Act (When) & Assert (Then)
        assertion.And(e =>
        {
            Assert.Equal("User", e.Resource);
            Assert.Equal("123", e.Identifier);
            Assert.Contains("123", e.Message);
        });
    }
}
