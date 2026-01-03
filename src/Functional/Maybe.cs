using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace UnambitiousFx.Functional;

/// <summary>
///     Provides a static class to create and work with instances of <see cref="Maybe{TValue}" />,
///     which represents a value or the absence of a value.
/// </summary>
public static class Maybe
{
    /// <summary>
    ///     Creates a Maybe instance that represents a value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to wrap.</typeparam>
    /// <param name="value">The value to be wrapped inside the Maybe. It must be a non-null value of type TValue.</param>
    /// <returns>An <see cref="Maybe{TValue}" /> instance containing the provided value.</returns>
    public static Maybe<TValue> Some<TValue>(TValue value)
        where TValue : notnull =>
        Maybe<TValue>.Some(value);

    /// <summary>
    ///     Creates an instance of <see cref="Maybe{TValue}" /> that represents no value.
    /// </summary>
    /// <typeparam name="TValue">The type of the service represented by the option.</typeparam>
    /// <returns>An <see cref="Maybe{TValue}" /> instance that indicates no value.</returns>
    public static Maybe<TValue> None<TValue>()
        where TValue : notnull =>
        Maybe<TValue>.None();
}

/// <summary>
///     Represents an optional value that can either contain a value (Some) or be empty (None).
/// </summary>
/// <typeparam name="TValue">The type of the value that may be present.</typeparam>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
[DebuggerTypeProxy(typeof(MaybeDebugView<>))]
public readonly record struct Maybe<TValue>
    where TValue : notnull
{
    private readonly TValue? _value;

    private Maybe(TValue value)
    {
        _value = value;
        IsSome = true;
    }

    private Maybe(bool isSome)
    {
        _value = default;
        IsSome = isSome;
    }

    /// <summary>
    ///     Gets a value indicating whether this instance contains a value.
    /// </summary>
    public bool IsSome { get; }

    /// <summary>
    ///     Gets a value indicating whether this instance is empty (contains no value).
    /// </summary>
    public bool IsNone => !IsSome;

    /// <summary>
    ///     Gets the underlying value if present, or the default value if empty.
    /// </summary>
    public TValue? Case => IsSome ? _value : default;

    private string DebuggerDisplay => IsSome ? $"Some({_value})" : "None";

    /// <summary>
    ///     Creates a Maybe instance that represents no value.
    /// </summary>
    /// <returns>An empty <see cref="Maybe{TValue}" /> instance.</returns>
    public static Maybe<TValue> None() => new(false);

    /// <summary>
    ///     Creates a Maybe instance that contains the specified value.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    /// <returns>A <see cref="Maybe{TValue}" /> instance containing the value.</returns>
    public static Maybe<TValue> Some(TValue value) => new(value);

    /// <summary>
    ///     Executes the specified action if this instance is empty (None).
    /// </summary>
    /// <param name="none">The action to execute when no value is present.</param>
    public void IfNone(Action none)
    {
        if (IsNone)
        {
            none();
        }
    }

    /// <summary>
    ///     Asynchronously executes the specified function if this instance is empty (None).
    /// </summary>
    /// <param name="none">The asynchronous function to execute when no value is present.</param>
    /// <returns>A ValueTask that completes when the operation finishes.</returns>
    public async ValueTask IfNone(Func<ValueTask> none)
    {
        if (IsNone)
        {
            await none();
        }
    }

    /// <summary>
    ///     Executes the specified action if this instance contains a value (Some).
    /// </summary>
    /// <param name="some">The action to execute with the contained value.</param>
    public void IfSome(Action<TValue> some)
    {
        if (IsSome)
        {
            some(_value!);
        }
    }

    /// <summary>
    ///     Asynchronously executes the specified function if this instance contains a value (Some).
    /// </summary>
    /// <param name="some">The asynchronous function to execute with the contained value.</param>
    /// <returns>A ValueTask that completes when the operation finishes.</returns>
    public async ValueTask IfSome(Func<TValue, ValueTask> some)
    {
        if (IsSome)
        {
            await some(_value!);
        }
    }

    /// <summary>
    ///     Attempts to retrieve the contained value.
    /// </summary>
    /// <param name="value">When this method returns, contains the value if present; otherwise, the default value.</param>
    /// <returns>true if this instance contains a value; otherwise, false.</returns>
    public bool Some([NotNullWhen(true)] out TValue? value)
    {
        value = _value;
        return IsSome;
    }

    /// <summary>
    ///     Matches the Maybe instance and executes the corresponding function based on whether a value is present.
    /// </summary>
    /// <typeparam name="TOut">The type of the result.</typeparam>
    /// <param name="some">The function to execute if a value is present.</param>
    /// <param name="none">The function to execute if no value is present.</param>
    /// <returns>The result of the executed function.</returns>
    public TOut Match<TOut>(Func<TValue, TOut> some, Func<TOut> none)
        => IsSome ? some(_value!) : none();

    /// <summary>
    ///     Matches the Maybe instance and executes the corresponding action based on whether a value is present.
    /// </summary>
    /// <param name="some">The action to execute if a value is present.</param>
    /// <param name="none">The action to execute if no value is present.</param>
    public void Match(Action<TValue> some, Action none)
    {
        if (IsSome)
        {
            some(_value!);
        }
        else
        {
            none();
        }
    }

    /// <summary>
    ///     Implicitly converts a value to a Maybe instance containing that value.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>A <see cref="Maybe{TValue}" /> instance containing the value.</returns>
    public static implicit operator Maybe<TValue>(TValue value) => Some(value);
}
