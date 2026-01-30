using System.Diagnostics.CodeAnalysis;
using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional;

/// <summary>
///     Represents the base interface for a result that can either be successful or faulted.
/// </summary>
public interface IResult
{
    /// <summary>
    ///     Gets a value indicating whether the result represents a fault or error state.
    /// </summary>
    bool IsFaulted { get; }

    /// <summary>
    ///     Gets a value indicating whether the result represents a successful state.
    /// </summary>
    bool IsSuccess { get; }

    /// <summary>
    ///     Attempts to retrieve the error from a faulted result.
    /// </summary>
    /// <param name="error">When this method returns false, contains the error; otherwise, null.</param>
    /// <returns>False if the result is faulted and contains an error; otherwise, true.</returns>
    bool TryGetError([NotNullWhen(true)] out Failure? error);
}

/// <summary>
///     Represents the base interface for an operation result that could include a value or an error.
/// </summary>
public interface IResult<TValue> : IResult
    where TValue : notnull
{
    /// <summary>
    ///     Attempts to retrieve the value and error from a result.
    /// </summary>
    /// <param name="value">When this method returns true, contains the value of the result; otherwise, null.</param>
    /// <param name="error">When this method returns false, contains the error of the result; otherwise, null.</param>
    /// <returns>True if the result is successful and contains a value; otherwise, false.</returns>
    bool TryGet([NotNullWhen(true)] out TValue? value, [NotNullWhen(false)] out Failure? error);

    /// <summary>
    ///     Attempts to retrieve the value from a successful result.
    /// </summary>
    /// <param name="value">When this method returns true, contains the value of the result; otherwise, null.</param>
    /// <returns>True if the result is successful and contains a value; otherwise, false.</returns>
    bool TryGetValue([NotNullWhen(true)] out TValue? value);
}