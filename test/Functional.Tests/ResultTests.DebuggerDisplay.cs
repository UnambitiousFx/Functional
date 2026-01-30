using System.Reflection;
using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.Tests;

public sealed class ResultTests_DebuggerDisplay
{
    #region Result DebuggerDisplay

    [Fact]
    public void DebuggerDisplay_Success_ReturnsCorrectString()
    {
        // Arrange
        var result = Result.Success();

        // Act
        var display = GetDebuggerDisplay(result);

        // Assert
        Assert.Contains("Success", display);
        Assert.Contains("reasons=0", display);
    }

    [Fact]
    public void DebuggerDisplay_Failure_ReturnsCorrectString()
    {
        // Arrange
        var result = Result.Failure("Test error");

        // Act
        var display = GetDebuggerDisplay(result);

        // Assert
        Assert.Contains("Failure", display);
        Assert.Contains("Test error", display);
        Assert.Contains("reasons=1", display);
    }

    [Fact]
    public void DebuggerDisplay_FailureWithErrorCode_IncludesCode()
    {
        // Arrange
        var error = new Failure("VALIDATION_001", "Test error");
        var result = Result.Failure(error);

        // Act
        var display = GetDebuggerDisplay(result);

        // Assert
        Assert.Contains("Failure", display);
        Assert.Contains("code=VALIDATION_001", display);
    }

    [Fact]
    public void DebuggerDisplay_SuccessWithMetadata_IncludesMetadata()
    {
        // Arrange
        var result = Result.Success().WithMetadata("key", "value");

        // Act
        var display = GetDebuggerDisplay(result);

        // Assert
        Assert.Contains("Success", display);
        Assert.Contains("meta=", display);
    }

    [Fact]
    public void DebuggerDisplay_FailureWithAggregateError_ShowsMultipleReasons()
    {
        // Arrange
        var errors = new Failure[]
        {
            new Failure("Error 1"),
            new Failure("Error 2"),
            new Failure("Error 3")
        };
        var result = Result.Failure(errors.AsEnumerable());

        // Act
        var display = GetDebuggerDisplay(result);

        // Assert
        Assert.Contains("Failure", display);
        Assert.Contains("reasons=3", display);
    }

    #endregion

    #region Result<T> DebuggerDisplay

    [Fact]
    public void DebuggerDisplay_GenericSuccess_ReturnsCorrectString()
    {
        // Arrange
        var result = Result.Success(42);

        // Act
        var display = GetDebuggerDisplay(result);

        // Assert
        Assert.Contains("Success", display);
        Assert.Contains("reasons=0", display);
    }

    [Fact]
    public void DebuggerDisplay_GenericFailure_ReturnsCorrectString()
    {
        // Arrange
        var result = Result.Failure<int>("Test error");

        // Act
        var display = GetDebuggerDisplay(result);

        // Assert
        Assert.Contains("Failure", display);
        Assert.Contains("Test error", display);
        Assert.Contains("reasons=1", display);
    }

    [Fact]
    public void DebuggerDisplay_GenericFailureWithErrorCode_IncludesCode()
    {
        // Arrange
        var error = new Failure("VALIDATION_001", "Test error");
        var result = Result.Failure<string>(error);

        // Act
        var display = GetDebuggerDisplay(result);

        // Assert
        Assert.Contains("Failure", display);
        Assert.Contains("code=VALIDATION_001", display);
    }

    [Fact]
    public void DebuggerDisplay_GenericSuccessWithMetadata_IncludesMetadata()
    {
        // Arrange
        var result = Result.Success(42).WithMetadata("key", "value");

        // Act
        var display = GetDebuggerDisplay(result);

        // Assert
        Assert.Contains("Success", display);
        Assert.Contains("meta=", display);
    }

    #endregion

    #region DebugView Tests

    [Fact]
    public void DebugView_Success_ReturnsCorrectProperties()
    {
        // Arrange
        var result = Result.Success();

        // Act
        var debugView = GetDebugView(result);
        var isSuccess = (bool?)debugView?.GetType().GetProperty("IsSuccess")?.GetValue(debugView);
        var isFaulted = (bool?)debugView?.GetType().GetProperty("IsFaulted")?.GetValue(debugView);

        // Assert
        Assert.True(isSuccess);
        Assert.False(isFaulted);
    }

    [Fact]
    public void DebugView_Success_ErrorPropertyIsNull()
    {
        // Arrange
        var result = Result.Success();

        // Act
        var debugView = GetDebugView(result);
        var errorProp = debugView?.GetType().GetProperty("Error")?.GetValue(debugView) as FailureBase;

        // Assert
        Assert.Null(errorProp);
    }

    [Fact]
    public void DebugView_Failure_ReturnsCorrectProperties()
    {
        // Arrange
        var error = new Failure("Test error");
        var result = Result.Failure(error);

        // Act
        var debugView = GetDebugView(result);
        var isSuccess = (bool?)debugView?.GetType().GetProperty("IsSuccess")?.GetValue(debugView);
        var isFaulted = (bool?)debugView?.GetType().GetProperty("IsFaulted")?.GetValue(debugView);
        var errorProp = debugView?.GetType().GetProperty("Error")?.GetValue(debugView) as FailureBase;

        // Assert
        Assert.False(isSuccess);
        Assert.True(isFaulted);
        Assert.NotNull(errorProp);
        Assert.Equal("Test error", errorProp.Message);
    }

    [Fact]
    public void DebugView_GenericSuccess_ReturnsCorrectProperties()
    {
        // Arrange
        var result = Result.Success(42);

        // Act
        var debugView = GetDebugView(result);
        var isSuccess = (bool?)debugView?.GetType().GetProperty("IsSuccess")?.GetValue(debugView);
        var value = debugView?.GetType().GetProperty("Value")?.GetValue(debugView);

        // Assert
        Assert.True(isSuccess);
        Assert.Equal(42, value);
    }

    [Fact]
    public void DebugView_GenericSuccess_ErrorPropertyIsNull()
    {
        // Arrange
        var result = Result.Success(42);

        // Act
        var debugView = GetDebugView(result);
        var errorProp = debugView?.GetType().GetProperty("Error")?.GetValue(debugView) as FailureBase;

        // Assert
        Assert.Null(errorProp);
    }

    [Fact]
    public void DebugView_GenericFailure_ReturnsCorrectProperties()
    {
        // Arrange
        var error = new Failure("Test error");
        var result = Result.Failure<int>(error);

        // Act
        var debugView = GetDebugView(result);
        var isSuccess = (bool?)debugView?.GetType().GetProperty("IsSuccess")?.GetValue(debugView);
        var errorProp = debugView?.GetType().GetProperty("Error")?.GetValue(debugView) as FailureBase;

        // Assert
        Assert.False(isSuccess);
        Assert.NotNull(errorProp);
        Assert.Equal("Test error", errorProp.Message);
    }

    #endregion

    #region Helper Methods

    private static string? GetDebuggerDisplay<T>(T value)
    {
        var property = typeof(T).GetProperty("DebuggerDisplay", BindingFlags.Instance | BindingFlags.NonPublic);
        return property?.GetValue(value) as string;
    }

    private static object? GetDebugView<T>(T value)
    {
        var debuggerTypeProxyAttribute = typeof(T).GetCustomAttribute<System.Diagnostics.DebuggerTypeProxyAttribute>();
        if (debuggerTypeProxyAttribute == null)
        {
            return null;
        }

        var proxyTypeName = debuggerTypeProxyAttribute.ProxyTypeName;
        // Extract just the type name (e.g., "ResultDebugView" or "ResultDebugView`1")
        var parts = proxyTypeName.Split(',')[0].Split('.');
        var proxyTypeGenericName = parts.Last().Split('<')[0].Split('`')[0];

        Type? proxyType;
        if (typeof(T).IsGenericType)
        {
            // For Result<T>, find the generic proxy type
            var genericArgs = typeof(T).GetGenericArguments();
            var arity = genericArgs.Length;

            // Look for the proxy type with the correct arity
            var proxyTypeGeneric = typeof(T).Assembly.GetTypes()
                .FirstOrDefault(t => t.Name == $"{proxyTypeGenericName}`{arity}" &&
                                    t.GetGenericArguments().Length == arity);

            if (proxyTypeGeneric == null)
            {
                return null;
            }

            proxyType = proxyTypeGeneric.MakeGenericType(genericArgs);
        }
        else
        {
            // For non-generic Result
            proxyType = typeof(T).Assembly.GetTypes()
                .FirstOrDefault(t => t.Name == proxyTypeGenericName);
        }

        if (proxyType == null)
        {
            return null;
        }

        return Activator.CreateInstance(proxyType, value);
    }

    #endregion
}
