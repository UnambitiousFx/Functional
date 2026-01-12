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
        where TValue : notnull
    {
        return Maybe<TValue>.Some(value);
    }

    /// <summary>
    ///     Creates an instance of <see cref="Maybe{TValue}" /> that represents no value.
    /// </summary>
    /// <typeparam name="TValue">The type of the service represented by the option.</typeparam>
    /// <returns>An <see cref="Maybe{TValue}" /> instance that indicates no value.</returns>
    public static Maybe<TValue> None<TValue>()
        where TValue : notnull
    {
        return Maybe<TValue>.None();
    }
}

/// <summary>
///     Represents an optional value that can either contain a value (Some) or be empty (None).
/// </summary>
/// <typeparam name="TValue">The type of the value that may be present.</typeparam>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
[DebuggerTypeProxy(typeof(MaybeDebugView<>))]
public readonly record struct Maybe<TValue> : IMaybe<TValue>
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

    private string DebuggerDisplay => IsSome ? $"Some({_value})" : "None";

    /// <inheritdoc />
    public bool IsSome { get; }

    /// <inheritdoc />
    public bool IsNone => !IsSome;

    /// <inheritdoc />
    public TValue? Case => IsSome ? _value : default;


    /// <inheritdoc />
    public void IfNone(Action none)
    {
        if (IsNone) none();
    }

    /// <inheritdoc />
    public async ValueTask IfNone(Func<ValueTask> none)
    {
        if (IsNone) await none();
    }

    /// <inheritdoc />
    public void IfSome(Action<TValue> some)
    {
        if (IsSome) some(_value!);
    }


    /// <inheritdoc />
    public async ValueTask IfSome(Func<TValue, ValueTask> some)
    {
        if (IsSome) await some(_value!);
    }


    /// <inheritdoc />
    public bool Some([NotNullWhen(true)] out TValue? value)
    {
        value = _value;
        return IsSome;
    }

    /// <inheritdoc />
    public TOut Match<TOut>(Func<TValue, TOut> some, Func<TOut> none)
    {
        return IsSome ? some(_value!) : none();
    }


    /// <inheritdoc />
    public void Match(Action<TValue> some, Action none)
    {
        if (IsSome)
            some(_value!);
        else
            none();
    }

    /// <summary>
    ///     Creates a Maybe instance that represents no value.
    /// </summary>
    /// <returns>An empty <see cref="Maybe{TValue}" /> instance.</returns>
    public static Maybe<TValue> None()
    {
        return new Maybe<TValue>(false);
    }

    /// <summary>
    ///     Creates a Maybe instance that contains the specified value.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    /// <returns>A <see cref="Maybe{TValue}" /> instance containing the value.</returns>
    public static Maybe<TValue> Some(TValue value)
    {
        return new Maybe<TValue>(value);
    }

    /// <summary>
    ///     Implicitly converts a value to a Maybe instance containing that value.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>A <see cref="Maybe{TValue}" /> instance containing the value.</returns>
    public static implicit operator Maybe<TValue>(TValue? value)
    {
        return value is null ? None() : Some(value);
    }
}