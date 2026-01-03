using System.Diagnostics;
using System.Reflection;
using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.Tests.Errors;

/// <summary>
/// Tests for Error debug view types.
/// </summary>
public class ErrorDebugViewTests
{
    #region AggregateErrorDebugView

    [Fact]
    public void AggregateErrorDebugView_Constructor_InitializesError()
    {
        // Arrange (Given)
        var error1 = new Error("Error 1");
        var error2 = new Error("Error 2");
        var aggregateError = new AggregateError(error1, error2);

        // Act (When)
        var debugView = CreateAggregateErrorDebugView(aggregateError);

        // Assert (Then)
        Assert.NotNull(debugView);
    }

    [Fact]
    public void AggregateErrorDebugView_Errors_ReturnsErrorArray()
    {
        // Arrange (Given)
        var error1 = new Error("Error 1");
        var error2 = new Error("Error 2");
        var aggregateError = new AggregateError(error1, error2);
        var debugView = CreateAggregateErrorDebugView(aggregateError);

        // Act (When)
        var errors = GetDebugViewProperty<Error[]>(debugView, "Errors");

        // Assert (Then)
        Assert.NotNull(errors);
        Assert.Equal(2, errors.Length);
        Assert.Equal("Error 1", errors[0].Message);
        Assert.Equal("Error 2", errors[1].Message);
    }

    [Fact]
    public void AggregateErrorDebugView_Code_ReturnsErrorCode()
    {
        // Arrange (Given)
        var error1 = new Error("Error 1");
        var aggregateError = new AggregateError(error1);
        var debugView = CreateAggregateErrorDebugView(aggregateError);

        // Act (When)
        var code = GetDebugViewProperty<string>(debugView, "Code");

        // Assert (Then)
        Assert.Equal(ErrorCodes.AggregateError, code);
    }

    [Fact]
    public void AggregateErrorDebugView_Message_ReturnsErrorMessage()
    {
        // Arrange (Given)
        var error1 = new Error("Error 1");
        var aggregateError = new AggregateError(error1);
        var debugView = CreateAggregateErrorDebugView(aggregateError);

        // Act (When)
        var message = GetDebugViewProperty<string>(debugView, "Message");

        // Assert (Then)
        Assert.Equal("Multiple errors occurred", message);
    }

    [Fact]
    public void AggregateErrorDebugView_Metadata_ReturnsErrorMetadata()
    {
        // Arrange (Given)
        var error1 = new Error("Error 1");
        var aggregateError = new AggregateError(error1);
        var debugView = CreateAggregateErrorDebugView(aggregateError);

        // Act (When)
        var metadata = GetDebugViewProperty<Metadata>(debugView, "Metadata");

        // Assert (Then)
        Assert.NotNull(metadata);
    }

    [Fact]
    public void AggregateErrorDebugView_ErrorCount_ReturnsCorrectCount()
    {
        // Arrange (Given)
        var error1 = new Error("Error 1");
        var error2 = new Error("Error 2");
        var error3 = new Error("Error 3");
        var aggregateError = new AggregateError(error1, error2, error3);
        var debugView = CreateAggregateErrorDebugView(aggregateError);

        // Act (When)
        var errorCount = GetDebugViewProperty<int>(debugView, "ErrorCount");

        // Assert (Then)
        Assert.Equal(3, errorCount);
    }

    [Fact]
    public void AggregateErrorDebugView_Errors_HasDebuggerBrowsableAttribute()
    {
        // Arrange (Given)
        var debugViewType = GetDebugViewType("AggregateErrorDebugView");

        // Act (When)
        var errorsProperty = debugViewType.GetProperty("Errors");
        var attribute = errorsProperty?.GetCustomAttribute<DebuggerBrowsableAttribute>();

        // Assert (Then)
        Assert.NotNull(attribute);
        Assert.Equal(DebuggerBrowsableState.RootHidden, attribute.State);
    }

    #endregion

    #region ExceptionalErrorDebugView

    [Fact]
    public void ExceptionalErrorDebugView_Constructor_InitializesError()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("Test exception");
        var exceptionalError = new ExceptionalError(exception);

        // Act (When)
        var debugView = CreateExceptionalErrorDebugView(exceptionalError);

        // Assert (Then)
        Assert.NotNull(debugView);
    }

    [Fact]
    public void ExceptionalErrorDebugView_Exception_ReturnsWrappedException()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("Test exception");
        var exceptionalError = new ExceptionalError(exception);
        var debugView = CreateExceptionalErrorDebugView(exceptionalError);

        // Act (When)
        var returnedException = GetDebugViewProperty<Exception>(debugView, "Exception");

        // Assert (Then)
        Assert.Same(exception, returnedException);
    }

    [Fact]
    public void ExceptionalErrorDebugView_ExceptionType_ReturnsExceptionTypeName()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("Test exception");
        var exceptionalError = new ExceptionalError(exception);
        var debugView = CreateExceptionalErrorDebugView(exceptionalError);

        // Act (When)
        var exceptionType = GetDebugViewProperty<string>(debugView, "ExceptionType");

        // Assert (Then)
        Assert.Equal("InvalidOperationException", exceptionType);
    }

    [Fact]
    public void ExceptionalErrorDebugView_Message_ReturnsExceptionMessage()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("Test exception");
        var exceptionalError = new ExceptionalError(exception);
        var debugView = CreateExceptionalErrorDebugView(exceptionalError);

        // Act (When)
        var message = GetDebugViewProperty<string>(debugView, "Message");

        // Assert (Then)
        Assert.Equal("Test exception", message);
    }

    [Fact]
    public void ExceptionalErrorDebugView_Code_ReturnsExceptionCode()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("Test exception");
        var exceptionalError = new ExceptionalError(exception);
        var debugView = CreateExceptionalErrorDebugView(exceptionalError);

        // Act (When)
        var code = GetDebugViewProperty<string>(debugView, "Code");

        // Assert (Then)
        Assert.Equal(ErrorCodes.Exception, code);
    }

    [Fact]
    public void ExceptionalErrorDebugView_MessageOverride_ReturnsNullWhenNotProvided()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("Test exception");
        var exceptionalError = new ExceptionalError(exception);
        var debugView = CreateExceptionalErrorDebugView(exceptionalError);

        // Act (When)
        var messageOverride = GetDebugViewProperty<string?>(debugView, "MessageOverride");

        // Assert (Then)
        Assert.Null(messageOverride);
    }

    [Fact]
    public void ExceptionalErrorDebugView_MessageOverride_ReturnsOverrideWhenProvided()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("Test exception");
        var exceptionalError = new ExceptionalError(exception, "Custom message");
        var debugView = CreateExceptionalErrorDebugView(exceptionalError);

        // Act (When)
        var messageOverride = GetDebugViewProperty<string?>(debugView, "MessageOverride");

        // Assert (Then)
        Assert.Equal("Custom message", messageOverride);
    }

    [Fact]
    public void ExceptionalErrorDebugView_Metadata_ReturnsErrorMetadata()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("Test exception");
        var exceptionalError = new ExceptionalError(exception);
        var debugView = CreateExceptionalErrorDebugView(exceptionalError);

        // Act (When)
        var metadata = GetDebugViewProperty<Metadata>(debugView, "Metadata");

        // Assert (Then)
        Assert.NotNull(metadata);
    }

    [Fact]
    public void ExceptionalErrorDebugView_Extra_ReturnsNullWhenNotProvided()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("Test exception");
        var exceptionalError = new ExceptionalError(exception);
        var debugView = CreateExceptionalErrorDebugView(exceptionalError);

        // Act (When)
        var extra = GetDebugViewProperty<IReadOnlyDictionary<string, object?>?>(debugView, "Extra");

        // Assert (Then)
        Assert.Null(extra);
    }

    [Fact]
    public void ExceptionalErrorDebugView_Extra_ReturnsExtraDataWhenProvided()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("Test exception");
        var extraData = new Dictionary<string, object?> { ["key"] = "value" };
        var exceptionalError = new ExceptionalError(exception, extra: extraData);
        var debugView = CreateExceptionalErrorDebugView(exceptionalError);

        // Act (When)
        var extra = GetDebugViewProperty<IReadOnlyDictionary<string, object?>?>(debugView, "Extra");

        // Assert (Then)
        Assert.NotNull(extra);
        Assert.True(extra.ContainsKey("key"));
        Assert.Equal("value", extra["key"]);
    }

    #endregion

    #region Helper Methods

    private static object CreateAggregateErrorDebugView(AggregateError error)
    {
        var debugViewType = GetDebugViewType("AggregateErrorDebugView");
        return Activator.CreateInstance(debugViewType, error)!;
    }

    private static object CreateExceptionalErrorDebugView(ExceptionalError error)
    {
        var debugViewType = GetDebugViewType("ExceptionalErrorDebugView");
        return Activator.CreateInstance(debugViewType, error)!;
    }

    private static Type GetDebugViewType(string typeName)
    {
        var assembly = typeof(Error).Assembly;
        var fullTypeName = $"UnambitiousFx.Functional.Errors.{typeName}";
        return assembly.GetType(fullTypeName, throwOnError: true)!;
    }

    private static T GetDebugViewProperty<T>(object debugView, string propertyName)
    {
        var property = debugView.GetType().GetProperty(propertyName);
        Assert.NotNull(property);
        return (T)property.GetValue(debugView)!;
    }

    #endregion
}
