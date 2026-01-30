namespace UnambitiousFx.Functional;

public static partial class MaybeTaskExtensions
{
    /// <summary>
    ///     Asynchronously transforms the value inside a MaybeTask using a function, preserving None.
    /// </summary>
    /// <typeparam name="TIn">The type of the input value.</typeparam>
    /// <typeparam name="TOut">The type of the output value.</typeparam>
    /// <param name="maybeTask">The MaybeTask instance to map.</param>
    /// <param name="map">The mapping function to apply if a value is present.</param>
    /// <returns>A MaybeTask containing the mapped value if present; otherwise, None.</returns>
    public static MaybeTask<TOut> Map<TIn, TOut>(this MaybeTask<TIn> maybeTask,
        Func<TIn, TOut> map)
        where TIn : notnull
        where TOut : notnull
    {
        return MapCore(maybeTask, map).AsAsync();

        static async ValueTask<Maybe<TOut>> MapCore(MaybeTask<TIn> maybeTask, Func<TIn, TOut> map)
        {
            var maybe = await maybeTask;
            return maybe.Map(map);
        }
    }
}
