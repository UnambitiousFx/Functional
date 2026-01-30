namespace UnambitiousFx.Functional;

public readonly partial struct MaybeTask<TValue>
{
    /// <summary>
    ///     Matches over the current <see cref="MaybeTask{TValue}" />, returning a result based on whether
    ///     the underlying value is present or not.
    /// </summary>
    /// <typeparam name="TOut">The return type of the match functions.</typeparam>
    /// <param name="some">A function invoked with the value if present.</param>
    /// <param name="none">A function invoked if the value is absent.</param>
    /// <returns>
    ///     A task that represents the asynchronous match operation. The result of the task is the
    ///     output of either the <paramref name="some" /> function if a value is present, or the
    ///     <paramref name="none" /> function if absent.
    /// </returns>
    public async ValueTask<TOut> Match<TOut>(Func<TValue, TOut> some,
        Func<TOut> none)
    {
        var maybe = await _inner;
        return maybe.Match(some, none);
    }

    /// <summary>
    ///     Applies the provided functions to the result of the underlying <see cref="Maybe{TValue}" /> task,
    ///     depending on whether it contains a value or not, and returns the result.
    /// </summary>
    /// <typeparam name="TOut">The type of the result returned by the matching functions.</typeparam>
    /// <param name="some">
    ///     A function to be executed if the underlying <see cref="Maybe{TValue}" /> contains a value.
    ///     The function takes the value as input and returns a result.
    /// </param>
    /// <param name="none">
    ///     A function to be executed if the underlying <see cref="Maybe{TValue}" /> does not contain a value.
    ///     It returns a result.
    /// </param>
    /// <returns>A <see cref="ValueTask{TResult}" /> containing the result of applying the provided functions.</returns>
    public async ValueTask<TOut> Match<TOut>(Func<TValue, ValueTask<TOut>> some,
        Func<ValueTask<TOut>> none)
    {
        var maybe = await _inner;
        return await maybe.Match(some, none);
    }
}