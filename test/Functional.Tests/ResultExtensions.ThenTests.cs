using UnambitiousFx.Functional.Errors;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result Then extension methods.
/// </summary>
public sealed partial class ResultExtensions
{
    #region Then vs Bind

    [Fact]
    public void Then_ReturnsSameType_UnlikeBind()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        // Then must return Result<int> (same type)
        var thenResult = result.Then(x => x > 0 ? Result.Success(x) : Result.Failure<int>("Invalid"));

        // Bind can change type to Result<string>
        var bindResult = result.Bind(x => Result.Success(x.ToString()));

        // Assert (Then)
        thenResult.ShouldBe().Success().And(value => Assert.Equal(42, value));
        bindResult.ShouldBe().Success().And(value => Assert.Equal("42", value));
    }

    #endregion

    #region Then - Success Cases

    [Fact]
    public void Then_WithSuccess_ExecutesTransformation()
    {
        // Arrange (Given)
        var result = Result.Success(10);

        // Act (When)
        var transformed = result.Then(x => x > 5 ? Result.Success(x) : Result.Failure<int>("Too small"));

        // Assert (Then)
        transformed.ShouldBe().Success().And(value => Assert.Equal(10, value));
    }

    [Fact]
    public void Then_WithSuccess_TransformationReturnsFailure_ReturnsFailure()
    {
        // Arrange (Given)
        var result = Result.Success(3);

        // Act (When)
        var transformed = result.Then(x => x > 5 ? Result.Success(x) : Result.Failure<int>("Too small"));

        // Assert (Then)
        transformed.ShouldBe().Failure().AndMessage("Too small");
    }

    #endregion

    #region Then - Failure Cases

    [Fact]
    public void Then_WithFailure_DoesNotExecuteTransformation()
    {
        // Arrange (Given)
        var error = new Error("Initial error");
        var result = Result.Failure<int>(error);
        var transformCalled = false;

        // Act (When)
        var transformed = result.Then(x =>
        {
            transformCalled = true;
            return Result.Success(x * 2);
        });

        // Assert (Then)
        transformed.ShouldBe().Failure().AndMessage("Initial error");
        Assert.False(transformCalled);
    }

    [Fact]
    public void Then_WithFailure_PropagatesError()
    {
        // Arrange (Given)
        var error = new Error("Initial error");
        var result = Result.Failure<int>(error);

        // Act (When)
        var transformed = result.Then(x => Result.Success(x * 2));

        // Assert (Then)
        transformed.ShouldBe().Failure().AndMessage("Initial error");
    }

    #endregion

    #region Then - Metadata Handling

    [Fact]
    public void Then_CopiesMetadata_WhenCopyReasonsAndMetadataIsTrue()
    {
        // Arrange (Given)
        var result = Result.Success(10).WithMetadata("key", "value");

        // Act (When)
        var transformed = result.Then(x => Result.Success(x * 2), true);

        // Assert (Then)
        transformed.ShouldBe().Success();
        Assert.Equal("value", transformed.Metadata["key"]);
    }

    [Fact]
    public void Then_DoesNotCopyMetadata_WhenCopyReasonsAndMetadataIsFalse()
    {
        // Arrange (Given)
        var result = Result.Success(10).WithMetadata("key", "value");

        // Act (When)
        var transformed = result.Then(x => Result.Success(x * 2), false);

        // Assert (Then)
        transformed.ShouldBe().Success();
        Assert.Empty(transformed.Metadata);
    }

    #endregion

    #region Then - Chaining

    [Fact]
    public void Then_CanChainMultipleThen()
    {
        // Arrange (Given)
        var result = Result.Success(10);

        // Act (When)
        var transformed = result
            .Then(x => x > 0 ? Result.Success(x) : Result.Failure<int>("Must be positive"))
            .Then(x => x < 100 ? Result.Success(x) : Result.Failure<int>("Must be less than 100"))
            .Then(x => x % 2 == 0 ? Result.Success(x) : Result.Failure<int>("Must be even"));

        // Assert (Then)
        transformed.ShouldBe().Success().And(value => Assert.Equal(10, value));
    }

    [Fact]
    public void Then_ChainStopsAtFirstFailure()
    {
        // Arrange (Given)
        var result = Result.Success(150);
        var thirdCalled = false;

        // Act (When)
        var transformed = result
            .Then(x => x > 0 ? Result.Success(x) : Result.Failure<int>("Must be positive"))
            .Then(x => x < 100 ? Result.Success(x) : Result.Failure<int>("Must be less than 100"))
            .Then(x =>
            {
                thirdCalled = true;
                return Result.Success(x);
            });

        // Assert (Then)
        transformed.ShouldBe().Failure().AndMessage("Must be less than 100");
        Assert.False(thirdCalled);
    }

    #endregion

    #region Then - Validation Use Cases

    [Fact]
    public void Then_CanImplementValidation()
    {
        // Arrange (Given)
        var result = Result.Success("hello");

        // Act (When)
        var validated = result.Then(x =>
            x.Length >= 3 && x.Length <= 10
                ? Result.Success(x)
                : Result.Failure<string>("String must be between 3 and 10 characters"));

        // Assert (Then)
        validated.ShouldBe().Success().And(value => Assert.Equal("hello", value));
    }

    [Fact]
    public void Then_ValidationFailure()
    {
        // Arrange (Given)
        var result = Result.Success("hi");

        // Act (When)
        var validated = result.Then(x =>
            x.Length >= 5
                ? Result.Success(x)
                : Result.Failure<string>($"String '{x}' is too short"));

        // Assert (Then)
        validated.ShouldBe().Failure().AndMessage("String 'hi' is too short");
    }

    #endregion
}
