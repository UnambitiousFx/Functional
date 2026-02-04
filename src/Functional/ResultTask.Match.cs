using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional;

public readonly partial struct ResultTask
{
    /// <summary>
    ///     Asynchronously pattern matches the result, executing the appropriate action for success or failure.
    /// </summary>
    /// <param name="onSuccess">Action to execute if the result is successful.</param>
    /// <param name="onFailure">Action to execute if the result is a failure.</param>
    /// <returns>A <see cref="ValueTask" /> representing the asynchronous operation.</returns>
    public async ValueTask Match(Action onSuccess, Action<Failure> onFailure)
    {
        var result = await _inner;
        result.Match(onSuccess, onFailure);
    }

    /// <summary>
    ///     Matches the result asynchronously, executing the appropriate function
    ///     based on the outcome of the operation represented by the <see cref="ResultTask" />.
    /// </summary>
    /// <param name="onSuccess">A function that will be executed if the result is successful.</param>
    /// <param name="onFailure">
    ///     A function that will be executed if the result is a failure, taking an <see cref="Failure" /> as
    ///     input.
    /// </param>
    /// <returns>A <see cref="ValueTask" /> representing the asynchronous operation.</returns>
    public async ValueTask Match(Func<ValueTask> onSuccess, Func<Failure, ValueTask> onFailure)
    {
        var result = await _inner;
        await result.Match(onSuccess, onFailure);
    }

    /// <summary>
    ///     Pattern matches the result, executing the appropriate asynchronous tasks.
    /// </summary>
    /// <param name="onSuccess">A function representing the asynchronous task to execute on success.</param>
    /// <param name="onFailure">
    ///     A function representing the asynchronous task to execute on failure, which takes an
    ///     <see cref="Failure" /> as input.
    /// </param>
    /// <returns>An awaitable <see cref="Task" />.</returns>
    public async Task Match(Func<Task> onSuccess, Func<Failure, Task> onFailure)
    {
        var result = await _inner;
        await result.Match(onSuccess, onFailure);
    }

    /// <summary>
    ///     Pattern matches the result asynchronously, returning a value from the appropriate function.
    /// </summary>
    /// <typeparam name="TOut">The type of value to return.</typeparam>
    /// <param name="onSuccess">Function to invoke if the result is successful.</param>
    /// <param name="onFailure">Function to invoke if the result is a failure.</param>
    /// <returns>A task representing the result of invoking the appropriate function.</returns>
    public async ValueTask<TOut> Match<TOut>(Func<TOut> onSuccess, Func<Failure, TOut> onFailure)
    {
        var result = await _inner;
        return result.Match(onSuccess, onFailure);
    }

    /// <summary>
    ///     Asynchronously pattern matches the result, returning a value from the appropriate function.
    /// </summary>
    /// <typeparam name="TOut">The type of value to return.</typeparam>
    /// <param name="onSuccess">
    ///     A function that returns a <see cref="ValueTask{TOut}" /> to execute if the result is
    ///     successful.
    /// </param>
    /// <param name="onFailure">A function that returns a <see cref="ValueTask{TOut}" /> to execute if the result is a failure.</param>
    /// <returns>A <see cref="ValueTask{TOut}" /> containing the result of invoking the appropriate function.</returns>
    public async ValueTask<TOut> Match<TOut>(Func<ValueTask<TOut>> onSuccess, Func<Failure, ValueTask<TOut>> onFailure)
    {
        var result = await _inner;
        return await result.Match(onSuccess, onFailure);
    }
}

public readonly partial struct ResultTask<TValue>
{
    /// <summary>
    ///     Asynchronously pattern matches the result, executing the appropriate action for success or failure.
    /// </summary>
    /// <param name="onSuccess">Action to execute if the result is successful.</param>
    /// <param name="onFailure">Action to execute if the result is a failure.</param>
    /// <returns>A <see cref="ValueTask" /> representing the asynchronous operation.</returns>
    public async ValueTask Match(Action<TValue> onSuccess, Action<Failure> onFailure)
    {
        var result = await _inner;
        result.Match(onSuccess, onFailure);
    }

    /// <summary>
    ///     Matches the result asynchronously, executing the appropriate asynchronous function
    ///     based on the outcome of the operation represented by the <see cref="ResultTask{TValue}" />.
    /// </summary>
    /// <param name="onSuccess">
    ///     A function that will be executed asynchronously if the result is successful,
    ///     taking the result value as input.
    /// </param>
    /// <param name="onFailure">
    ///     A function that will be executed asynchronously if the result is a failure,
    ///     taking an <see cref="Failure" /> as input.
    /// </param>
    /// <returns>A <see cref="ValueTask" /> representing the asynchronous operation.</returns>
    public async ValueTask Match(Func<TValue, ValueTask> onSuccess, Func<Failure, ValueTask> onFailure)
    {
        var result = await _inner;
        await result.Match(onSuccess, onFailure);
    }

    /// <summary>
    ///     Pattern matches the result, executing the appropriate asynchronous tasks.
    /// </summary>
    /// <param name="onSuccess">A function representing the asynchronous task to execute on success.</param>
    /// <param name="onFailure">
    ///     A function representing the asynchronous task to execute on failure, which takes an
    ///     <see cref="Failure" /> as input.
    /// </param>
    /// <returns>An awaitable <see cref="Task" />.</returns>
    public async Task Match(Func<TValue, Task> onSuccess, Func<Failure, Task> onFailure)
    {
        var result = await _inner;
        await result.Match(onSuccess, onFailure);
    }

    /// <summary>
    ///     Pattern matches the result asynchronously, returning a value from the appropriate function.
    /// </summary>
    /// <typeparam name="TOut">The type of value to return.</typeparam>
    /// <param name="onSuccess">Function to invoke if the result is successful.</param>
    /// <param name="onFailure">Function to invoke if the result is a failure.</param>
    /// <returns>A task representing the result of invoking the appropriate function.</returns>
    public async ValueTask<TOut> Match<TOut>(Func<TValue, TOut> onSuccess, Func<Failure, TOut> onFailure)
    {
        var result = await _inner;
        return result.Match(onSuccess, onFailure);
    }

    /// <summary>
    ///     Asynchronously pattern matches the result, invoking the appropriate function based on the result type.
    /// </summary>
    /// <param name="onSuccess">
    ///     A function to execute if the result is successful, which returns a <see cref="ValueTask{TOut}" />.
    /// </param>
    /// <param name="onFailure">
    ///     A function to execute if the result is a failure, which returns a <see cref="ValueTask{TOut}" />.
    /// </param>
    /// <typeparam name="TOut">The type of the value returned by the function.</typeparam>
    /// <returns>
    ///     A <see cref="ValueTask{TOut}" /> containing the result of executing the corresponding function.
    /// </returns>
    public async ValueTask<TOut> Match<TOut>(Func<TValue, ValueTask<TOut>> onSuccess,
        Func<Failure, ValueTask<TOut>> onFailure)
    {
        var result = await _inner;
        return await result.Match(onSuccess, onFailure);
    }
}