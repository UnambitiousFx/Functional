namespace UnambitiousFx.Functional.ValueTasks;

/// Provides extension methods for working with asynchronous options in the context
/// of the IOption interface and its implementations.
public static partial class MaybeExtensions
{
    /// Matches the current state of the asynchronous option and executes the corresponding function.
    /// This method allows handling both the presence and absence of a value in the option in an asynchronous context.
    /// <typeparam name="TOut">The type of the result returned by the invoked function.</typeparam>
    /// <typeparam name="TValue">The type of the value contained in the option.</typeparam>
    /// <param name="awaitableOption">
    ///     A ValueTask that resolves to an instance of <see cref="Maybe{TValue}" />, representing the asynchronous option to
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
    public static async ValueTask<TOut> MatchAsync<TOut, TValue>(this ValueTask<Maybe<TValue>> awaitableOption,
        Func<TValue, TOut> some,
        Func<TOut> none)
        where TValue : notnull
    {
        return (await awaitableOption).Match(some, none);
    }
}
