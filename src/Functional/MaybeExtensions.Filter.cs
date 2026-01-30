namespace UnambitiousFx.Functional;

public static partial class MaybeExtensions
{
    /// <summary>
    ///     Filters the Maybe value using a predicate, returning None when the predicate fails.
    /// </summary>
    /// <typeparam name="TValue">The type of the contained value.</typeparam>
    /// <param name="maybe">The Maybe instance to filter.</param>
    /// <param name="predicate">The predicate to apply to the contained value.</param>
    /// <returns>The original Maybe when the predicate holds; otherwise None.</returns>
    public static Maybe<TValue> Filter<TValue>(this Maybe<TValue> maybe,
        Func<TValue, bool> predicate)
        where TValue : notnull
    {
        if (!maybe.Some(out var value))
        {
            return maybe;
        }

        return predicate(value) ? maybe : Maybe.None<TValue>();
    }
}
