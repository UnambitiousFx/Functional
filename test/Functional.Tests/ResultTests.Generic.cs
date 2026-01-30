using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed class ResultOfTValueTests
{
    #region Helper Types

    private record TestData(int Id, string Name);

    #endregion

    #region Success with Value

    [Fact]
    public void Success_WithIntValue_CreatesSuccessfulResult()
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
        var result = Result.Success("hello world");

        // Assert (Then)
        result.ShouldBe().Success().And(v => Assert.Equal("hello world", v));
    }

    [Fact]
    public void Success_WithComplexType_CreatesSuccessfulResult()
    {
        // Arrange (Given)
        var data = new TestData(42, "test");

        // Act (When)
        var result = Result.Success(data);

        // Assert (Then)
        result.ShouldBe().Success().And(v =>
        {
            Assert.Equal(42, v.Id);
            Assert.Equal("test", v.Name);
        });
    }

    #endregion

    #region TryGet with Value

    [Fact]
    public void TryGet_WithSuccess_ReturnsTrueAndNullError()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var success = result.TryGet(out var value, out var error);

        // Assert (Then)
        Assert.True(success);
        Assert.Equal(42, value);
        Assert.Null(error);
    }

    [Fact]
    public void TryGet_WithFailure_ReturnsFalseAndError()
    {
        // Arrange (Given)
        var testError = new Failure("Test error");
        var result = Result.Failure<int>(testError);

        // Act (When)
        var success = result.TryGet(out var value, out var error);

        // Assert (Then)
        Assert.False(success);
        Assert.Equal(default, value);
        Assert.NotNull(error);
        Assert.Equal("Test error", error.Message);
    }

    #endregion

    #region Match with Value

    [Fact]
    public void Match_Action_WithSuccess_ExecutesSuccessAction()
    {
        // Arrange (Given)
        var result = Result.Success(42);
        var successCalled = false;
        var failureCalled = false;

        // Act (When)
        result.Match(
            () => successCalled = true,
            _ =>
            {
                failureCalled = true;
            });

        // Assert (Then)
        Assert.True(successCalled);
        Assert.False(failureCalled);
    }

    [Fact]
    public void Match_Action_WithFailure_ExecutesFailureAction()
    {
        // Arrange (Given)
        var result = Result.Failure<int>("Error");
        var successCalled = false;
        var failureCalled = false;

        // Act (When)
        result.Match(
            () => successCalled = true,
            _ =>
            {
                failureCalled = true;
            });

        // Assert (Then)
        Assert.False(successCalled);
        Assert.True(failureCalled);
    }

    [Fact]
    public void Match_Func_WithSuccess_ReturnsSuccessValue()
    {
        // Arrange (Given)
        var result = Result.Success(42);

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
        var result = Result.Failure<int>("Error");

        // Act (When)
        var value = result.Match(
            () => "success",
            e => $"failure: {e.Message}");

        // Assert (Then)
        Assert.StartsWith("failure:", value);
    }

    [Fact]
    public void Match_FuncWithValue_WithSuccess_PassesValue()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var value = result.Match(
            v => v * 2,
            _ => 0);

        // Assert (Then)
        Assert.Equal(84, value);
    }

    [Fact]
    public void Match_FuncWithValue_WithFailure_ExecutesFailureBranch()
    {
        // Arrange (Given)
        var result = Result.Failure<int>("Error");

        // Act (When)
        var value = result.Match(
            v => v * 2,
            _ => -1);

        // Assert (Then)
        Assert.Equal(-1, value);
    }

    [Fact]
    public void Match_ActionWithValue_WithSuccess_PassesValue()
    {
        // Arrange (Given)
        var result = Result.Success(42);
        var capturedValue = 0;

        // Act (When)
        result.Match(
            v => capturedValue = v,
            _ => { });

        // Assert (Then)
        Assert.Equal(42, capturedValue);
    }

    #endregion

    #region IfSuccess / IfFailure with Value

    [Fact]
    public void IfSuccess_WithSuccess_ExecutesAction()
    {
        // Arrange (Given)
        var result = Result.Success(42);
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
        var result = Result.Failure<int>("Error");
        var called = false;

        // Act (When)
        result.IfSuccess(() => called = true);

        // Assert (Then)
        Assert.False(called);
    }

    [Fact]
    public void IfSuccess_WithValue_WithSuccess_PassesValue()
    {
        // Arrange (Given)
        var result = Result.Success(42);
        var capturedValue = 0;

        // Act (When)
        result.IfSuccess(v => capturedValue = v);

        // Assert (Then)
        Assert.Equal(42, capturedValue);
    }

    [Fact]
    public void IfSuccess_WithValue_WithFailure_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var result = Result.Failure<int>("Error");
        var called = false;

        // Act (When)
        result.IfSuccess(_ => called = true);

        // Assert (Then)
        Assert.False(called);
    }

    [Fact]
    public void IfFailure_WithSuccess_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var result = Result.Success(42);
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
        var result = Result.Failure<int>("Error");
        var called = false;

        // Act (When)
        result.IfFailure(_ => called = true);

        // Assert (Then)
        Assert.True(called);
    }

    #endregion

    #region Deconstruct with Value

    [Fact]
    public void Deconstruct_WithSuccess_ReturnsValueAndNullError()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var (value, error) = result;

        // Assert (Then)
        Assert.Equal(42, value);
        Assert.Null(error);
    }

    [Fact]
    public void Deconstruct_WithFailure_ReturnsDefaultValueAndError()
    {
        // Arrange (Given)
        var testError = new Failure("Test error");
        var result = Result.Failure<int>(testError);

        // Act (When)
        var (value, error) = result;

        // Assert (Then)
        Assert.Equal(default, value);
        Assert.NotNull(error);
        Assert.Equal("Test error", error.Message);
    }

    #endregion

    #region Metadata with Value

    [Fact]
    public void Metadata_EmptyByDefault()
    {
        // Arrange (Given) & Act (When)
        var result = Result.Success(42);

        // Assert (Then)
        Assert.Empty(result.Metadata);
    }

    [Fact]
    public void WithMetadata_KeyValue_AddsMetadata()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var resultWithMeta = result.WithMetadata("key", "value");

        // Assert (Then)
        Assert.Single(resultWithMeta.Metadata);
        Assert.Equal("value", resultWithMeta.Metadata["key"]);
    }

    [Fact]
    public void WithMetadata_PreservesValue()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var resultWithMeta = result.WithMetadata("key", "value");

        // Assert (Then)
        resultWithMeta.ShouldBe().Success().And(v => Assert.Equal(42, v));
    }

    [Fact]
    public void WithMetadata_IReadOnlyMetadata_MergesMetadata()
    {
        // Arrange (Given)
        var result = Result.Success(42).WithMetadata("key1", "value1");
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
        var result = Result.Success(42);
        var kvps = new[]
        {
            new KeyValuePair<string, object?>("key1", "value1"), new KeyValuePair<string, object?>("key2", 100)
        };

        // Act (When)
        var resultWithMeta = result.WithMetadata(kvps);

        // Assert (Then)
        Assert.Equal(2, resultWithMeta.Metadata.Count);
    }

    [Fact]
    public void WithMetadata_Tuples_AddsMetadata()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var resultWithMeta = result.WithMetadata(
            ("key1", "value1"),
            ("key2", 100));

        // Assert (Then)
        Assert.Equal(2, resultWithMeta.Metadata.Count);
    }

    [Fact]
    public void WithMetadata_Builder_ConfiguresMetadata()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var resultWithMeta = result.WithMetadata(builder =>
        {
            builder.Add("key1", "value1");
            builder.Add("key2", 100);
        });

        // Assert (Then)
        Assert.Equal(2, resultWithMeta.Metadata.Count);
    }

    #endregion

    #region ToString with Value

    [Fact]
    public void ToString_WithSuccess_ReturnsSuccessStringWithValue()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var str = result.ToString();

        // Assert (Then)
        Assert.StartsWith("Success", str);
        Assert.Contains("42", str);
    }

    [Fact]
    public void ToString_WithFailure_ReturnsFailureString()
    {
        // Arrange (Given)
        var result = Result.Failure<int>("Test error");

        // Act (When)
        var str = result.ToString();

        // Assert (Then)
        Assert.StartsWith("Failure error=", str);
    }

    [Fact]
    public void ToString_WithSuccess_AndMetadata_IncludesMetadata()
    {
        // Arrange (Given)
        var result = Result.Success(42).WithMetadata("key", "value");

        // Act (When)
        var str = result.ToString();

        // Assert (Then)
        Assert.Contains("meta=", str);
    }

    #endregion
}
