using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result ValueOr extension methods.
/// </summary>
public sealed partial class ResultExtensions
{
    #region ValueOr with direct value

    [Fact]
    public void ValueOr_WithSuccess_ReturnsValue()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var value = result.ValueOr(0);

        // Assert (Then)
        Assert.Equal(42, value);
    }

    [Fact]
    public void ValueOr_WithFailure_ReturnsFallback()
    {
        // Arrange (Given)
        var error = new Failure("Test error");
        var result = Result.Failure<int>(error);

        // Act (When)
        var value = result.ValueOr(99);

        // Assert (Then)
        Assert.Equal(99, value);
    }

    [Fact]
    public void ValueOr_WithSuccess_StringType_ReturnsValue()
    {
        // Arrange (Given)
        var result = Result.Success("hello");

        // Act (When)
        var value = result.ValueOr("default");

        // Assert (Then)
        Assert.Equal("hello", value);
    }

    [Fact]
    public void ValueOr_WithFailure_StringType_ReturnsFallback()
    {
        // Arrange (Given)
        var error = new Failure("Test error");
        var result = Result.Failure<string>(error);

        // Act (When)
        var value = result.ValueOr("fallback");

        // Assert (Then)
        Assert.Equal("fallback", value);
    }

    #endregion

    #region ValueOr with factory function

    [Fact]
    public void ValueOr_WithFactory_WithSuccess_ReturnsValue()
    {
        // Arrange (Given)
        var result = Result.Success(42);
        var factoryCalled = false;

        // Act (When)
        var value = result.ValueOr(() =>
        {
            factoryCalled = true;
            return 0;
        });

        // Assert (Then)
        Assert.Equal(42, value);
        Assert.False(factoryCalled);
    }

    [Fact]
    public void ValueOr_WithFactory_WithFailure_CallsFactory()
    {
        // Arrange (Given)
        var error = new Failure("Test error");
        var result = Result.Failure<int>(error);
        var factoryCalled = false;

        // Act (When)
        var value = result.ValueOr(() =>
        {
            factoryCalled = true;
            return 99;
        });

        // Assert (Then)
        Assert.Equal(99, value);
        Assert.True(factoryCalled);
    }

    [Fact]
    public void ValueOr_WithFactory_CanUseComplexLogic()
    {
        // Arrange (Given)
        var error = new Failure("Test error");
        var result = Result.Failure<string>(error);

        // Act (When)
        var value = result.ValueOr(() =>
        {
            // Complex logic to compute fallback
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd");
            return $"fallback-{timestamp}";
        });

        // Assert (Then)
        Assert.StartsWith("fallback-", value);
    }

    #endregion

    #region ValueOr with complex types

    [Fact]
    public void ValueOr_WithComplexType_ReturnsCorrectValue()
    {
        // Arrange (Given)
        var person = new { Name = "John", Age = 30 };
        var result = Result.Success(person);
        var fallback = new { Name = "Default", Age = 0 };

        // Act (When)
        var value = result.ValueOr(fallback);

        // Assert (Then)
        Assert.Equal("John", value.Name);
        Assert.Equal(30, value.Age);
    }

    [Fact]
    public void ValueOr_WithComplexType_WithFailure_ReturnsFallback()
    {
        // Arrange (Given)
        var error = new Failure("Test error");
        var result = Result.Failure<(string Name, int Age)>(error);
        var fallback = (Name: "Default", Age: 0);

        // Act (When)
        var value = result.ValueOr(fallback);

        // Assert (Then)
        Assert.Equal("Default", value.Name);
        Assert.Equal(0, value.Age);
    }

    #endregion
}
