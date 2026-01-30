using UnambitiousFx.Functional.xunit;
using ExceptionalFailure = UnambitiousFx.Functional.Failures.ExceptionalFailure;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Try extension method on Result.
/// </summary>
public sealed partial class ResultExtensions
{
    [Fact]
    public void Try_WithSuccessAndFunctionSucceeds_ReturnsTransformedValue()
    {
        // Arrange (Given)
        var result = Result.Success(5);

        // Act (When)
        var transformed = result.Try(x => x * 2);

        // Assert (Then)
        transformed.ShouldBe().Success().And(value => Assert.Equal(10, value));
    }

    [Fact]
    public void Try_WithSuccessAndFunctionThrows_ReturnsFailureWithException()
    {
        // Arrange (Given)
        var result = Result.Success(5);

        // Act (When)
        var transformed = result.Try<int, int>(x => throw new InvalidOperationException("Test exception"));

        // Assert (Then)
        transformed.ShouldBe().Failure();
        transformed.TryGetError(out Functional.Failures.Failure? error);
        var exceptionalError = Assert.IsType<ExceptionalFailure>(error);
        Assert.IsType<InvalidOperationException>(exceptionalError.Exception);
    }

    [Fact]
    public void Try_WithFailure_ReturnsOriginalFailure()
    {
        // Arrange (Given)
        var result = Result.Failure<int>("Original error");

        // Act (When)
        var transformed = result.Try(x => x * 2);

        // Assert (Then)
        transformed.ShouldBe().Failure().AndMessage("Original error");
    }

    [Fact]
    public void Try_WithSuccessAndDivisionByZero_CatchesException()
    {
        // Arrange (Given)
        var result = Result.Success(10);

        // Act (When)
        var transformed = result.Try(x => x / 0);

        // Assert (Then)
        transformed.ShouldBe().Failure();
        transformed.TryGetError(out Functional.Failures.Failure? error);
        var exceptionalError = Assert.IsType<ExceptionalFailure>(error);
        Assert.IsType<DivideByZeroException>(exceptionalError.Exception);
    }

    [Fact]
    public void Try_AllowsTypeTransformation()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var transformed = result.Try(x => x.ToString());

        // Assert (Then)
        transformed.ShouldBe().Success().And(value => Assert.Equal("42", value));
    }

    [Fact]
    public void Try_CanBeChainedWithOtherOperations()
    {
        // Arrange (Given)
        var result = Result.Success(10);

        // Act (When)
        var final = result
            .Try(x => x * 2)
            .Try(x => x + 5);

        // Assert (Then)
        final.ShouldBe().Success().And(value => Assert.Equal(25, value));
    }


    [Fact]
    public void Try_WithFileOperation_CapturesIOException()
    {
        // Arrange (Given)
        var result = Result.Success("/nonexistent/path/file.txt");

        // Act (When)
        var tried = result.Try(path => File.ReadAllText(path));

        // Assert (Then)
        tried.ShouldBe().Failure();
    }

    [Fact]
    public void Try_WithListAccess_CapturesIndexOutOfRangeException()
    {
        // Arrange (Given)
        var result = Result.Success(new List<int> { 1, 2, 3 });

        // Act (When)
        var tried = result.Try(list => list[10]); // Index out of range

        // Assert (Then)
        tried.ShouldBe().Failure();
        Assert.False(tried.TryGet(out _, out var error));
        Assert.IsType<ExceptionalFailure>(error);
    }

    [Fact]
    public void Try_CanBeUsedForSafeTypeConversion()
    {
        // Arrange (Given)
        var result = Result.Success("42");

        // Act (When)
        var tried = result.Try(x => int.Parse(x));

        // Assert (Then)
        tried.ShouldBe().Success().And(value => Assert.Equal(42, value));
    }

    [Fact]
    public void Try_WithSuccess_ExecutesFunction()
    {
        // Arrange (Given)
        var result = Result.Success(5);

        // Act (When)
        var tried = result.Try(x => x * 2);

        // Assert (Then)
        tried.ShouldBe().Success().And(value => Assert.Equal(10, value));
    }

    [Fact]
    public void Try_WithFailure_PropagatesError()
    {
        // Arrange (Given)
        var error = new Functional.Failures.Failure("Initial error");
        var result = Result.Failure<int>(error);

        // Act (When)
        var tried = result.Try(x => x * 2);

        // Assert (Then)
        tried.ShouldBe().Failure().AndMessage("Initial error");
    }

    [Fact]
    public void Try_WhenFunctionThrows_CapturesException()
    {
        // Arrange (Given)
        var result = Result.Success(0);

        // Act (When)
        var tried = result.Try(x => 10 / x); // Division by zero

        // Assert (Then)
        tried.ShouldBe().Failure();
    }

    [Fact]
    public void Try_CapturesSpecificException()
    {
        // Arrange (Given)
        var result = Result.Success("not a number");

        // Act (When)
        var tried = result.Try(x => int.Parse(x));

        // Assert (Then)
        tried.ShouldBe().Failure();
        Assert.False(tried.TryGet(out _, out var error));
        Assert.IsType<ExceptionalFailure>(error);
    }

    [Fact]
    public void Try_WithSuccess_ChangesType()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var tried = result.Try(x => x.ToString());

        // Assert (Then)
        tried.ShouldBe().Success().And(value => Assert.Equal("42", value));
    }

    [Fact]
    public void Try_CanChainMultipleTry()
    {
        // Arrange (Given)
        var result = Result.Success("42");

        // Act (When)
        var final = result
            .Try(x => int.Parse(x))
            .Try(x => x * 2)
            .Try(x => x.ToString());

        // Assert (Then)
        final.ShouldBe().Success().And(value => Assert.Equal("84", value));
    }

    [Fact]
    public void Try_SuccessChainStopsAtFirstException()
    {
        // Arrange (Given)
        var result = Result.Success("not a number");
        var thirdCalled = false;

        // Act (When)
        var final = result
            .Try(x => int.Parse(x)) // This will throw
            .Try(x => x * 2)
            .Try(x =>
            {
                thirdCalled = true;
                return x.ToString();
            });

        // Assert (Then)
        final.ShouldBe().Failure();
        Assert.False(thirdCalled);
    }

    [Fact]
    public void Try_FailureChainStopsAtFirstException()
    {
        // Arrange (Given)
        var result = Result.Success(10);

        // Act (When)
        var final = result
            .Try(x => x * 2)
            .Try<int, int>(x => throw new InvalidOperationException("First exception"))
            .Try(x => x + 100); // This should not execute

        // Assert (Then)
        final.ShouldBe().Failure();
    }
}
