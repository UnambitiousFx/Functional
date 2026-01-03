using UnambitiousFx.Functional.Errors;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result MapError extension methods.
/// </summary>
public sealed partial class ResultExtensions
{
    #region MapError - Result<T>

    [Fact]
    public void MapError_WithSuccess_DoesNotMapError()
    {
        // Arrange (Given)
        var result = Result.Success(42);
        var mapperCalled = false;

        // Act (When)
        var mapped = result.MapError(err =>
        {
            mapperCalled = true;
            return new Error("Should not be called");
        });

        // Assert (Then)
        mapped.ShouldBe().Success().And(value => Assert.Equal(42, value));
        Assert.False(mapperCalled);
    }

    [Fact]
    public void MapError_WithFailure_MapsError()
    {
        // Arrange (Given)
        var error = new Error("Original error");
        var result = Result.Failure<int>(error);

        // Act (When)
        var mapped = result.MapError(err => new Error("Mapped error"));

        // Assert (Then)
        mapped.ShouldBe().Failure().AndMessage("Mapped error");
    }

    [Fact]
    public void MapError_WithFailure_CanChangeErrorCode()
    {
        // Arrange (Given)
        var error = new Error("ERR_001", "Original error");
        var result = Result.Failure<int>(error);

        // Act (When)
        var mapped = result.MapError(err => new Error("ERR_002", err.Message));

        // Assert (Then)
        mapped.ShouldBe().Failure()
            .AndCode("ERR_002")
            .AndMessage("Original error");
    }

    [Fact]
    public void MapError_WithFailure_CanEnrichErrorMessage()
    {
        // Arrange (Given)
        var error = new Error("Database connection failed");
        var result = Result.Failure<int>(error);

        // Act (When)
        var mapped = result.MapError(err => new Error($"System error: {err.Message}"));

        // Assert (Then)
        mapped.ShouldBe().Failure().AndMessage("System error: Database connection failed");
    }

    #endregion

    #region MapError - Non-generic Result

    [Fact]
    public void MapError_NonGeneric_WithSuccess_DoesNotMapError()
    {
        // Arrange (Given)
        var result = Result.Success();
        var mapperCalled = false;

        // Act (When)
        var mapped = result.MapError(err =>
        {
            mapperCalled = true;
            return new Error("Should not be called");
        });

        // Assert (Then)
        mapped.ShouldBe().Success();
        Assert.False(mapperCalled);
    }

    [Fact]
    public void MapError_NonGeneric_WithFailure_MapsError()
    {
        // Arrange (Given)
        var error = new Error("Original error");
        var result = Result.Failure(error);

        // Act (When)
        var mapped = result.MapError(err => new Error("Mapped error"));

        // Assert (Then)
        mapped.ShouldBe().Failure().AndMessage("Mapped error");
    }

    #endregion

    #region MapError - Chaining

    [Fact]
    public void MapError_CanChainMultipleMappings()
    {
        // Arrange (Given)
        var error = new Error("ERR_001", "error");
        var result = Result.Failure<int>(error);

        // Act (When)
        var mapped = result
            .MapError(err => new Error("ERR_002", err.Message))
            .MapError(err => new Error("ERR_003", $"Final: {err.Message}"));

        // Assert (Then)
        mapped.ShouldBe().Failure()
            .AndCode("ERR_003")
            .AndMessage("Final: error");
    }

    [Fact]
    public void MapError_CanCombineWithOtherOperations()
    {
        // Arrange (Given)
        var result = Result.Success(5);

        // Act (When)
        var final = result
            .Map(x => x * 2)
            .Bind(x => x > 100 ? Result.Success(x) : Result.Failure<int>("Too small"))
            .MapError(err => new Error($"Validation failed: {err.Message}"));

        // Assert (Then)
        final.ShouldBe().Failure().AndMessage("Validation failed: Too small");
    }

    #endregion

    #region MapError - Different error types

    [Fact]
    public void MapError_CanConvertToValidationError()
    {
        // Arrange (Given)
        var error = new Error("Invalid input");
        var result = Result.Failure<int>(error);

        // Act (When)
        var mapped = result.MapError(err => new ValidationError(new[] { err.Message }));

        // Assert (Then)
        mapped.ShouldBe().Failure();
        Assert.False(mapped.TryGet(out var value, out var mappedError));
        Assert.IsType<ValidationError>(mappedError);
    }

    [Fact]
    public void MapError_CanAddMetadataToError()
    {
        // Arrange (Given)
        var error = new Error("Original error");
        var result = Result.Failure<int>(error);

        // Act (When)
        var mapped = result.MapError(err => new Error(
            err.Code,
            err.Message,
            new Dictionary<string, object?> { ["timestamp"] = DateTime.UtcNow }));

        // Assert (Then)
        mapped.ShouldBe().Failure();
        Assert.False(mapped.TryGet(out var value, out var mappedError));
        Assert.NotNull(mappedError);
        Assert.Contains("timestamp", mappedError.Metadata.Keys);
    }

    #endregion
}
