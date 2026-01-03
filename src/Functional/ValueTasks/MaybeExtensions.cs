namespace UnambitiousFx.Functional.ValueTasks;

public static partial class MaybeExtensions
{
    /// <summary>
    ///     Asynchronously binds a function to a ValueTask-wrapped Maybe value, transforming the value if present.
    /// </summary>
    /// <typeparam name="TIn">The type of the input value.</typeparam>
    /// <typeparam name="TOut">The type of the output value.</typeparam>
    /// <param name="option">The ValueTask-wrapped Maybe value to bind.</param>
    /// <param name="bind">The function to apply to the value if present.</param>
    /// <returns>A ValueTask containing the transformed Maybe value if present, otherwise None.</returns>
    public static ValueTask<Maybe<TOut>> BindAsync<TIn, TOut>(this ValueTask<Maybe<TIn>> option,
        Func<TIn, Maybe<TOut>> bind)
        where TIn : notnull
        where TOut : notnull
    {
        return option.MatchAsync(bind, Maybe.None<TOut>);
    }
}
