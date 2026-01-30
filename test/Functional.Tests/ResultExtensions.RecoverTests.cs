using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result Recover extension methods.
/// </summary>
public sealed partial class ResultExtensions
{
    #region Recover - Use Cases

    [Fact]
    public void Recover_ImplementsRetryWithDefault()
    {
        // Arrange (Given)
        var attempts = 0;
        var operation = () =>
        {
            attempts++;
            return attempts < 3
                ? Result.Failure<int>("Failed")
                : Result.Success(42);
        };

        // Act (When)
        var result1 = operation().Recover(-1);
        var result2 = operation().Recover(-1);
        var result3 = operation().Recover(-1);

        // Assert (Then)
        result1.ShouldBe().Success().And(value => Assert.Equal(-1, value));
        result2.ShouldBe().Success().And(value => Assert.Equal(-1, value));
        result3.ShouldBe().Success().And(value => Assert.Equal(42, value));
    }

    #endregion

    #region Recover with direct value

    [Fact]
    public void Recover_WithSuccess_ReturnsOriginalResult()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var recovered = result.Recover(0);

        // Assert (Then)
        recovered.ShouldBe().Success().And(value => Assert.Equal(42, value));
    }

    [Fact]
    public void Recover_WithFailure_ReturnsFallback()
    {
        // Arrange (Given)
        var error = new Functional.Failures.Failure("Test error");
        var result = Result.Failure<int>(error);

        // Act (When)
        var recovered = result.Recover(99);

        // Assert (Then)
        recovered.ShouldBe().Success().And(value => Assert.Equal(99, value));
    }

    [Fact]
    public void Recover_WithFailure_StringType_ReturnsFallback()
    {
        // Arrange (Given)
        var error = new Functional.Failures.Failure("Database error");
        var result = Result.Failure<string>(error);

        // Act (When)
        var recovered = result.Recover("default value");

        // Assert (Then)
        recovered.ShouldBe().Success().And(value => Assert.Equal("default value", value));
    }

    #endregion

    #region Recover with factory function

    [Fact]
    public void Recover_WithFactory_WithSuccess_DoesNotCallFactory()
    {
        // Arrange (Given)
        var result = Result.Success(42);
        var factoryCalled = false;

        // Act (When)
        var recovered = result.Recover(err =>
        {
            factoryCalled = true;
            return 0;
        });

        // Assert (Then)
        recovered.ShouldBe().Success().And(value => Assert.Equal(42, value));
        Assert.False(factoryCalled);
    }

    [Fact]
    public void Recover_WithFactory_WithFailure_CallsFactory()
    {
        // Arrange (Given)
        var error = new Functional.Failures.Failure("Test error");
        var result = Result.Failure<int>(error);
        var factoryCalled = false;

        // Act (When)
        var recovered = result.Recover(err =>
        {
            factoryCalled = true;
            return 99;
        });

        // Assert (Then)
        recovered.ShouldBe().Success().And(value => Assert.Equal(99, value));
        Assert.True(factoryCalled);
    }

    [Fact]
    public void Recover_WithFactory_ReceivesError()
    {
        // Arrange (Given)
        var error = new Functional.Failures.Failure("ERR_404", "Not found");
        var result = Result.Failure<int>(error);
        var capturedErrorCode = "";

        // Act (When)
        var recovered = result.Recover(err =>
        {
            capturedErrorCode = err.Code;
            return -1;
        });

        // Assert (Then)
        recovered.ShouldBe().Success().And(value => Assert.Equal(-1, value));
        Assert.Equal("ERR_404", capturedErrorCode);
    }

    #endregion

    #region Recover - Error-based recovery

    [Fact]
    public void Recover_CanProvideErrorSpecificFallback()
    {
        // Arrange (Given)
        var error = new Functional.Failures.Failure("TIMEOUT", "Connection timeout");
        var result = Result.Failure<int>(error);

        // Act (When)
        var recovered = result.Recover(err =>
            err.Code == "TIMEOUT" ? -1 : 0);

        // Assert (Then)
        recovered.ShouldBe().Success().And(value => Assert.Equal(-1, value));
    }

    [Fact]
    public void Recover_CanUseErrorMessageInFallback()
    {
        // Arrange (Given)
        var error = new Functional.Failures.Failure("Database connection failed");
        var result = Result.Failure<string>(error);

        // Act (When)
        var recovered = result.Recover(err => $"Error occurred: {err.Message}");

        // Assert (Then)
        recovered.ShouldBe().Success().And(value =>
            Assert.Equal("Error occurred: Database connection failed", value));
    }

    #endregion

    #region Recover - Chaining

    [Fact]
    public void Recover_CanChainWithOtherOperations()
    {
        // Arrange (Given)
        var result = Result.Success(5);

        // Act (When)
        var final = result
            .Bind(x => x > 10 ? Result.Success(x) : Result.Failure<int>("Too small"))
            .Recover(100)
            .Map(x => x * 2);

        // Assert (Then)
        final.ShouldBe().Success().And(value => Assert.Equal(200, value));
    }

    [Fact]
    public void Recover_MultipleTimes_UsesFirstRecovery()
    {
        // Arrange (Given)
        var error = new Functional.Failures.Failure("Test error");
        var result = Result.Failure<int>(error);

        // Act (When)
        var recovered = result
            .Recover(100)
            .Recover(200);

        // Assert (Then)
        // First recovery succeeds with 100, second recovery doesn't apply
        recovered.ShouldBe().Success().And(value => Assert.Equal(100, value));
    }

    #endregion

    #region Recover - Complex Types

    [Fact]
    public void Recover_WithComplexType_WorksCorrectly()
    {
        // Arrange (Given)
        var error = new Functional.Failures.Failure("Not found");
        var result = Result.Failure<(string Name, int Age)>(error);
        var fallback = (Name: "Default", Age: 0);

        // Act (When)
        var recovered = result.Recover(fallback);

        // Assert (Then)
        recovered.ShouldBe().Success();
        Assert.True(recovered.TryGet(out var value, out _));
        Assert.Equal("Default", value.Name);
        Assert.Equal(0, value.Age);
    }

    [Fact]
    public void Recover_WithFactory_CanCreateComplexFallback()
    {
        // Arrange (Given)
        var error = new Functional.Failures.Failure("User not found");
        var result = Result.Failure<(string Name, int Age)>(error);

        // Act (When)
        var recovered = result.Recover(err =>
        {
            // Create fallback based on error
            if (err.Message.Contains("not found"))
            {
                return (Name: "Guest", Age: 0);
            }

            return (Name: "Unknown", Age: -1);
        });

        // Assert (Then)
        recovered.ShouldBe().Success();
        Assert.True(recovered.TryGet(out var value, out _));
        Assert.Equal("Guest", value.Name);
        Assert.Equal(0, value.Age);
    }

    #endregion
}
