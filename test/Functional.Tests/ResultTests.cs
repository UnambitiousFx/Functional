using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed class ResultTests
{
    #region Success Creation

    [Fact]
    public void Success_CreatesSuccessfulResult()
    {
        // Arrange (Given) & Act (When)
        var result = Result.Success();

        // Assert (Then)
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFaulted);
        result.ShouldBe().Success();
    }

    [Fact]
    public void Success_WithValue_CreatesSuccessfulResult()
    {
        // Arrange (Given) & Act (When)
        var result = Result.Success(42);

        // Assert (Then)
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFaulted);
        result.ShouldBe().Success().And(v => Assert.Equal(42, v));
    }

    [Fact]
    public void Success_WithStringValue_CreatesSuccessfulResult()
    {
        // Arrange (Given) & Act (When)
        var result = Result.Success("hello");

        // Assert (Then)
        result.ShouldBe().Success().And(v => Assert.Equal("hello", v));
    }

    #endregion

    #region Failure Creation

    [Fact]
    public void Failure_FromException_CreatesFailedResult()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("Test error");

        // Act (When)
        var result = Result.Failure(exception);

        // Assert (Then)
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFaulted);
        result.ShouldBe().Failure();
    }

    [Fact]
    public void Failure_FromError_CreatesFailedResult()
    {
        // Arrange (Given)
        var error = new Failure("Invalid input");

        // Act (When)
        var result = Result.Failure(error);

        // Assert (Then)
        result.ShouldBe().Failure().AndMessage("Invalid input");
    }

    [Fact]
    public void Failure_FromString_CreatesFailedResult()
    {
        // Arrange (Given)
        var message = "Something went wrong";

        // Act (When)
        var result = Result.Failure(message);

        // Assert (Then)
        result.ShouldBe().Failure().AndMessage(message);
    }

    [Fact]
    public void Failure_FromMultipleErrors_CreatesAggregateError()
    {
        // Arrange (Given)
        var errors = new[] { new Failure("Error 1"), new Failure("Error 2"), new Failure("Error 3") };

        // Act (When)
        var result = Result.Failure(errors.AsEnumerable());

        // Assert (Then)
        result.ShouldBe().Failure();
        Assert.False(result.TryGetError(out var error));
        Assert.NotNull(error);
        Assert.IsType<AggregateFailure>(error);
    }

    [Fact]
    public void Failure_Generic_FromException_CreatesFailedResult()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("Test error");

        // Act (When)
        var result = Result.Failure<int>(exception);

        // Assert (Then)
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFaulted);
        result.ShouldBe().Failure();
    }

    [Fact]
    public void Failure_Generic_FromError_CreatesFailedResult()
    {
        // Arrange (Given)
        var error = new Failure("Invalid input");

        // Act (When)
        var result = Result.Failure<string>(error);

        // Assert (Then)
        result.ShouldBe().Failure().AndMessage("Invalid input");
    }

    [Fact]
    public void Failure_Generic_FromString_CreatesFailedResult()
    {
        // Arrange (Given)
        var message = "Something went wrong";

        // Act (When)
        var result = Result.Failure<bool>(message);

        // Assert (Then)
        result.ShouldBe().Failure().AndMessage(message);
    }

    [Fact]
    public void Failure_Generic_FromMultipleErrors_CreatesAggregateError()
    {
        // Arrange (Given)
        var errors = new[] { new Failure("Error 1"), new Failure("Error 2") };

        // Act (When)
        var result = Result.Failure<int>(errors.AsEnumerable());

        // Assert (Then)
        result.ShouldBe().Failure();
    }

    #endregion

    #region TryGet

    [Fact]
    public void TryGetError_WithSuccess_ReturnsTrueAndNullError()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var success = result.TryGetError(out var error);

        // Assert (Then)
        Assert.True(success);
        Assert.Null(error);
    }

    [Fact]
    public void TryGetError_WithFailure_ReturnsFalseAndError()
    {
        // Arrange (Given)
        var testError = new Failure("Test error");
        var result = Result.Failure(testError);

        // Act (When)
        var success = result.TryGetError(out var error);

        // Assert (Then)
        Assert.False(success);
        Assert.NotNull(error);
        Assert.Equal("Test error", error.Message);
    }

    #endregion

    #region Match

    [Fact]
    public void Match_Action_WithSuccess_ExecutesSuccessAction()
    {
        // Arrange (Given)
        var result = Result.Success();
        var successCalled = false;
        var failureCalled = false;

        // Act (When)
        result.Match(
            () => successCalled = true,
            _ => failureCalled = true);

        // Assert (Then)
        Assert.True(successCalled);
        Assert.False(failureCalled);
    }

    [Fact]
    public void Match_Action_WithFailure_ExecutesFailureAction()
    {
        // Arrange (Given)
        var result = Result.Failure("Error");
        var successCalled = false;
        var failureCalled = false;
        Failure? capturedError = null;

        // Act (When)
        result.Match(
            () => successCalled = true,
            e =>
            {
                failureCalled = true;
                capturedError = e;
            });

        // Assert (Then)
        Assert.False(successCalled);
        Assert.True(failureCalled);
        Assert.NotNull(capturedError);
    }

    [Fact]
    public void Match_Func_WithSuccess_ReturnsSuccessValue()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var value = result.Match(
            () => "success",
            _ => "failure");

        // Assert (Then)
        Assert.Equal("success", value);
    }

    [Fact]
    public void Match_Func_WithFailure_ReturnsFailureValue()
    {
        // Arrange (Given)
        var result = Result.Failure("Error");

        // Act (When)
        var value = result.Match(
            () => "success",
            e => $"failure: {e.Message}");

        // Assert (Then)
        Assert.StartsWith("failure:", value);
    }

    #endregion

    #region IfSuccess / IfFailure

    [Fact]
    public void IfSuccess_WithSuccess_ExecutesAction()
    {
        // Arrange (Given)
        var result = Result.Success();
        var called = false;

        // Act (When)
        result.IfSuccess(() => called = true);

        // Assert (Then)
        Assert.True(called);
    }

    [Fact]
    public void IfSuccess_WithFailure_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var result = Result.Failure("Error");
        var called = false;

        // Act (When)
        result.IfSuccess(() => called = true);

        // Assert (Then)
        Assert.False(called);
    }

    [Fact]
    public void IfFailure_WithSuccess_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var result = Result.Success();
        var called = false;

        // Act (When)
        result.IfFailure(_ => called = true);

        // Assert (Then)
        Assert.False(called);
    }

    [Fact]
    public void IfFailure_WithFailure_ExecutesAction()
    {
        // Arrange (Given)
        var result = Result.Failure("Error");
        var called = false;
        Failure? capturedError = null;

        // Act (When)
        result.IfFailure(e =>
        {
            called = true;
            capturedError = e;
        });

        // Assert (Then)
        Assert.True(called);
        Assert.NotNull(capturedError);
    }

    #endregion

    #region Deconstruct

    [Fact]
    public void Deconstruct_WithSuccess_ReturnsNullError()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        result.Deconstruct(out var error);

        // Assert (Then)
        Assert.Null(error);
    }

    [Fact]
    public void Deconstruct_WithFailure_ReturnsError()
    {
        // Arrange (Given)
        var testError = new Failure("Test error");
        var result = Result.Failure(testError);

        // Act (When)
        result.Deconstruct(out var error);

        // Assert (Then)
        Assert.NotNull(error);
        Assert.Equal("Test error", error.Message);
    }

    #endregion

    #region Metadata

    [Fact]
    public void Metadata_EmptyByDefault()
    {
        // Arrange (Given) & Act (When)
        var result = Result.Success();

        // Assert (Then)
        Assert.Empty(result.Metadata);
    }

    [Fact]
    public void WithMetadata_KeyValue_AddsMetadata()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var resultWithMeta = result.WithMetadata("key", "value");

        // Assert (Then)
        Assert.Single(resultWithMeta.Metadata);
        Assert.Equal("value", resultWithMeta.Metadata["key"]);
    }

    [Fact]
    public void WithMetadata_IReadOnlyMetadata_MergesMetadata()
    {
        // Arrange (Given)
        var result = Result.Success().WithMetadata("key1", "value1");
        var additionalMeta = new Metadata { ["key2"] = "value2" };

        // Act (When)
        var resultWithMeta = result.WithMetadata(additionalMeta);

        // Assert (Then)
        Assert.Equal(2, resultWithMeta.Metadata.Count);
        Assert.Equal("value1", resultWithMeta.Metadata["key1"]);
        Assert.Equal("value2", resultWithMeta.Metadata["key2"]);
    }

    [Fact]
    public void WithMetadata_KeyValuePairs_MergesMetadata()
    {
        // Arrange (Given)
        var result = Result.Success();
        var kvps = new[]
        {
            new KeyValuePair<string, object?>("key1", "value1"), new KeyValuePair<string, object?>("key2", 42)
        };

        // Act (When)
        var resultWithMeta = result.WithMetadata(kvps);

        // Assert (Then)
        Assert.Equal(2, resultWithMeta.Metadata.Count);
        Assert.Equal("value1", resultWithMeta.Metadata["key1"]);
        Assert.Equal(42, resultWithMeta.Metadata["key2"]);
    }

    [Fact]
    public void WithMetadata_Tuples_AddsMetadata()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var resultWithMeta = result.WithMetadata(
            ("key1", "value1"),
            ("key2", 42),
            ("key3", true));

        // Assert (Then)
        Assert.Equal(3, resultWithMeta.Metadata.Count);
        Assert.Equal("value1", resultWithMeta.Metadata["key1"]);
        Assert.Equal(42, resultWithMeta.Metadata["key2"]);
        Assert.Equal(true, resultWithMeta.Metadata["key3"]);
    }

    [Fact]
    public void WithMetadata_Builder_ConfiguresMetadata()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var resultWithMeta = result.WithMetadata(builder =>
        {
            builder.Add("key1", "value1");
            builder.Add("key2", 42);
        });

        // Assert (Then)
        Assert.Equal(2, resultWithMeta.Metadata.Count);
        Assert.Equal("value1", resultWithMeta.Metadata["key1"]);
        Assert.Equal(42, resultWithMeta.Metadata["key2"]);
    }

    [Fact]
    public void WithMetadata_PreservesSuccessState()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var resultWithMeta = result.WithMetadata("key", "value");

        // Assert (Then)
        Assert.True(resultWithMeta.IsSuccess);
    }

    [Fact]
    public void WithMetadata_PreservesFailureState()
    {
        // Arrange (Given)
        var result = Result.Failure("Error");

        // Act (When)
        var resultWithMeta = result.WithMetadata("key", "value");

        // Assert (Then)
        Assert.False(resultWithMeta.IsSuccess);
        resultWithMeta.ShouldBe().Failure();
    }

    #endregion

    #region ToString

    [Fact]
    public void ToString_WithSuccess_ReturnsSuccessString()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var str = result.ToString();

        // Assert (Then)
        Assert.Equal("Success", str);
    }

    [Fact]
    public void ToString_WithSuccess_AndMetadata_IncludesMetadata()
    {
        // Arrange (Given)
        var result = Result.Success().WithMetadata("key", "value");

        // Act (When)
        var str = result.ToString();

        // Assert (Then)
        Assert.StartsWith("Success meta=", str);
    }

    [Fact]
    public void ToString_WithFailure_ReturnsFailureString()
    {
        // Arrange (Given)
        var result = Result.Failure("Test error");

        // Act (When)
        var str = result.ToString();

        // Assert (Then)
        Assert.StartsWith("Failure error=", str);
    }

    [Fact]
    public void ToString_WithFailure_AndMetadata_IncludesMetadata()
    {
        // Arrange (Given)
        var result = Result.Failure("Error").WithMetadata("key", "value");

        // Act (When)
        var str = result.ToString();

        // Assert (Then)
        Assert.Contains("meta=", str);
    }

    #endregion
}
