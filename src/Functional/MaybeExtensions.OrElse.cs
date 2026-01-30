namespace UnambitiousFx.Functional;

public static partial class MaybeExtensions
{
    /// <summary>
    ///     Returns the current Maybe if it is Some; otherwise returns the fallback value.
    /// </summary>
    /// <typeparam name="TValue">The type of the contained value.</typeparam>
    /// <param name="maybe">The Maybe instance to evaluate.</param>
    /// <param name="fallback">The fallback Maybe to use when None.</param>
    /// <returns>The current Maybe if Some; otherwise the fallback.</returns>
    public static Maybe<TValue> OrElse<TValue>(this Maybe<TValue> maybe,
        Maybe<TValue> fallback)
        where TValue : notnull
    {
        return maybe.IsSome ? maybe : fallback;
    }

    /// <summary>
    ///     Returns the current Maybe if it is Some; otherwise returns the fallback created by the factory.
    /// </summary>
    /// <typeparam name="TValue">The type of the contained value.</typeparam>
    /// <param name="maybe">The Maybe instance to evaluate.</param>
    /// <param name="fallbackFactory">The factory to create a fallback Maybe when None.</param>
    /// <returns>The current Maybe if Some; otherwise the fallback from the factory.</returns>
    public static Maybe<TValue> OrElse<TValue>(this Maybe<TValue> maybe,
        Func<Maybe<TValue>> fallbackFactory)
        where TValue : notnull
    {
        return maybe.IsSome ? maybe : fallbackFactory();
    }
}
