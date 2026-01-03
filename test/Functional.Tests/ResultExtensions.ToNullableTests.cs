using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result ToNullable extension methods.
/// </summary>
public sealed partial class ResultExtensions
{
    #region ToNullable - Value Types

    [Fact]
    public void ToNullable_WithSuccess_ReturnsValue()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var nullable = result.ToNullable();

        // Assert (Then)
        Assert.Equal(42, nullable);
    }

    [Fact]
    public void ToNullable_WithFailure_ReturnsDefault()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = Result.Failure<int>(error);

        // Act (When)
        var nullable = result.ToNullable();

        // Assert (Then)
        // For value types, default is 0
        Assert.Equal(0, nullable);
    }

    #endregion

    #region ToNullable - Reference Types

    [Fact]
    public void ToNullable_WithSuccess_StringType_ReturnsValue()
    {
        // Arrange (Given)
        var result = Result.Success("hello");

        // Act (When)
        var nullable = result.ToNullable();

        // Assert (Then)
        Assert.NotNull(nullable);
        Assert.Equal("hello", nullable);
    }

    [Fact]
    public void ToNullable_WithFailure_StringType_ReturnsNull()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = Result.Failure<string>(error);

        // Act (When)
        var nullable = result.ToNullable();

        // Assert (Then)
        Assert.Null(nullable);
    }

    #endregion

    #region ToNullable - Integration with null-coalescing

    [Fact]
    public void ToNullable_CanUseWithNullCoalescing()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        int? nullable = result.ToNullable();
        var value = nullable ?? 0;

        // Assert (Then)
        Assert.Equal(42, value);
    }

    [Fact]
    public void ToNullable_WithFailure_ReturnsDefaultValue()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = Result.Failure<int>(error);

        // Act (When)
        var nullable = result.ToNullable();

        // Assert (Then)
        // For value types, default is 0
        Assert.Equal(0, nullable);
    }

    [Fact]
    public void ToNullable_CanChainWithNullConditional()
    {
        // Arrange (Given)
        var result = Result.Success("hello");

        // Act (When)
        var length = result.ToNullable()?.Length;

        // Assert (Then)
        Assert.NotNull(length);
        Assert.Equal(5, length);
    }

    [Fact]
    public void ToNullable_WithFailure_NullConditionalReturnsNull()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = Result.Failure<string>(error);

        // Act (When)
        var length = result.ToNullable()?.Length;

        // Assert (Then)
        Assert.Null(length);
    }

    #endregion

    #region ToNullable - Complex Types

    [Fact]
    public void ToNullable_WithComplexType_ReturnsValue()
    {
        // Arrange (Given)
        var person = new Person { Name = "John", Age = 30 };
        var result = Result.Success(person);

        // Act (When)
        var nullable = result.ToNullable();

        // Assert (Then)
        Assert.NotNull(nullable);
        Assert.Equal("John", nullable.Name);
        Assert.Equal(30, nullable.Age);
    }

    [Fact]
    public void ToNullable_WithComplexType_Failure_ReturnsNull()
    {
        // Arrange (Given)
        var error = new Error("Not found");
        var result = Result.Failure<Person>(error);

        // Act (When)
        var nullable = result.ToNullable();

        // Assert (Then)
        Assert.Null(nullable);
    }

    [Fact]
    public void ToNullable_CanAccessPropertiesWithNullConditional()
    {
        // Arrange (Given)
        var person = new Person { Name = "John", Age = 30 };
        var result = Result.Success(person);

        // Act (When)
        var name = result.ToNullable()?.Name;

        // Assert (Then)
        Assert.Equal("John", name);
    }

    #endregion

    #region ToNullable - Use Cases

    [Fact]
    public void ToNullable_UsefulForOptionalOperations()
    {
        // Arrange (Given)
        var result = Result.Success(new List<int> { 1, 2, 3 });

        // Act (When)
        var count = result.ToNullable()?.Count;

        // Assert (Then)
        Assert.Equal(3, count);
    }

    [Fact]
    public void ToNullable_WithFailure_GracefullyHandlesOperations()
    {
        // Arrange (Given)
        var error = new Error("Empty list");
        var result = Result.Failure<List<int>>(error);

        // Act (When)
        var count = result.ToNullable()?.Count;

        // Assert (Then)
        Assert.Null(count);
    }

    [Fact]
    public void ToNullable_CanCombineWithLINQ()
    {
        // Arrange (Given)
        var result = Result.Success(new[] { 1, 2, 3, 4, 5 });

        // Act (When)
        var evenCount = result.ToNullable()?.Count(x => x % 2 == 0);

        // Assert (Then)
        Assert.Equal(2, evenCount);
    }

    #endregion
}
