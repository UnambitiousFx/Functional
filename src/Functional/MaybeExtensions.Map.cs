namespace UnambitiousFx.Functional;

public static partial class MaybeExtensions
{
    /// <summary>
    ///     Transforms the value inside the Maybe using a function, preserving None.
    /// </summary>
    /// <typeparam name="TIn">The type of the input value.</typeparam>
    /// <typeparam name="TOut">The type of the output value.</typeparam>
    /// <param name="maybe">The Maybe instance to map.</param>
    /// <param name="map">The mapping function to apply if a value is present.</param>
    /// <returns>A Maybe containing the mapped value if present; otherwise, None.</returns>
    public static Maybe<TOut> Map<TIn, TOut>(this Maybe<TIn> maybe,
        Func<TIn, TOut> map)
        where TIn : notnull
        where TOut : notnull
    {
        return maybe.Match(
            some: value => Maybe.Some(map(value)),
            none: Maybe.None<TOut>);
    }
}
