namespace UnambitiousFx.Functional;

public static partial class MaybeTaskExtensions
{
    /// <summary>
    ///     Asynchronously executes a side effect if the MaybeTask is Some, then returns the original value.
    /// </summary>
    /// <typeparam name="TValue">The type of the contained value.</typeparam>
    /// <param name="maybeTask">The MaybeTask instance to tap.</param>
    /// <param name="tap">The action to execute if a value is present.</param>
    /// <returns>The original MaybeTask unchanged.</returns>
    public static MaybeTask<TValue> Tap<TValue>(this MaybeTask<TValue> maybeTask,
        Action<TValue> tap)
        where TValue : notnull
    {
        return TapCore(maybeTask, tap).AsAsync();

        static async ValueTask<Maybe<TValue>> TapCore(MaybeTask<TValue> maybeTask, Action<TValue> tap)
        {
            var maybe = await maybeTask;
            return maybe.Tap(tap);
        }
    }

    /// <summary>
    ///     Asynchronously executes a side effect if the MaybeTask is Some, then returns the original value.
    /// </summary>
    /// <typeparam name="TValue">The type of the contained value.</typeparam>
    /// <param name="maybeTask">The MaybeTask instance to tap.</param>
    /// <param name="tap">The async action to execute if a value is present.</param>
    /// <returns>The original MaybeTask unchanged.</returns>
    public static MaybeTask<TValue> Tap<TValue>(this MaybeTask<TValue> maybeTask,
        Func<TValue, ValueTask> tap)
        where TValue : notnull
    {
        return TapCore(maybeTask, tap).AsAsync();

        static async ValueTask<Maybe<TValue>> TapCore(MaybeTask<TValue> maybeTask, Func<TValue, ValueTask> tap)
        {
            var maybe = await maybeTask;
            if (maybe.Some(out var value))
            {
                await tap(value);
            }

            return maybe;
        }
    }
}
