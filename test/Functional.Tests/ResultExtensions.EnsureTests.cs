using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result Ensure extension methods.
/// </summary>
public sealed partial class ResultExtensions
{
    #region Ensure - Error factory receives value

    [Fact]
    public void Ensure_ErrorFactory_ReceivesValue()
    {
        // Arrange (Given)
        var result = Result.Success(100);

        // Act (When)
        var ensured = result.Ensure(
            x => x < 50,
            x => new Failure($"Value {x} exceeds maximum of 50"));

        // Assert (Then)
        ensured.ShouldBe().Failure().AndMessage("Value 100 exceeds maximum of 50");
    }

    #endregion

    #region Ensure - Basic validation

    [Fact]
    public void Ensure_WithSuccess_PredicateTrue_ReturnsSuccess()
    {
        // Arrange (Given)
        var result = Result.Success(10);

        // Act (When)
        var ensured = result.Ensure(x => x > 0, x => new Failure("Must be positive"));

        // Assert (Then)
        ensured.ShouldBe().Success().And(value => Assert.Equal(10, value));
    }

    [Fact]
    public void Ensure_WithSuccess_PredicateFalse_ReturnsFailure()
    {
        // Arrange (Given)
        var result = Result.Success(-5);

        // Act (When)
        var ensured = result.Ensure(x => x > 0, x => new Failure("Must be positive"));

        // Assert (Then)
        ensured.ShouldBe().Failure().AndMessage("Must be positive");
    }

    [Fact]
    public void Ensure_WithFailure_DoesNotExecutePredicate()
    {
        // Arrange (Given)
        var error = new Failure("Initial error");
        var result = Result.Failure<int>(error);
        var predicateCalled = false;

        // Act (When)
        var ensured = result.Ensure(
            x =>
            {
                predicateCalled = true;
                return true;
            },
            x => new Failure("Validation failed"));

        // Assert (Then)
        ensured.ShouldBe().Failure().AndMessage("Initial error");
        Assert.False(predicateCalled);
    }

    #endregion

    #region Ensure - Chaining multiple validations

    [Fact]
    public void Ensure_CanChainMultipleValidations()
    {
        // Arrange (Given)
        var result = Result.Success(25);

        // Act (When)
        var ensured = result
            .Ensure(x => x > 0, x => new Failure("Must be positive"))
            .Ensure(x => x < 100, x => new Failure("Must be less than 100"))
            .Ensure(x => x % 5 == 0, x => new Failure("Must be divisible by 5"));

        // Assert (Then)
        ensured.ShouldBe().Success().And(value => Assert.Equal(25, value));
    }

    [Fact]
    public void Ensure_ChainedValidations_StopsAtFirstFailure()
    {
        // Arrange (Given)
        var result = Result.Success(150);
        var thirdPredicateCalled = false;

        // Act (When)
        var ensured = result
            .Ensure(x => x > 0, x => new Failure("Must be positive"))
            .Ensure(x => x < 100, x => new Failure("Must be less than 100"))
            .Ensure(x =>
            {
                thirdPredicateCalled = true;
                return x % 5 == 0;
            }, x => new Failure("Must be divisible by 5"));

        // Assert (Then)
        ensured.ShouldBe().Failure().AndMessage("Must be less than 100");
        Assert.False(thirdPredicateCalled);
    }

    #endregion

    #region Ensure - String validation

    [Fact]
    public void Ensure_WithString_ValidatesLength()
    {
        // Arrange (Given)
        var result = Result.Success("hello");

        // Act (When)
        var ensured = result.Ensure(
            x => x.Length >= 3,
            x => new Failure($"String '{x}' is too short"));

        // Assert (Then)
        ensured.ShouldBe().Success().And(value => Assert.Equal("hello", value));
    }

    [Fact]
    public void Ensure_WithString_FailsValidation()
    {
        // Arrange (Given)
        var result = Result.Success("hi");

        // Act (When)
        var ensured = result.Ensure(
            x => x.Length >= 5,
            x => new Failure($"String '{x}' must be at least 5 characters"));

        // Assert (Then)
        ensured.ShouldBe().Failure().AndMessage("String 'hi' must be at least 5 characters");
    }

    #endregion

    #region Ensure - Complex predicates

    [Fact]
    public void Ensure_WithComplexPredicate_ValidatesCorrectly()
    {
        // Arrange (Given)
        var result = Result.Success(new { Name = "John", Age = 30 });

        // Act (When)
        var ensured = result.Ensure(
            x => x.Age >= 18 && !string.IsNullOrWhiteSpace(x.Name),
            x => new Failure("Invalid person data"));

        // Assert (Then)
        ensured.ShouldBe().Success();
    }

    [Fact]
    public void Ensure_WithComplexPredicate_FailsValidation()
    {
        // Arrange (Given)
        var result = Result.Success(new { Name = "John", Age = 15 });

        // Act (When)
        var ensured = result.Ensure(
            x => x.Age >= 18,
            x => new Failure($"{x.Name} must be at least 18 years old"));

        // Assert (Then)
        ensured.ShouldBe().Failure().AndMessage("John must be at least 18 years old");
    }

    #endregion
}
