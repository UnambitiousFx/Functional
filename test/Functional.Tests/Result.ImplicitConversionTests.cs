using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.Tests;

public sealed class ResultImplicitConversionTests
{
    [Fact]
    public void ImplicitConversion_FromErrorToResult_CreatesFailedResult()
    {
        // Arrange (Given)
        var error = new Failure("some error");

        // Act (When)
        Result r = error; // implicit conversion

        // Assert (Then)
        Assert.False(r.IsSuccess);
        Assert.False(r.TryGetError(out var e));
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
        var err = new Failure("typed error");

        // Act (When)
        Result<int> r = err; // implicit conversion

        // Assert (Then)
        Assert.False(r.IsSuccess);
        Assert.False(r.TryGet(out var value2, out var e2));
        Assert.NotNull(e2);
        Assert.Equal("typed error", e2!.Message);
    }
}
