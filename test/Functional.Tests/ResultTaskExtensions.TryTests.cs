using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for async Try extension methods on Result using ValueTask.
/// </summary>
public sealed partial class ResultTaskExtensionsTests
{
    [Fact]
    public async Task Try_WithSyncResult_WithSuccessAndAsyncFunctionSucceeds_ReturnsTransformedValue()
    {
        // Arrange (Given)
        var result = Result.Success(5);

        // Act (When)
        var transformed = await result.AsAsync().Try(async x =>
        {
            await ValueTask.CompletedTask;
            return x * 2;
        });

        // Assert (Then)
        transformed.ShouldBe().Success().And(value => Assert.Equal(10, value));
    }

    [Fact]
    public async Task Try_WithSyncResult_WithSuccessAndAsyncFunctionThrows_ReturnsFailureWithException()
    {
        // Arrange (Given)
        var result = Result.Success(5);

        // Act (When)
        var transformed = await result.AsAsync().Try<int, int>(async x =>
        {
            await ValueTask.CompletedTask;
            throw new InvalidOperationException("Test exception");
        });

        // Assert (Then)
        transformed.ShouldBe().Failure();
        transformed.TryGetError(out var error);
        var exceptionalError = Assert.IsType<ExceptionalFailure>(error);
        Assert.IsType<InvalidOperationException>(exceptionalError.Exception);
    }

    [Fact]
    public async Task Try_WithSyncResult_WithFailure_ReturnsOriginalFailure()
    {
        // Arrange (Given)
        var result = Result.Failure<int>("Original error");

        // Act (When)
        var transformed = await result.AsAsync().Try(async x =>
        {
            await ValueTask.CompletedTask;
            return x * 2;
        });

        // Assert (Then)
        transformed.ShouldBe().Failure().AndMessage("Original error");
    }

    [Fact]
    public async Task Try_WithSyncResult_WithSuccessAndDivisionByZero_CatchesException()
    {
        // Arrange (Given)
        var result = Result.Success(10);

        // Act (When)
        var transformed = await result.AsAsync().Try(async x =>
        {
            await ValueTask.CompletedTask;
            return x / 0;
        });

        // Assert (Then)
        transformed.ShouldBe().Failure();
        transformed.TryGetError(out var error);
        var exceptionalError = Assert.IsType<ExceptionalFailure>(error);
        Assert.IsType<DivideByZeroException>(exceptionalError.Exception);
    }

    [Fact]
    public async Task Try_WithSyncResult_AllowsTypeTransformation()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var transformed = await result.AsAsync().Try(async x =>
        {
            await ValueTask.CompletedTask;
            return x.ToString();
        });

        // Assert (Then)
        transformed.ShouldBe().Success().And(value => Assert.Equal("42", value));
    }

    [Fact]
    public async Task Try_WithAsyncResult_WithSuccessAndAsyncFunctionSucceeds_ReturnsTransformedValue()
    {
        // Arrange (Given)
        var result = Result.Success(5).AsAsync();

        // Act (When)
        var transformed = await result.Try(async x =>
        {
            await ValueTask.CompletedTask;
            return x * 2;
        });

        // Assert (Then)
        transformed.ShouldBe().Success().And(value => Assert.Equal(10, value));
    }

    [Fact]
    public async Task Try_WithAsyncResult_WithSuccessAndAsyncFunctionThrows_ReturnsFailureWithException()
    {
        // Arrange (Given)
        var result = Result.Success(5).AsAsync();

        // Act (When)
        var transformed = await result.Try<int, int>(async x =>
        {
            await ValueTask.CompletedTask;
            throw new InvalidOperationException("Test exception");
        });

        // Assert (Then)
        transformed.ShouldBe().Failure();
        transformed.TryGetError(out var error);
        var exceptionalError = Assert.IsType<ExceptionalFailure>(error);
        Assert.IsType<InvalidOperationException>(exceptionalError.Exception);
    }

    [Fact]
    public async Task Try_WithAsyncResult_WithFailure_ReturnsOriginalFailure()
    {
        // Arrange (Given)
        var result = Result.Failure<int>("Original error").AsAsync();

        // Act (When)
        var transformed = await result.Try(async x =>
        {
            await ValueTask.CompletedTask;
            return x * 2;
        });

        // Assert (Then)
        transformed.ShouldBe().Failure().AndMessage("Original error");
    }

    [Fact]
    public async Task Try_WithAsyncResult_WithSuccessAndDivisionByZero_CatchesException()
    {
        // Arrange (Given)
        var result = Result.Success(10).AsAsync();

        // Act (When)
        var transformed = await result.Try(async x =>
        {
            await ValueTask.CompletedTask;
            return x / 0;
        });

        // Assert (Then)
        transformed.ShouldBe().Failure();
        transformed.TryGetError(out var error);
        var exceptionalError = Assert.IsType<ExceptionalFailure>(error);
        Assert.IsType<DivideByZeroException>(exceptionalError.Exception);
    }

    [Fact]
    public async Task Try_WithAsyncResult_AllowsTypeTransformation()
    {
        // Arrange (Given)
        var result = Result.Success(42).AsAsync();

        // Act (When)
        var transformed = await result.Try(async x =>
        {
            await ValueTask.CompletedTask;
            return x.ToString();
        });

        // Assert (Then)
        transformed.ShouldBe().Success().And(value => Assert.Equal("42", value));
    }


    [Fact]
    public async Task Try_WithAsyncResult_WithSuccessAndSyncFunctionSucceeds_ReturnsTransformedValue()
    {
        // Arrange (Given)
        var result = Result.Success(5).AsAsync();

        // Act (When)
        var transformed = await result.Try(x => { return x * 2; });

        // Assert (Then)
        transformed.ShouldBe().Success().And(value => Assert.Equal(10, value));
    }

    [Fact]
    public async Task Try_WithAsyncResult_WithSuccessAndSyncFunctionThrows_ReturnsFailureWithException()
    {
        // Arrange (Given)
        var result = Result.Success(5).AsAsync();

        Func<int, int> syncFunction = x => { throw new InvalidOperationException("Test exception"); };
        // Act (When)
        var transformed = await result.Try(syncFunction);

        // Assert (Then)
        transformed.ShouldBe().Failure();
        transformed.TryGetError(out var error);
        var exceptionalError = Assert.IsType<ExceptionalFailure>(error);
        Assert.IsType<InvalidOperationException>(exceptionalError.Exception);
    }
}