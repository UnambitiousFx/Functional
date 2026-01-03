using System.Diagnostics.CodeAnalysis;
using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional;

/// <summary>
/// Represents the base interface for a result that can either be successful or faulted.
/// </summary>
public interface IResult
{
    /// <summary>
    /// Gets a value indicating whether the result represents a fault or error state.
    /// </summary>
    bool IsFaulted { get; }

    /// <summary>
    /// Gets a value indicating whether the result represents a successful state.
    /// </summary>
    bool IsSuccess { get; }

    /// <summary>
    /// Attempts to retrieve the error from a faulted result.
    /// </summary>
    /// <param name="error">When this method returns false, contains the error; otherwise, null.</param>
    /// <returns>False if the result is faulted and contains an error; otherwise, true.</returns>
    bool TryGet([NotNullWhen(false)] out Error? error);
}
