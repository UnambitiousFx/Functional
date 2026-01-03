namespace UnambitiousFx.Functional.ValueTasks;

public static partial class ResultExtensions
{
    extension<TValue1>(ValueTask<Result<TValue1>> awaitableResult) where TValue1 : notnull
    {
        /// <summary>
        ///     Async ValueOr returning fallback(s) when failure.
        /// </summary>
        public async ValueTask<TValue1> ValueOrAsync(TValue1 fallback1)
        {
            var result = await awaitableResult.ConfigureAwait(false);
            return result.ValueOr(fallback1);
        }

        /// <summary>
        ///     Async ValueOr using fallback factory when failure.
        /// </summary>
        public async ValueTask<TValue1> ValueOrAsync(Func<TValue1> fallbackFactory)
        {
            var result = await awaitableResult.ConfigureAwait(false);
            return result.ValueOr(fallbackFactory);
        }
    }
}