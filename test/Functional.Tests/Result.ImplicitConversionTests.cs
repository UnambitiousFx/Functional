using UnambitiousFx.Functional.Errors;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed class ResultImplicitConversionTests
{
    [Fact]
    public void ImplicitConversion_FromSuccess_PreservesSuccessAndMetadata()
    {
        // Arrange (Given)
        var typed = Result.Success(42).WithMetadata("key", "value");

        // Act (When)
        Result untyped = typed; // implicit conversion

        // Assert (Then)
        Assert.True(untyped.IsSuccess);
        Assert.True(untyped.TryGet(out var error));
        Assert.Null(error);
        Assert.Single(untyped.Metadata);
        Assert.Equal("value", untyped.Metadata["key"]);
    }

    [Fact]
    public void ImplicitConversion_FromFailure_PreservesFailureAndMetadataAndError()
    {
        // Arrange (Given)
        var originalError = new Error("Test failure");
        var typed = Result.Failure<int>(originalError).WithMetadata("k", 123);

        // Act (When)
        Result untyped = typed; // implicit conversion

        // Assert (Then)
        Assert.False(untyped.IsSuccess);
        Assert.False(untyped.TryGet(out var error));
        Assert.NotNull(error);
        Assert.Equal("Test failure", error!.Message);
        Assert.Single(untyped.Metadata);
        Assert.Equal(123, untyped.Metadata["k"]);
    }

    [Fact]
    public void ImplicitConversion_FromErrorToResult_CreatesFailedResult()
    {
        // Arrange (Given)
        var error = new Error("some error");

        // Act (When)
        Result r = error; // implicit conversion

        // Assert (Then)
        Assert.False(r.IsSuccess);
        Assert.False(r.TryGet(out var e));
        Assert.NotNull(e);
        Assert.Equal("some error", e!.Message);
    }

    [Fact]
    public void ImplicitConversion_FromValueToResultOfT_CreatesSuccessfulResult()
    {
        // Arrange (Given)
        Result<int> r = 7; // implicit conversion from value

        // Act & Assert (Then)
        Assert.True(r.IsSuccess);
        var ok = r.TryGet(out var value, out var err);
        Assert.True(ok);
        Assert.Equal(7, value);
        Assert.Null(err);
    }

    [Fact]
    public void ImplicitConversion_FromErrorToResultOfT_CreatesFailedResult()
    {
        // Arrange (Given)
        var err = new Error("typed error");

        // Act (When)
        Result<int> r = err; // implicit conversion

        // Assert (Then)
        Assert.False(r.IsSuccess);
        Assert.False(r.TryGet(out var value2, out var e2));
        Assert.NotNull(e2);
        Assert.Equal("typed error", e2!.Message);
    }
}