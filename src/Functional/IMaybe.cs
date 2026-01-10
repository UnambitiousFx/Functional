using System.Diagnostics.CodeAnalysis;

namespace UnambitiousFx.Functional;

/// <summary>
/// Represents an interface for a container that can optionally hold a single value
/// of type <typeparamref name="TValue"/>. The container can either be in a state where
/// it contains a value (Some) or it does not (None).
/// </summary>
/// <typeparam name="TValue">
/// The type of the value that this container may hold. Must be a non-nullable type.
/// </typeparam>
public interface IMaybe<TValue> where TValue : notnull
{
    /// <summary>
    ///     Gets a value indicating whether this instance contains a value.
    /// </summary>
    bool IsSome { get; }

    /// <summary>
    ///     Gets a value indicating whether this instance is empty (contains no value).
    /// </summary>
    bool IsNone { get; }

    /// <summary>
    ///     Gets the underlying value if present, or the default value if empty.
    /// </summary>
    TValue? Case { get; }

    /// <summary>
    ///     Executes the specified action if this instance is empty (None).
    /// </summary>
    /// <param name="none">The action to execute when no value is present.</param>
    void IfNone(Action none);

    /// <summary>
    ///     Asynchronously executes the specified function if this instance is empty (None).
    /// </summary>
    /// <param name="none">The asynchronous function to execute when no value is present.</param>
    /// <returns>A ValueTask that completes when the operation finishes.</returns>
    ValueTask IfNone(Func<ValueTask> none);

    /// <summary>
    ///     Executes the specified action if this instance contains a value (Some).
    /// </summary>
    /// <param name="some">The action to execute with the contained value.</param>
    void IfSome(Action<TValue> some);

    /// <summary>
    ///     Asynchronously executes the specified function if this instance contains a value (Some).
    /// </summary>
    /// <param name="some">The asynchronous function to execute with the contained value.</param>
    /// <returns>A ValueTask that completes when the operation finishes.</returns>
    ValueTask IfSome(Func<TValue, ValueTask> some);

    /// <summary>
    ///     Attempts to retrieve the contained value.
    /// </summary>
    /// <param name="value">When this method returns, contains the value if present; otherwise, the default value.</param>
    /// <returns>true if this instance contains a value; otherwise, false.</returns>
    bool Some([NotNullWhen(true)] out TValue? value);

    /// <summary>
    ///     Matches the Maybe instance and executes the corresponding function based on whether a value is present.
    /// </summary>
    /// <typeparam name="TOut">The type of the result.</typeparam>
    /// <param name="some">The function to execute if a value is present.</param>
    /// <param name="none">The function to execute if no value is present.</param>
    /// <returns>The result of the executed function.</returns>
    TOut Match<TOut>(Func<TValue, TOut> some, Func<TOut> none);

    /// <summary>
    ///     Matches the Maybe instance and executes the corresponding action based on whether a value is present.
    /// </summary>
    /// <param name="some">The action to execute if a value is present.</param>
    /// <param name="none">The action to execute if no value is present.</param>
    void Match(Action<TValue> some, Action none);
}