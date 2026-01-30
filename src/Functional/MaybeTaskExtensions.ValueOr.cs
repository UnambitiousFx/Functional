namespace UnambitiousFx.Functional;

public static partial class MaybeTaskExtensions
{
    /// <summary>
    ///     Asynchronously returns the contained value if present; otherwise returns the provided fallback value.
    /// </summary>
    /// <typeparam name="TValue">The type of the contained value.</typeparam>
    /// <param name="maybeTask">The MaybeTask instance to evaluate.</param>
    /// <param name="fallback">The fallback value to use when None.</param>
    /// <returns>A task containing the contained value or the fallback.</returns>
    public static async ValueTask<TValue> ValueOr<TValue>(this MaybeTask<TValue> maybeTask,
        TValue fallback)
        where TValue : notnull
    {
        var maybe = await maybeTask;
        return maybe.ValueOr(fallback);
    }

    /// <summary>
    ///     Asynchronously returns the contained value if present; otherwise returns the value created by the factory.
    /// </summary>
    /// <typeparam name="TValue">The type of the contained value.</typeparam>
    /// <param name="maybeTask">The MaybeTask instance to evaluate.</param>
    /// <param name="fallbackFactory">The factory to create a fallback value when None.</param>
    /// <returns>A task containing the contained value or the fallback created by the factory.</returns>
    public static async ValueTask<TValue> ValueOr<TValue>(this MaybeTask<TValue> maybeTask,
        Func<TValue> fallbackFactory)
        where TValue : notnull
    {
        var maybe = await maybeTask;
        return maybe.ValueOr(fallbackFactory);
    }
}
