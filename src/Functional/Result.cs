using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional;

/// <summary>
///     Represents the result of an operation that can either succeed or fail with an error.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
[DebuggerTypeProxy(typeof(ResultDebugView))]
public readonly record struct Result : IResult
{
    private readonly Error? _error;
    private readonly Metadata? _metadata;

    private Result(bool isSuccess, Error? error, Metadata? metadata)
    {
        IsSuccess = isSuccess;
        _error = error;
        _metadata = metadata;
    }

    private string DebuggerDisplay => BuildDebuggerDisplay();

    /// <summary>
    ///     Gets the metadata associated with this result.
    /// </summary>
    public IReadOnlyMetadata Metadata => (IReadOnlyMetadata?)_metadata ?? Functional.Metadata.Empty;

    /// <summary>
    ///     Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFaulted => !IsSuccess;

    /// <summary>
    ///     Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    ///     Attempts to extract the error if the operation failed.
    /// </summary>
    /// <param name="error">The error if the operation failed, otherwise null.</param>
    /// <returns>True if the operation succeeded, false otherwise.</returns>
    public bool TryGet([NotNullWhen(false)] out Error? error)
    {
        error = _error;
        return IsSuccess;
    }

    private string BuildDebuggerDisplay()
    {
        var type = IsSuccess ? "Success" : "Failure";
        var reasons = 0;
        var message = string.Empty;
        var code = string.Empty;

        if (TryGet(out var error))
        {
            reasons = 0;
        }
        else
        {
            message = $"({error?.Message ?? "Unknown error"})";
            reasons = error is IAggregateError aggregate ? aggregate.Errors.Count() : 1;
            if (error?.Code is not null && error.Code != ErrorCodes.Error && error.Code != ErrorCodes.Exception)
            {
                code = $" code={error.Code}";
            }
        }

        var metaPart = Metadata.Count == 0
            ? string.Empty
            : " meta=" + Metadata.ToString(2);

        return $"{type}{message}{code} reasons={reasons}{metaPart}";
    }

    /// <summary>
    ///     Pattern matches the result, executing the appropriate action.
    /// </summary>
    /// <param name="success">Action to execute if the result is successful.</param>
    /// <param name="failure">Action to execute if the result is a failure.</param>
    public void Match(Action success, Action<Error> failure)
    {
        if (IsSuccess)
        {
            success();
        }
        else
        {
            failure(_error!);
        }
    }

    /// <summary>
    ///     Pattern matches the result, returning a value from the appropriate function.
    /// </summary>
    /// <typeparam name="TOut">The type of value to return.</typeparam>
    /// <param name="success">Function to invoke if the result is successful.</param>
    /// <param name="failure">Function to invoke if the result is a failure.</param>
    /// <returns>The result of invoking the appropriate function.</returns>
    public TOut Match<TOut>(Func<TOut> success, Func<Error, TOut> failure) => IsSuccess ? success() : failure(_error!);

    /// <summary>
    ///     Executes the action if the result is successful.
    /// </summary>
    /// <param name="action">Action to execute.</param>
    public void IfSuccess(Action action)
    {
        if (IsSuccess)
        {
            action();
        }
    }

    /// <summary>
    ///     Executes the action if the result is a failure.
    /// </summary>
    /// <param name="action">Action to execute with the error.</param>
    public void IfFailure(Action<Error> action)
    {
        if (!IsSuccess)
        {
            action(_error!);
        }
    }

    /// <summary>
    ///     Deconstructs the result into its error component.
    /// </summary>
    /// <param name="error">The error if the operation failed, otherwise null.</param>
    public void Deconstruct(out Error? error) => error = _error;

    /// <summary>
    ///     Creates a new result with the specified metadata key-value pair added.
    /// </summary>
    /// <param name="key">The metadata key.</param>
    /// <param name="value">The metadata value.</param>
    /// <returns>A new result with the added metadata.</returns>
    public Result WithMetadata(string key, object? value)
    {
        var newMetadata = new Metadata(Metadata);
        newMetadata[key] = value;
        return new Result(IsSuccess, _error, newMetadata);
    }

    /// <summary>
    ///     Creates a new result with the specified metadata merged.
    /// </summary>
    /// <param name="metadata">The metadata to merge.</param>
    /// <returns>A new result with the merged metadata.</returns>
    public Result WithMetadata(IReadOnlyMetadata metadata)
    {
        var newMetadata = new Metadata(Metadata);
        foreach (var kv in metadata)
        {
            newMetadata[kv.Key] = kv.Value;
        }

        return new Result(IsSuccess, _error, newMetadata);
    }

    /// <summary>
    ///     Creates a new result with the specified metadata merged.
    /// </summary>
    /// <param name="metadata">The metadata key-value pairs to merge.</param>
    /// <returns>A new result with the merged metadata.</returns>
    public Result WithMetadata(IEnumerable<KeyValuePair<string, object?>> metadata)
    {
        var newMetadata = new Metadata(Metadata);
        foreach (var kv in metadata)
        {
            newMetadata[kv.Key] = kv.Value;
        }

        return new Result(IsSuccess, _error, newMetadata);
    }

    /// <summary>
    ///     Creates a new result with the specified metadata items added.
    /// </summary>
    /// <param name="items">The metadata items to add.</param>
    /// <returns>A new result with the added metadata.</returns>
    public Result WithMetadata(params (string Key, object? Value)[] items)
    {
        var newMetadata = new Metadata(Metadata);
        foreach (var (key, value) in items)
        {
            newMetadata[key] = value;
        }

        return new Result(IsSuccess, _error, newMetadata);
    }

    /// <summary>
    ///     Creates a new result with metadata configured using a builder action.
    /// </summary>
    /// <param name="configure">Action to configure the metadata builder.</param>
    /// <returns>A new result with the configured metadata.</returns>
    public Result WithMetadata(Action<MetadataBuilder> configure)
    {
        var builder = new MetadataBuilder(Metadata);
        configure(builder);
        return new Result(IsSuccess, _error, builder.Build());
    }

    /// <summary>
    ///     Returns a string representation of the result.
    /// </summary>
    /// <returns>A string indicating success or failure with error details.</returns>
    public override string ToString()
    {
        var metaPart = Metadata.Count == 0
            ? string.Empty
            : " meta=" + Metadata.ToString(2);

        return IsSuccess
            ? $"Success{metaPart}"
            : $"Failure error={_error}{metaPart}";
    }

    #region Static Success

    /// <summary>
    ///     Creates a successful result.
    /// </summary>
    /// <returns>A successful result.</returns>
    public static Result Success() => new(true, null, null);

    /// <summary>
    ///     Creates a successful result with a value.
    /// </summary>
    /// <typeparam name="TValue1">The type of the value.</typeparam>
    /// <param name="value1">The success value.</param>
    /// <returns>A successful result containing the value.</returns>
    public static Result<TValue1> Success<TValue1>(TValue1 value1) where TValue1 : notnull =>
        new(true, value1, null, null);

    #endregion

    #region Static Failure

    /// <summary>
    ///     Creates a failed result from an exception.
    /// </summary>
    /// <param name="error">The exception that caused the failure.</param>
    /// <returns>A failed result containing the exception.</returns>
    public static Result Failure(Exception error) => new(false, new ExceptionalError(error), null);

    /// <summary>
    ///     Creates a failed result from an error.
    /// </summary>
    /// <param name="error">The error that caused the failure.</param>
    /// <returns>A failed result containing the error.</returns>
    public static Result Failure(Error error) => new(false, error, null);

    /// <summary>
    ///     Creates a failed result from an error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <returns>A failed result containing the error message.</returns>
    public static Result Failure(string message) => new(false, new ExceptionalError(new Exception(message)), null);

    /// <summary>
    ///     Creates a failed result from multiple errors.
    /// </summary>
    /// <param name="errors">The collection of errors.</param>
    /// <returns>A failed result containing the aggregated errors.</returns>
    public static Result Failure(params IEnumerable<Error> errors) => new(false, new AggregateError(errors), null);

    /// <summary>
    ///     Creates a failed result with a typed value from an exception.
    /// </summary>
    /// <typeparam name="TValue1">The type of the value.</typeparam>
    /// <param name="error">The exception that caused the failure.</param>
    /// <returns>A failed result containing the exception.</returns>
    public static Result<TValue1> Failure<TValue1>(Exception error) where TValue1 : notnull =>
        new(false, default, new ExceptionalError(error), null);

    /// <summary>
    ///     Creates a failed result with a typed value from an error.
    /// </summary>
    /// <typeparam name="TValue1">The type of the value.</typeparam>
    /// <param name="error">The error that caused the failure.</param>
    /// <returns>A failed result containing the error.</returns>
    public static Result<TValue1> Failure<TValue1>(Error error) where TValue1 : notnull =>
        new(false, default, error, null);

    /// <summary>
    ///     Creates a failed result with a typed value from an error message.
    /// </summary>
    /// <typeparam name="TValue1">The type of the value.</typeparam>
    /// <param name="message">The error message.</param>
    /// <returns>A failed result containing the error message.</returns>
    public static Result<TValue1> Failure<TValue1>(string message) where TValue1 : notnull =>
        new(false, default, new ExceptionalError(new Exception(message)), null);

    /// <summary>
    ///     Creates a failed result with a typed value from multiple errors.
    /// </summary>
    /// <typeparam name="TValue1">The type of the value.</typeparam>
    /// <param name="errors">The collection of errors.</param>
    /// <returns>A failed result containing the aggregated errors.</returns>
    public static Result<TValue1> Failure<TValue1>(params IEnumerable<Error> errors) where TValue1 : notnull =>
        new(false, default, new AggregateError(errors), null);

    #endregion
}

/// <summary>
///     Represents the result of an operation that can succeed with a value or fail with an error.
/// </summary>
/// <typeparam name="TValue1">The type of the success value.</typeparam>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
[DebuggerTypeProxy(typeof(ResultDebugView<>))]
public readonly record struct Result<TValue1> : IResult
    where TValue1 : notnull
{
    private readonly Error? _error;
    private readonly Metadata? _metadata;
    private readonly TValue1? _value;

    internal Result(bool isSuccess, TValue1? value, Error? error, Metadata? metadata)
    {
        IsSuccess = isSuccess;
        _value = value;
        _error = error;
        _metadata = metadata;
    }

    private string DebuggerDisplay => BuildDebuggerDisplay();

    /// <summary>
    ///     Gets the metadata associated with this result.
    /// </summary>
    public IReadOnlyMetadata Metadata => (IReadOnlyMetadata?)_metadata ?? Functional.Metadata.Empty;

    /// <summary>
    ///     Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFaulted => !IsSuccess;

    /// <summary>
    ///     Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    ///     Attempts to extract the error if the operation failed.
    /// </summary>
    /// <param name="error">The error if the operation failed, otherwise null.</param>
    /// <returns>True if the operation succeeded, false otherwise.</returns>
    public bool TryGet([NotNullWhen(false)] out Error? error)
    {
        error = _error;
        return IsSuccess;
    }

    private string BuildDebuggerDisplay()
    {
        var type = IsSuccess ? "Success" : "Failure";
        var reasons = 0;
        var message = string.Empty;
        var code = string.Empty;

        if (TryGet(out Error? error))
        {
            reasons = 0;
        }
        else
        {
            message = $"({error?.Message ?? "Unknown error"})";
            reasons = error is IAggregateError aggregate ? aggregate.Errors.Count() : 1;
            if (error?.Code is not null && error.Code != ErrorCodes.Error && error.Code != ErrorCodes.Exception)
            {
                code = $" code={error.Code}";
            }
        }

        var metaPart = Metadata.Count == 0
            ? string.Empty
            : " meta=" + Metadata.ToString(2);

        return $"{type}{message}{code} reasons={reasons}{metaPart}";
    }

    /// <summary>
    ///     Pattern matches the result, executing the appropriate action.
    /// </summary>
    /// <param name="success">Action to execute if the result is successful.</param>
    /// <param name="failure">Action to execute if the result is a failure.</param>
    public void Match(Action success, Action<Error> failure)
    {
        if (IsSuccess)
        {
            success();
        }
        else
        {
            failure(_error!);
        }
    }

    /// <summary>
    ///     Pattern matches the result, returning a value from the appropriate function.
    /// </summary>
    /// <typeparam name="TOut">The type of value to return.</typeparam>
    /// <param name="success">Function to invoke if the result is successful.</param>
    /// <param name="failure">Function to invoke if the result is a failure.</param>
    /// <returns>The result of invoking the appropriate function.</returns>
    public TOut Match<TOut>(Func<TOut> success, Func<Error, TOut> failure) => IsSuccess ? success() : failure(_error!);

    /// <summary>
    ///     Executes the action if the result is successful.
    /// </summary>
    /// <param name="action">Action to execute.</param>
    public void IfSuccess(Action action)
    {
        if (IsSuccess)
        {
            action();
        }
    }

    /// <summary>
    ///     Executes the action if the result is a failure.
    /// </summary>
    /// <param name="action">Action to execute with the error.</param>
    public void IfFailure(Action<Error> action)
    {
        if (!IsSuccess)
        {
            action(_error!);
        }
    }

    /// <summary>
    ///     Pattern matches the result, executing the appropriate action with the success value.
    /// </summary>
    /// <param name="success">Action to execute with the success value if the result is successful.</param>
    /// <param name="failure">Action to execute if the result is a failure.</param>
    public void Match(Action<TValue1> success, Action<Error> failure)
    {
        if (IsSuccess)
        {
            success(_value!);
        }
        else
        {
            failure(_error!);
        }
    }

    /// <summary>
    ///     Pattern matches the result, returning a value from the appropriate function with the success value.
    /// </summary>
    /// <typeparam name="TOut">The type of value to return.</typeparam>
    /// <param name="success">Function to invoke with the success value if the result is successful.</param>
    /// <param name="failure">Function to invoke if the result is a failure.</param>
    /// <returns>The result of invoking the appropriate function.</returns>
    public TOut Match<TOut>(Func<TValue1, TOut> success, Func<Error, TOut> failure) =>
        IsSuccess ? success(_value!) : failure(_error!);

    /// <summary>
    ///     Executes the action with the success value if the result is successful.
    /// </summary>
    /// <param name="action">Action to execute with the success value.</param>
    public void IfSuccess(Action<TValue1> action)
    {
        if (IsSuccess)
        {
            action(_value!);
        }
    }

    /// <summary>
    ///     Attempts to extract the success value and error.
    /// </summary>
    /// <param name="value1">The success value if successful, otherwise null.</param>
    /// <param name="error">The error if the operation failed, otherwise null.</param>
    /// <returns>True if the operation succeeded, false otherwise.</returns>
    public bool TryGet([NotNullWhen(true)] out TValue1? value1,
        [NotNullWhen(false)] out Error? error)
    {
        value1 = _value;
        error = _error;
        return IsSuccess;
    }

    /// <summary>
    ///     Attempts to extract the success value.
    /// </summary>
    /// <param name="value1">The success value if successful, otherwise null.</param>
    /// <returns>True if the operation succeeded, false otherwise.</returns>
    public bool TryGet([NotNullWhen(true)] out TValue1? value1)
    {
        value1 = _value;
        return IsSuccess;
    }

    /// <summary>
    ///     Deconstructs the result into its value and error components.
    /// </summary>
    /// <param name="value">The success value if successful, otherwise null.</param>
    /// <param name="error">The error if the operation failed, otherwise null.</param>
    public void Deconstruct(out TValue1? value, out Error? error)
    {
        value = _value;
        error = _error;
    }

    /// <summary>
    ///     Creates a new result with the specified metadata key-value pair added.
    /// </summary>
    /// <param name="key">The metadata key.</param>
    /// <param name="value">The metadata value.</param>
    /// <returns>A new result with the added metadata.</returns>
    public Result<TValue1> WithMetadata(string key, object? value)
    {
        var newMetadata = new Metadata(Metadata);
        newMetadata[key] = value;
        return new Result<TValue1>(IsSuccess, _value, _error, newMetadata);
    }

    /// <summary>
    ///     Creates a new result with the specified metadata merged.
    /// </summary>
    /// <param name="metadata">The metadata to merge.</param>
    /// <returns>A new result with the merged metadata.</returns>
    public Result<TValue1> WithMetadata(IReadOnlyMetadata metadata)
    {
        var newMetadata = new Metadata(Metadata);
        foreach (var kv in metadata)
        {
            newMetadata[kv.Key] = kv.Value;
        }

        return new Result<TValue1>(IsSuccess, _value, _error, newMetadata);
    }

    /// <summary>
    ///     Creates a new result with the specified metadata merged.
    /// </summary>
    /// <param name="metadata">The metadata key-value pairs to merge.</param>
    /// <returns>A new result with the merged metadata.</returns>
    public Result<TValue1> WithMetadata(IEnumerable<KeyValuePair<string, object?>> metadata)
    {
        var newMetadata = new Metadata(Metadata);
        foreach (var kv in metadata)
        {
            newMetadata[kv.Key] = kv.Value;
        }

        return new Result<TValue1>(IsSuccess, _value, _error, newMetadata);
    }

    /// <summary>
    ///     Creates a new result with the specified metadata items added.
    /// </summary>
    /// <param name="items">The metadata items to add.</param>
    /// <returns>A new result with the added metadata.</returns>
    public Result<TValue1> WithMetadata(params (string Key, object? Value)[] items)
    {
        var newMetadata = new Metadata(Metadata);
        foreach (var (key, value) in items)
        {
            newMetadata[key] = value;
        }

        return new Result<TValue1>(IsSuccess, _value, _error, newMetadata);
    }

    /// <summary>
    ///     Creates a new result with metadata configured using a builder action.
    /// </summary>
    /// <param name="configure">Action to configure the metadata builder.</param>
    /// <returns>A new result with the configured metadata.</returns>
    public Result<TValue1> WithMetadata(Action<MetadataBuilder> configure)
    {
        var builder = new MetadataBuilder(Metadata);
        configure(builder);
        return new Result<TValue1>(IsSuccess, _value, _error, builder.Build());
    }

    /// <summary>
    ///     Returns a string representation of the result.
    /// </summary>
    /// <returns>A string indicating success with value or failure with error details.</returns>
    public override string ToString()
    {
        var metaPart = Metadata.Count == 0
            ? string.Empty
            : " meta=" + Metadata.ToString(2);

        return IsSuccess
            ? $"Success {_value}{metaPart}"
            : $"Failure error={_error}{metaPart}";
    }
}

internal sealed class ResultDebugView<TValue>
    where TValue : notnull
{
    private readonly Result<TValue> _result;

    public ResultDebugView(Result<TValue> result)
    {
        _result = result;
    }

    public bool IsSuccess => _result.IsSuccess;
    public bool IsFaulted => _result.IsFaulted;

    public TValue? Value => _result.TryGet(out TValue? value) ? value : default;

    public Error? Error => _result.TryGet(out _, out var error) ? null : error;

    public Metadata Metadata => (Metadata)_result.Metadata;

    public object? ErrorDetails
    {
        get
        {
            if (!_result.TryGet(out _, out var error))
            {
                return error switch
                {
                    AggregateError agg => new
                    {
                        Type = "AggregateError", ErrorCount = agg.Errors.Count(), Errors = agg.Errors.ToArray()
                    },
                    ExceptionalError exc => new
                    {
                        Type = "ExceptionalError", exc.Exception, ExceptionType = exc.Exception.GetType().Name
                    },
                    _ => new { Type = error.GetType().Name, error.Code, error.Message }
                };
            }

            return null;
        }
    }
}
