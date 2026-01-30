namespace UnambitiousFx.Functional;

public static partial class ResultTaskExtensions
{
    /// <summary>
    ///     Wraps a <see cref="Result" /> in a <see cref="ResultTask" />.
    /// </summary>
    /// <param name="result">The result to wrap.</param>
    /// <returns>A wrapper around the result.</returns>
    public static ResultTask AsAsync(this Result result)
    {
        return new ResultTask(new ValueTask<Result>(result));
    }

    /// <summary>
    ///     Wraps a <see cref="Result{TValue}" /> in a <see cref="ResultTask{TValue}" />.
    /// </summary>
    /// <param name="result">The result to wrap.</param>
    /// <returns>A wrapper around the result.</returns>
    public static ResultTask<TValue> AsAsync<TValue>(this Result<TValue> result) where TValue : notnull
    {
        return new ResultTask<TValue>(new ValueTask<Result<TValue>>(result));
    }

    /// <summary>
    ///     Wraps a <see cref="ValueTask{TResult}" /> in a <see cref="ResultTask" />.
    /// </summary>
    /// <param name="resultTask">The task to wrap.</param>
    /// <returns>A wrapper around the task.</returns>
    public static ResultTask AsAsync(this ValueTask<Result> resultTask)
    {
        return new ResultTask(resultTask);
    }

    /// <summary>
    ///     Wraps a <see cref="ValueTask{TResult}" /> in a <see cref="ResultTask{TValue}" />.
    /// </summary>
    /// <param name="resultTask">The task to wrap.</param>
    /// <returns>A wrapper around the task.</returns>
    public static ResultTask<TValue> AsAsync<TValue>(this ValueTask<Result<TValue>> resultTask) where TValue : notnull
    {
        return new ResultTask<TValue>(resultTask);
    }

    /// <summary>
    ///     Wraps a <see cref="Task{TResult}" /> in a <see cref="ResultTask" />.
    /// </summary>
    /// <param name="resultTask">The task to wrap.</param>
    /// <returns>A wrapper around the task.</returns>
    public static ResultTask AsAsync(this Task<Result> resultTask)
    {
        return new ResultTask(new ValueTask<Result>(resultTask));
    }

    /// <summary>
    ///     Wraps a <see cref="Task{TResult}" /> in a <see cref="ResultTask{TValue}" />.
    /// </summary>
    /// <param name="resultTask">The task to wrap.</param>
    /// <returns>A wrapper around the task.</returns>
    public static ResultTask<TValue> AsAsync<TValue>(this Task<Result<TValue>> resultTask) where TValue : notnull
    {
        return new ResultTask<TValue>(new ValueTask<Result<TValue>>(resultTask));
    }
}