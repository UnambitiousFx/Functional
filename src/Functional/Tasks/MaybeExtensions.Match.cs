namespace UnambitiousFx.Functional.Tasks;

/// Provides extension methods for working with asynchronous options in the context
/// of the IOption interface and its implementations.
public static partial class MaybeExtensions
{
    /// Matches the current state of the asynchronous option and executes the corresponding function.
    /// This method allows handling both the presence and absence of a value in the option in an asynchronous context.
    /// <typeparam name="TOut">The type of the result returned by the invoked function.</typeparam>
    /// <typeparam name="TValue">The type of the value contained in the option.</typeparam>
    /// <param name="awaitableOption">
    ///     A Task that resolves to an instance of <see cref="Maybe{TValue}" />, representing the asynchronous option to
    ///     be matched.
    /// </param>
    /// <param name="some">
    ///     A function to execute if the option contains a value. The function receives the contained value as a parameter
    ///     and must return a result of type TOut.
    /// </param>
    /// <param name="none">
    ///     A function to execute if the option does not contain a value. This function must return a result of type TOut.
    /// </param>
    /// <returns>
    ///     The result of invoking either the "some" or "none" function based on the state of the matched option.
    /// </returns>
    public static async Task<TOut> MatchAsync<TOut, TValue>(this Task<Maybe<TValue>> awaitableOption,
        Func<TValue, TOut> some,
        Func<TOut> none)
        where TValue : notnull
    {
        return (await awaitableOption).Match(some, none);
    }

    /// Matches the current state of the asynchronous option and executes the corresponding asynchronous function.
    /// This method supports handling both the presence and absence of a value within the option in an asynchronous context.
    /// <typeparam name="TOut">The type of the result returned by the executed asynchronous function.</typeparam>
    /// <typeparam name="TValue">The type of the value contained within the option.</typeparam>
    /// <param name="awaitableOption">
    /// A Task that resolves to an instance of <see cref="Maybe{TValue}" />, representing the asynchronous option to
    /// be matched.
    /// </param>
    /// <param name="some">
    /// An asynchronous function to execute if the option contains a value. The function receives the value as input and must
    /// return a Task of type TOut.
    /// </param>
    /// <param name="none">
    /// An asynchronous function to execute if the option does not contain a value. This function must return a Task of type TOut.
    /// </param>
    /// <returns>
    /// A Task of type TOut that resolves to the result of invoking either the "some" or "none" asynchronous function based
    /// on the state of the matched option.
    /// </returns>
    public static async Task<TOut> MatchAsync<TOut, TValue>(this Task<Maybe<TValue>> awaitableOption,
        Func<TValue, Task<TOut>> some,
        Func<Task<TOut>> none)
        where TValue : notnull
    {
        var maybe = await awaitableOption;
        return await maybe.Match(some, none);
    }
}
