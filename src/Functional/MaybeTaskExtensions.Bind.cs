namespace UnambitiousFx.Functional;

public static partial class MaybeTaskExtensions
{
    /// <summary>
    ///     Asynchronously binds a function to a ValueTask-wrapped Maybe value, transforming the value if present.
    /// </summary>
    /// <typeparam name="TIn">The type of the input value.</typeparam>
    /// <typeparam name="TOut">The type of the output value.</typeparam>
    /// <param name="option">The ValueTask-wrapped Maybe value to bind.</param>
    /// <param name="bind">The function to apply to the value if present.</param>
    /// <returns>A ValueTask containing the transformed Maybe value if present, otherwise None.</returns>
    public static MaybeTask<TOut> Bind<TIn, TOut>(this MaybeTask<TIn> option,
        Func<TIn, Maybe<TOut>> bind)
        where TIn : notnull
        where TOut : notnull
    {
        return BindCore(option, bind).AsAsync();

        static async ValueTask<Maybe<TOut>> BindCore(MaybeTask<TIn> option, Func<TIn, Maybe<TOut>> bind)
        {
            var maybe = await option;
            return maybe.Match(bind, Maybe.None<TOut>);
        }
    }
}
