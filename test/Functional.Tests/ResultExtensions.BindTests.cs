using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result Bind extension methods.
/// </summary>
public sealed partial class ResultExtensions
{
    #region Bind Result<T> to Result

    [Fact]
    public void Bind_ResultToResult_WithSuccess_ExecutesBind()
    {
        // Arrange (Given)
        var result = Result.Success(5);

        // Act (When)
        var bound = result.Bind(x => x > 0 ? Result.Success() : Result.Failure("Negative"));

        // Assert (Then)
        bound.ShouldBe().Success();
    }

    [Fact]
    public void Bind_ResultToResult_WithFailure_PropagatesError()
    {
        // Arrange (Given)
        var error = new Failure("Initial error");
        var result = Result.Failure<int>(error);

        // Act (When)
        var bound = result.Bind(x => Result.Success());

        // Assert (Then)
        bound.ShouldBe().Failure().AndMessage("Initial error");
    }

    [Fact]
    public void Bind_ResultToResult_BindReturnsFailure_ReturnsFailure()
    {
        // Arrange (Given)
        var result = Result.Success(-5);

        // Act (When)
        var bound = result.Bind(x => x > 0 ? Result.Success() : Result.Failure("Negative"));

        // Assert (Then)
        bound.ShouldBe().Failure().AndMessage("Negative");
    }

    #endregion

    #region Bind Result<T> to Result<TOut>

    [Fact]
    public void Bind_ResultToResultOfT_WithSuccess_ExecutesBind()
    {
        // Arrange (Given)
        var result = Result.Success(5);

        // Act (When)
        var bound = result.Bind(x => Result.Success(x * 2));

        // Assert (Then)
        bound.ShouldBe().Success().And(value => Assert.Equal(10, value));
    }

    [Fact]
    public void Bind_ResultToResultOfT_WithFailure_PropagatesError()
    {
        // Arrange (Given)
        var error = new Failure("Initial error");
        var result = Result.Failure<int>(error);

        // Act (When)
        var bound = result.Bind(x => Result.Success(x.ToString()));

        // Assert (Then)
        bound.ShouldBe().Failure().AndMessage("Initial error");
    }

    [Fact]
    public void Bind_ResultToResultOfT_BindReturnsFailure_ReturnsFailure()
    {
        // Arrange (Given)
        var result = Result.Success(5);

        // Act (When)
        var bound = result.Bind(x => Result.Failure<string>("Bind error"));

        // Assert (Then)
        bound.ShouldBe().Failure().AndMessage("Bind error");
    }

    #endregion

    #region Bind Result to Result

    [Fact]
    public void Bind_NonGenericToNonGeneric_WithSuccess_ExecutesBind()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var bound = result.Bind(() => Result.Success());

        // Assert (Then)
        bound.ShouldBe().Success();
    }

    [Fact]
    public void Bind_NonGenericToNonGeneric_WithFailure_PropagatesError()
    {
        // Arrange (Given)
        var error = new Failure("Initial error");
        var result = Result.Failure(error);

        // Act (When)
        var bound = result.Bind(() => Result.Success());

        // Assert (Then)
        bound.ShouldBe().Failure().AndMessage("Initial error");
    }

    [Fact]
    public void Bind_NonGenericToNonGeneric_BindReturnsFailure_ReturnsFailure()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var bound = result.Bind(() => Result.Failure("Bind error"));

        // Assert (Then)
        bound.ShouldBe().Failure().AndMessage("Bind error");
    }

    #endregion

    #region Bind Result to Result<T>

    [Fact]
    public void Bind_NonGenericToGeneric_WithSuccess_ExecutesBind()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var bound = result.Bind(() => Result.Success(42));

        // Assert (Then)
        bound.ShouldBe().Success().And(value => Assert.Equal(42, value));
    }

    [Fact]
    public void Bind_NonGenericToGeneric_WithFailure_PropagatesError()
    {
        // Arrange (Given)
        var error = new Failure("Initial error");
        var result = Result.Failure(error);

        // Act (When)
        var bound = result.Bind(() => Result.Success(42));

        // Assert (Then)
        bound.ShouldBe().Failure().AndMessage("Initial error");
    }

    #endregion

    #region Chaining

    [Fact]
    public void Bind_CanChainMultipleBinds()
    {
        // Arrange (Given)
        var result = Result.Success(5);

        // Act (When)
        var bound = result
            .Bind(x => Result.Success(x * 2))
            .Bind(x => Result.Success(x + 10))
            .Bind(x => Result.Success(x.ToString()));

        // Assert (Then)
        bound.ShouldBe().Success().And(value => Assert.Equal("20", value));
    }

    [Fact]
    public void Bind_ChainStopsAtFirstFailure()
    {
        // Arrange (Given)
        var result = Result.Success(5);
        var thirdCalled = false;

        // Act (When)
        var bound = result
            .Bind(x => Result.Success(x * 2))
            .Bind(x => Result.Failure<int>("Second bind failed"))
            .Bind(x =>
            {
                thirdCalled = true;
                return Result.Success(x + 10);
            });

        // Assert (Then)
        bound.ShouldBe().Failure().AndMessage("Second bind failed");
        Assert.False(thirdCalled);
    }

    #endregion
}