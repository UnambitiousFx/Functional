using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnambitiousFx.Functional;

/// <summary>
///     Represents an awaitable wrapper around a <see cref="ValueTask{TResult}" /> that yields a <see cref="Result" />.
/// </summary>
[StructLayout(LayoutKind.Auto)]
public readonly partial struct ResultTask
{
    private readonly ValueTask<Result> _inner;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ResultTask" /> struct.
    /// </summary>
    /// <param name="inner">The underlying result task.</param>
    public ResultTask(ValueTask<Result> inner)
    {
        _inner = inner;
    }

    /// <summary>
    ///     Gets the awaiter for the underlying <see cref="ValueTask{TResult}" />.
    /// </summary>
    /// <returns>The awaiter for the wrapped task.</returns>
    public ValueTaskAwaiter<Result> GetAwaiter()
    {
        return _inner.GetAwaiter();
    }

    /// <summary>
    ///     Returns the underlying <see cref="ValueTask{TResult}" />.
    /// </summary>
    /// <returns>The wrapped task.</returns>
    public ValueTask<Result> AsValueTask()
    {
        return _inner;
    }

    /// <summary>
    ///     Implicitly converts a <see cref="Result" /> to a <see cref="ResultTask" />.
    /// </summary>
    /// <param name="result">The result to wrap.</param>
    /// <returns>A completed ResultAsync wrapping the result.</returns>
    public static implicit operator ResultTask(Result result)
    {
        return new ResultTask(new ValueTask<Result>(result));
    }

    /// <summary>
    ///     Implicitly converts an <see cref="Failures.Failure" /> to a <see cref="ResultTask" />.
    /// </summary>
    /// <param name="failure">The error to wrap.</param>
    /// <returns>A completed ResultTask wrapping the failure.</returns>
    public static implicit operator ResultTask(Failures.Failure failure)
    {
        return new ResultTask(new ValueTask<Result>(Result.Failure(failure)));
    }

    /// <summary>
    ///     Implicitly converts a <see cref="ResultTask" /> to a <see cref="ValueTask{TResult}" />.
    /// </summary>
    /// <param name="resultTask">The wrapper to convert.</param>
    /// <returns>The underlying task.</returns>
    public static implicit operator ValueTask<Result>(ResultTask resultTask)
    {
        return resultTask._inner;
    }
}

/// <summary>
///     Represents an awaitable wrapper around a <see cref="ValueTask{TResult}" /> that yields a
///     <see cref="Result{TValue}" />.
/// </summary>
/// <typeparam name="TValue">The result value type.</typeparam>
[StructLayout(LayoutKind.Auto)]
public readonly partial struct ResultTask<TValue> where TValue : notnull
{
    private readonly ValueTask<Result<TValue>> _resultTask;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ResultTask{TValue}" /> struct.
    /// </summary>
    /// <param name="resultTask">The underlying result task.</param>
    public ResultTask(ValueTask<Result<TValue>> resultTask)
    {
        _resultTask = resultTask;
    }

    /// <summary>
    ///     Gets the awaiter for the underlying <see cref="ValueTask{TResult}" />.
    /// </summary>
    /// <returns>The awaiter for the wrapped task.</returns>
    public ValueTaskAwaiter<Result<TValue>> GetAwaiter()
    {
        return _resultTask.GetAwaiter();
    }

    /// <summary>
    ///     Returns the underlying <see cref="ValueTask{TResult}" />.
    /// </summary>
    /// <returns>The wrapped task.</returns>
    public ValueTask<Result<TValue>> AsValueTask()
    {
        return _resultTask;
    }

    /// <summary>
    ///     Implicitly converts a <see cref="Result{TValue}" /> to a <see cref="ResultTask{TValue}" />.
    /// </summary>
    /// <param name="result">The result to wrap.</param>
    /// <returns>A completed ResultAsync wrapping the result.</returns>
    public static implicit operator ResultTask<TValue>(Result<TValue> result)
    {
        return new ResultTask<TValue>(new ValueTask<Result<TValue>>(result));
    }

    /// <summary>
    ///     Implicitly converts a value to a <see cref="ResultTask{TValue}" />.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    /// <returns>A completed ResultTask wrapping the value.</returns>
    public static implicit operator ResultTask<TValue>(TValue value)
    {
        return new ResultTask<TValue>(new ValueTask<Result<TValue>>(Result.Success(value)));
    }

    /// <summary>
    ///     Implicitly converts an <see cref="Failures.Failure" /> to a <see cref="ResultTask{TValue}" />.
    /// </summary>
    /// <param name="failure">The error to wrap.</param>
    /// <returns>A completed ResultTask wrapping the failure.</returns>
    public static implicit operator ResultTask<TValue>(Failures.Failure failure)
    {
        return new ResultTask<TValue>(new ValueTask<Result<TValue>>(Result.Failure<TValue>(failure)));
    }

    /// <summary>
    ///     Implicitly converts a <see cref="ResultTask{TValue}" /> to a <see cref="ValueTask{TResult}" />.
    /// </summary>
    /// <param name="resultTask">The wrapper to convert.</param>
    /// <returns>The underlying task.</returns>
    public static implicit operator ValueTask<Result<TValue>>(ResultTask<TValue> resultTask)
    {
        return resultTask._resultTask;
    }
}