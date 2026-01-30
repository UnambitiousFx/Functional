using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnambitiousFx.Functional;

/// <summary>
///     Represents an awaitable wrapper around a <see cref="ValueTask{TResult}" /> that yields a
///     <see cref="Maybe{TValue}" />.
/// </summary>
/// <typeparam name="TValue">The optional value type.</typeparam>
[StructLayout(LayoutKind.Auto)]
public readonly partial struct MaybeTask<TValue> where TValue : notnull
{
    private readonly ValueTask<Maybe<TValue>> _inner;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MaybeTask{TValue}" /> struct.
    /// </summary>
    /// <param name="inner">The underlying maybe task.</param>
    public MaybeTask(ValueTask<Maybe<TValue>> inner)
    {
        _inner = inner;
    }

    /// <summary>
    ///     Gets the awaiter for the underlying <see cref="ValueTask{TResult}" />.
    /// </summary>
    /// <returns>The awaiter for the wrapped task.</returns>
    public ValueTaskAwaiter<Maybe<TValue>> GetAwaiter()
    {
        return _inner.GetAwaiter();
    }

    /// <summary>
    ///     Returns the underlying <see cref="ValueTask{TResult}" />.
    /// </summary>
    /// <returns>The wrapped task.</returns>
    public ValueTask<Maybe<TValue>> AsValueTask()
    {
        return _inner;
    }

    /// <summary>
    ///     Implicitly converts a <see cref="Maybe{TValue}" /> to a <see cref="MaybeTask{TValue}" />.
    /// </summary>
    /// <param name="maybe">The maybe to wrap.</param>
    /// <returns>A completed MaybeTask wrapping the maybe.</returns>
    public static implicit operator MaybeTask<TValue>(Maybe<TValue> maybe)
    {
        return new MaybeTask<TValue>(new ValueTask<Maybe<TValue>>(maybe));
    }

    /// <summary>
    ///     Implicitly converts a value to a <see cref="MaybeTask{TValue}" />.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    /// <returns>A completed MaybeTask wrapping the value.</returns>
    public static implicit operator MaybeTask<TValue>(TValue value)
    {
        return new MaybeTask<TValue>(new ValueTask<Maybe<TValue>>(Maybe.Some(value)));
    }

    /// <summary>
    ///     Implicitly converts a <see cref="MaybeTask{TValue}" /> to a <see cref="ValueTask{TResult}" />.
    /// </summary>
    /// <param name="maybeTask">The wrapper to convert.</param>
    /// <returns>The underlying task.</returns>
    public static implicit operator ValueTask<Maybe<TValue>>(MaybeTask<TValue> maybeTask)
    {
        return maybeTask._inner;
    }
}
