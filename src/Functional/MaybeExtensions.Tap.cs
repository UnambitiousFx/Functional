namespace UnambitiousFx.Functional;

public static partial class MaybeExtensions
{
    /// <summary>
    ///     Executes a side effect if the Maybe is Some, then returns the original value.
    /// </summary>
    /// <typeparam name="TValue">The type of the contained value.</typeparam>
    /// <param name="maybe">The Maybe instance to tap.</param>
    /// <param name="tap">The action to execute if a value is present.</param>
    /// <returns>The original Maybe instance unchanged.</returns>
    public static Maybe<TValue> Tap<TValue>(this Maybe<TValue> maybe,
        Action<TValue> tap)
        where TValue : notnull
    {
        if (maybe.Some(out var value))
        {
            tap(value);
        }

        return maybe;
    }
}
