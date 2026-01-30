namespace UnambitiousFx.Functional;

public static partial class MaybeTaskExtensions
{
    /// <summary>
    ///     Asynchronously returns the current MaybeTask if it is Some; otherwise returns the fallback value.
    /// </summary>
    /// <typeparam name="TValue">The type of the contained value.</typeparam>
    /// <param name="maybeTask">The MaybeTask instance to evaluate.</param>
    /// <param name="fallback">The fallback Maybe to use when None.</param>
    /// <returns>The current MaybeTask if Some; otherwise the fallback.</returns>
    public static MaybeTask<TValue> OrElse<TValue>(this MaybeTask<TValue> maybeTask,
        Maybe<TValue> fallback)
        where TValue : notnull
    {
        return OrElseCore(maybeTask, fallback).AsAsync();

        static async ValueTask<Maybe<TValue>> OrElseCore(MaybeTask<TValue> maybeTask, Maybe<TValue> fallback)
        {
            var maybe = await maybeTask;
            return maybe.OrElse(fallback);
        }
    }

    /// <summary>
    ///     Asynchronously returns the current MaybeTask if it is Some; otherwise returns the fallback created by the factory.
    /// </summary>
    /// <typeparam name="TValue">The type of the contained value.</typeparam>
    /// <param name="maybeTask">The MaybeTask instance to evaluate.</param>
    /// <param name="fallbackFactory">The factory to create a fallback Maybe when None.</param>
    /// <returns>The current MaybeTask if Some; otherwise the fallback from the factory.</returns>
    public static MaybeTask<TValue> OrElse<TValue>(this MaybeTask<TValue> maybeTask,
        Func<Maybe<TValue>> fallbackFactory)
        where TValue : notnull
    {
        return OrElseCore(maybeTask, fallbackFactory).AsAsync();

        static async ValueTask<Maybe<TValue>> OrElseCore(MaybeTask<TValue> maybeTask,
            Func<Maybe<TValue>> fallbackFactory)
        {
            var maybe = await maybeTask;
            return maybe.OrElse(fallbackFactory);
        }
    }
}
