namespace UnambitiousFx.Functional;

public static partial class MaybeExtensions
{
    /// <summary>
    ///     Returns the contained value if present; otherwise returns the provided fallback value.
    /// </summary>
    /// <typeparam name="TValue">The type of the contained value.</typeparam>
    /// <param name="maybe">The Maybe instance to evaluate.</param>
    /// <param name="fallback">The fallback value to use when None.</param>
    /// <returns>The contained value or the fallback.</returns>
    public static TValue ValueOr<TValue>(this Maybe<TValue> maybe,
        TValue fallback)
        where TValue : notnull
    {
        return maybe.Match(
            some: value => value,
            none: () => fallback);
    }

    /// <summary>
    ///     Returns the contained value if present; otherwise returns the value created by the factory.
    /// </summary>
    /// <typeparam name="TValue">The type of the contained value.</typeparam>
    /// <param name="maybe">The Maybe instance to evaluate.</param>
    /// <param name="fallbackFactory">The factory to create a fallback value when None.</param>
    /// <returns>The contained value or the fallback created by the factory.</returns>
    public static TValue ValueOr<TValue>(this Maybe<TValue> maybe,
        Func<TValue> fallbackFactory)
        where TValue : notnull
    {
        return maybe.Match(
            some: value => value,
            none: fallbackFactory);
    }
}
