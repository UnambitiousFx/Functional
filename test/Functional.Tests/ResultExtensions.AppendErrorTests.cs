using System.Diagnostics;
using System.Reflection;
using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result HasError, HasException, AppendError, and PrependError extension methods.
/// </summary>
public sealed partial class ResultExtensions
{
    [Fact]
    public void DebugView_ErrorDetails_AggregateError_ReturnsCorrectObject()
    {
        // Arrange
        var errors = new[] { new Failure("e1"), new Failure("e2") };
        var result = Result.Failure(errors.AsEnumerable());

        // Act
        var debugView = GetDebugView(result);
        var details = debugView?.GetType()
                                .GetProperty("ErrorDetails")
                               ?.GetValue(debugView);

        // Assert
        Assert.NotNull(details);
        var type = details?.GetType()
                           .GetProperty("Type")
                          ?.GetValue(details) as string;
        var count = (int?)details?.GetType()
                                  .GetProperty("ErrorCount")
                                 ?.GetValue(details);
        var arr = details?.GetType()
                          .GetProperty("Errors")
                         ?.GetValue(details) as Failure[];

        Assert.Equal("AggregateError", type);
        Assert.Equal(2,                count);
        Assert.NotNull(arr);
        Assert.Equal(2,    arr.Length);
        Assert.Equal("e1", arr![0].Message);
        Assert.Equal("e2", arr[1].Message);
    }

    [Fact]
    public void DebugView_ErrorDetails_ExceptionalError_ReturnsCorrectObject()
    {
        // Arrange
        var ex     = new Exception("boom");
        var result = Result.Failure(ex);

        // Act
        var debugView = GetDebugView(result);
        var details = debugView?.GetType()
                                .GetProperty("ErrorDetails")
                               ?.GetValue(debugView);

        // Assert
        Assert.NotNull(details);
        var type = details?.GetType()
                           .GetProperty("Type")
                          ?.GetValue(details) as string;
        var exception = details?.GetType()
                                .GetProperty("Exception")
                               ?.GetValue(details) as Exception;
        var exceptionType = details?.GetType()
                                    .GetProperty("ExceptionType")
                                   ?.GetValue(details) as string;

        Assert.Equal("ExceptionalError", type);
        Assert.Equal(ex,                 exception);
        Assert.Equal(ex.GetType()
                       .Name, exceptionType);
    }

    [Fact]
    public void DebugView_ErrorDetails_DefaultError_ReturnsCorrectObject()
    {
        // Arrange
        var err    = new Failure("VALIDATION_001", "msg");
        var result = Result.Failure(err);

        // Act
        var debugView = GetDebugView(result);
        var details = debugView?.GetType()
                                .GetProperty("ErrorDetails")
                               ?.GetValue(debugView);

        // Assert
        Assert.NotNull(details);
        var type = details?.GetType()
                           .GetProperty("Type")
                          ?.GetValue(details) as string;
        var code = details?.GetType()
                           .GetProperty("Code")
                          ?.GetValue(details) as string;
        var message = details?.GetType()
                              .GetProperty("Message")
                             ?.GetValue(details) as string;

        Assert.Equal("Failure",        type);
        Assert.Equal("VALIDATION_001", code);
        Assert.Equal("msg",            message);
    }

    // --- Generic versions ---

    [Fact]
    public void DebugView_GenericErrorDetails_AggregateError_ReturnsCorrectObject()
    {
        // Arrange
        var errors = new[] { new Failure("e1"), new Failure("e2") };
        var result = Result.Failure<int>(errors.AsEnumerable());

        // Act
        var debugView = GetDebugView(result);
        var details = debugView?.GetType()
                                .GetProperty("ErrorDetails")
                               ?.GetValue(debugView);

        // Assert
        Assert.NotNull(details);
        var type = details?.GetType()
                           .GetProperty("Type")
                          ?.GetValue(details) as string;
        var count = (int?)details?.GetType()
                                  .GetProperty("ErrorCount")
                                 ?.GetValue(details);
        var arr = details?.GetType()
                          .GetProperty("Errors")
                         ?.GetValue(details) as Failure[];

        Assert.Equal("AggregateError", type);
        Assert.Equal(2,                count);
        Assert.NotNull(arr);
        Assert.Equal(2,    arr.Length);
        Assert.Equal("e1", arr![0].Message);
        Assert.Equal("e2", arr[1].Message);
    }

    [Fact]
    public void DebugView_GenericErrorDetails_ExceptionalError_ReturnsCorrectObject()
    {
        // Arrange
        var ex     = new Exception("boom");
        var result = Result.Failure<int>(ex);

        // Act
        var debugView = GetDebugView(result);
        var details = debugView?.GetType()
                                .GetProperty("ErrorDetails")
                               ?.GetValue(debugView);

        // Assert
        Assert.NotNull(details);
        var type = details?.GetType()
                           .GetProperty("Type")
                          ?.GetValue(details) as string;
        var exception = details?.GetType()
                                .GetProperty("Exception")
                               ?.GetValue(details) as Exception;
        var exceptionType = details?.GetType()
                                    .GetProperty("ExceptionType")
                                   ?.GetValue(details) as string;

        Assert.Equal("ExceptionalError", type);
        Assert.Equal(ex,                 exception);
        Assert.Equal(ex.GetType()
                       .Name, exceptionType);
    }

    [Fact]
    public void DebugView_GenericErrorDetails_DefaultError_ReturnsCorrectObject()
    {
        // Arrange
        var err    = new Failure("VALIDATION_001", "msg");
        var result = Result.Failure<int>(err);

        // Act
        var debugView = GetDebugView(result);
        var details = debugView?.GetType()
                                .GetProperty("ErrorDetails")
                               ?.GetValue(debugView);

        // Assert
        Assert.NotNull(details);
        var type = details?.GetType()
                           .GetProperty("Type")
                          ?.GetValue(details) as string;
        var code = details?.GetType()
                           .GetProperty("Code")
                          ?.GetValue(details) as string;
        var message = details?.GetType()
                              .GetProperty("Message")
                             ?.GetValue(details) as string;

        Assert.Equal("Failure",        type);
        Assert.Equal("VALIDATION_001", code);
        Assert.Equal("msg",            message);
    }

    #region Helper Methods

    private static object? GetDebugView<T>(T value)
    {
        var debuggerTypeProxyAttribute = typeof(T).GetCustomAttribute<DebuggerTypeProxyAttribute>();
        if (debuggerTypeProxyAttribute == null) {
            return null;
        }

        var proxyTypeName = debuggerTypeProxyAttribute.ProxyTypeName;
        // Extract just the type name (e.g., "ResultDebugView" or "ResultDebugView`1")
        var parts = proxyTypeName.Split(',')[0]
                                 .Split('.');
        var proxyTypeGenericName = parts.Last()
                                        .Split('<')[0]
                                        .Split('`')[0];

        Type? proxyType;
        if (typeof(T).IsGenericType) {
            // For generic types (e.g., Result<T>), find the generic proxy type with matching arity
            var genericArgs = typeof(T).GetGenericArguments();
            var arity       = genericArgs.Length;

            var proxyTypeGeneric = typeof(T).Assembly.GetTypes()
                                            .FirstOrDefault(t => t.Name == $"{proxyTypeGenericName}`{arity}" &&
                                                                 t.GetGenericArguments()
                                                                  .Length ==
                                                                 arity);

            if (proxyTypeGeneric == null) {
                return null;
            }

            proxyType = proxyTypeGeneric.MakeGenericType(genericArgs);
        }
        else {
            // For non-generic types (e.g., Result)
            proxyType = typeof(T).Assembly.GetTypes()
                                 .FirstOrDefault(t => t.Name == proxyTypeGenericName);
        }

        if (proxyType == null) {
            return null;
        }

        return Activator.CreateInstance(proxyType, value);
    }

    #endregion
}
