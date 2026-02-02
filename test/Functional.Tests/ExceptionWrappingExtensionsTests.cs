using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.Tests;

public sealed class ExceptionWrappingExtensionsTests
{
    [Fact]
    public void Wrap_WithException_CreatesExceptionalFailure()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("Test error");

        // Act (When)
        var failure = exception.Wrap();

        // Assert (Then)
        Assert.IsType<ExceptionalFailure>(failure);
        Assert.Same(exception, failure.Exception);
        Assert.Equal("Test error", failure.Message);
    }

    [Fact]
    public void Wrap_WithMessageOverride_UsesOverriddenMessage()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("Original message");

        // Act (When)
        var failure = exception.Wrap(messageOverride: "Custom message");

        // Assert (Then)
        Assert.Equal("Custom message", failure.Message);
        Assert.Same(exception, failure.Exception);
    }

    [Fact]
    public void Wrap_WithExtra_IncludesMetadata()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("Test error");
        var extra = new Dictionary<string, object?> { ["key1"] = "value1", ["key2"] = 42 };

        // Act (When)
        var failure = exception.Wrap(extra: extra);

        // Assert (Then)
        var hasKey1 = failure.Extra!.TryGetValue("key1", out var value1);
        Assert.True(hasKey1);
        Assert.Equal("value1", value1!);
        var hasKey2 = failure.Extra!.TryGetValue("key2", out var value2);
        Assert.True(hasKey2);
        Assert.Equal(42, value2!);
    }

    [Fact]
    public void AsError_BehavesLikeWrap()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("Test error");

        // Act (When)
        var failure = exception.AsError();

        // Assert (Then)
        Assert.IsType<ExceptionalFailure>(failure);
        Assert.Same(exception, failure.Exception);
        Assert.Equal("Test error", failure.Message);
    }

    [Fact]
    public void AsError_WithMessageOverride_UsesOverriddenMessage()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("Original message");

        // Act (When)
        var failure = exception.AsError(messageOverride: "Custom message");

        // Assert (Then)
        Assert.Equal("Custom message", failure.Message);
    }

    [Fact]
    public void AsError_WithExtra_IncludesMetadata()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("Test error");
        var extra = new Dictionary<string, object?> { ["key"] = "value" };

        // Act (When)
        var failure = exception.AsError(extra: extra);

        // Assert (Then)
        var hasKey = failure.Extra!.TryGetValue("key", out var value);
        Assert.True(hasKey);
        Assert.Equal("value", value!);
    }

    [Fact]
    public void WrapAndPrepend_WithEmptyContext_BehavesLikeWrap()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("Test error");

        // Act (When)
        var failure = exception.WrapAndPrepend(string.Empty);

        // Assert (Then)
        Assert.IsType<ExceptionalFailure>(failure);
        Assert.Equal("Test error", failure.Message);
    }

    [Fact]
    public void WrapAndPrepend_WithNullContext_BehavesLikeWrap()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("Test error");

        // Act (When)
        var failure = exception.WrapAndPrepend(null!);

        // Assert (Then)
        Assert.IsType<ExceptionalFailure>(failure);
        Assert.Equal("Test error", failure.Message);
    }

    [Fact]
    public void WrapAndPrepend_WithValidContext_PrependsContext()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("Test error");

        // Act (When)
        var failure = exception.WrapAndPrepend("Context: ");

        // Assert (Then)
        Assert.Equal("Context: Test error", failure.Message);
        Assert.Same(exception, failure.Exception);
    }

    [Fact]
    public void WrapAndPrepend_WithContextAndMessageOverride_UsesOverriddenMessage()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("Original message");

        // Act (When)
        var failure = exception.WrapAndPrepend("Context: ", messageOverride: "Custom message");

        // Assert (Then)
        Assert.Equal("Context: Custom message", failure.Message);
    }

    [Fact]
    public void WrapAndPrepend_WithExtra_IncludesMetadata()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("Test error");
        var extra = new Dictionary<string, object?> { ["key"] = "value" };

        // Act (When)
        var failure = exception.WrapAndPrepend("Context: ", extra: extra);

        // Assert (Then)
        var hasKey = failure.Extra!.TryGetValue("key", out var value);
        Assert.True(hasKey);
        Assert.Equal("value", value!);
    }
}
