using UnambitiousFx.Functional.Failures;
using Xunit.Sdk;

namespace UnambitiousFx.Functional.xunit.Tests;

public class ConflictFailureAssertionTests
{
    [Fact]
    public void And_ExecutesCustomAssertion_ReturnsSelf()
    {
        // Arrange (Given)
        var error     = new ConflictFailure("Resource already exists");
        var assertion = new ConflictFailureAssertion(error);

        // Act (When)
        var result = assertion.And(e => Assert.NotNull(e));

        // Assert (Then)
        Assert.Equal(error, result.Failure);
    }

    [Fact]
    public void AndMessage_WhenMessageMatches_Succeeds()
    {
        // Arrange (Given)
        var error     = new ConflictFailure("Resource already exists");
        var assertion = new ConflictFailureAssertion(error);

        // Act (When)
        var result = assertion.AndMessage("Resource already exists");

        // Assert (Then)
        Assert.Equal(error, result.Failure);
    }

    [Fact]
    public void AndMessage_WhenMessageDoesNotMatch_Throws()
    {
        // Arrange (Given)
        var error     = new ConflictFailure("Resource already exists");
        var assertion = new ConflictFailureAssertion(error);

        // Act (When) & Assert (Then)
        Assert.Throws<EqualException>(() => assertion.AndMessage("Wrong message"));
    }

    [Fact]
    public void AndCode_WhenCodeMatches_Succeeds()
    {
        // Arrange (Given)
        var error     = new ConflictFailure("Resource already exists");
        var assertion = new ConflictFailureAssertion(error);

        // Act (When)
        var result = assertion.AndCode("CONFLICT");

        // Assert (Then)
        Assert.Equal(error, result.Failure);
    }

    [Fact]
    public void AndCode_WhenCodeDoesNotMatch_Throws()
    {
        // Arrange (Given)
        var error     = new ConflictFailure("Resource already exists");
        var assertion = new ConflictFailureAssertion(error);

        // Act (When) & Assert (Then)
        Assert.Throws<EqualException>(() => assertion.AndCode("WRONG_CODE"));
    }

    [Fact]
    public void FluentChaining_WithMultipleAssertions_Works()
    {
        // Arrange (Given)
        var error     = new ConflictFailure("Resource already exists");
        var assertion = new ConflictFailureAssertion(error);

        // Act (When) & Assert (Then)
        assertion
           .AndCode("CONFLICT")
           .AndMessage("Resource already exists");
    }

    [Fact]
    public void And_AllowsCustomAssertions_OnErrorProperties()
    {
        // Arrange (Given)
        var error     = new ConflictFailure("Resource already exists");
        var assertion = new ConflictFailureAssertion(error);

        // Act (When) & Assert (Then)
        assertion.And(e =>
        {
            Assert.Equal("CONFLICT", e.Code);
            Assert.Contains("already exists", e.Message);
        });
    }
}
