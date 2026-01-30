namespace UnambitiousFx.Functional;

public static partial class MaybeTaskExtensions
{
    /// <summary>
    ///     Asynchronously filters the MaybeTask value using a predicate, returning None when the predicate fails.
    /// </summary>
    /// <typeparam name="TValue">The type of the contained value.</typeparam>
    /// <param name="maybeTask">The MaybeTask instance to filter.</param>
    /// <param name="predicate">The predicate to apply to the contained value.</param>
    /// <returns>The original MaybeTask when the predicate holds; otherwise None.</returns>
    public static MaybeTask<TValue> Filter<TValue>(this MaybeTask<TValue> maybeTask,
        Func<TValue, bool> predicate)
        where TValue : notnull
    {
        return FilterCore(maybeTask, predicate).AsAsync();

        static async ValueTask<Maybe<TValue>> FilterCore(MaybeTask<TValue> maybeTask, Func<TValue, bool> predicate)
        {
            var maybe = await maybeTask;
            return maybe.Filter(predicate);
        }
    }
}
