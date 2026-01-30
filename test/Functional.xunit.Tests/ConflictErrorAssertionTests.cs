using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.xunit.Tests;

public class ConflictErrorAssertionTests
{
    [Fact(DisplayName = "And executes custom assertion and returns self for chaining")]
    public void And_ExecutesCustomAssertion_ReturnsSelf()
    {
        // Arrange (Given)
        var error = new ConflictFailure("Resource already exists");
        var assertion = new ConflictErrorAssertion(error);

        // Act (When)
        var result = assertion.And(e => Assert.NotNull(e));

        // Assert (Then)
        Assert.Equal(error, result.Failure);
    }

    [Fact(DisplayName = "AndMessage succeeds when message matches")]
    public void AndMessage_WhenMessageMatches_Succeeds()
    {
        // Arrange (Given)
        var error = new ConflictFailure("Resource already exists");
        var assertion = new ConflictErrorAssertion(error);

        // Act (When)
        var result = assertion.AndMessage("Resource already exists");

        // Assert (Then)
        Assert.Equal(error, result.Failure);
    }

    [Fact(DisplayName = "AndMessage throws when message does not match")]
    public void AndMessage_WhenMessageDoesNotMatch_Throws()
    {
        // Arrange (Given)
        var error = new ConflictFailure("Resource already exists");
        var assertion = new ConflictErrorAssertion(error);

        // Act (When) & Assert (Then)
        Assert.Throws<Xunit.Sdk.EqualException>(() => assertion.AndMessage("Wrong message"));
    }

    [Fact(DisplayName = "AndCode succeeds when code matches")]
    public void AndCode_WhenCodeMatches_Succeeds()
    {
        // Arrange (Given)
        var error = new ConflictFailure("Resource already exists");
        var assertion = new ConflictErrorAssertion(error);

        // Act (When)
        var result = assertion.AndCode("CONFLICT");

        // Assert (Then)
        Assert.Equal(error, result.Failure);
    }

    [Fact(DisplayName = "AndCode throws when code does not match")]
    public void AndCode_WhenCodeDoesNotMatch_Throws()
    {
        // Arrange (Given)
        var error = new ConflictFailure("Resource already exists");
        var assertion = new ConflictErrorAssertion(error);

        // Act (When) & Assert (Then)
        Assert.Throws<Xunit.Sdk.EqualException>(() => assertion.AndCode("WRONG_CODE"));
    }

    [Fact(DisplayName = "Fluent chaining works with multiple assertions")]
    public void FluentChaining_WithMultipleAssertions_Works()
    {
        // Arrange (Given)
        var error = new ConflictFailure("Resource already exists");
        var assertion = new ConflictErrorAssertion(error);

        // Act (When) & Assert (Then)
        assertion
            .AndCode("CONFLICT")
            .AndMessage("Resource already exists");
    }

    [Fact(DisplayName = "And allows custom assertions on error properties")]
    public void And_AllowsCustomAssertions_OnErrorProperties()
    {
        // Arrange (Given)
        var error = new ConflictFailure("Resource already exists");
        var assertion = new ConflictErrorAssertion(error);

        // Act (When) & Assert (Then)
        assertion.And(e =>
        {
            Assert.Equal("CONFLICT", e.Code);
            Assert.Contains("already exists", e.Message);
        });
    }
}
