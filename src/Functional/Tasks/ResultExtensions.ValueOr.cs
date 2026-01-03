namespace UnambitiousFx.Functional.Tasks;

public static partial class ResultExtensions
{
    extension<TValue1>(Task<Result<TValue1>> awaitableResult) where TValue1 : notnull
    {
        /// <summary>
        ///     Async ValueOr returning fallback(s) when failure.
        /// </summary>
        public async Task<TValue1> ValueOrAsync(TValue1 fallback1)
        {
            var result = await awaitableResult.ConfigureAwait(false);
            return result.ValueOr(fallback1);
        }

        /// <summary>
        ///     Async ValueOr using fallback factory when failure.
        /// </summary>
        public async Task<TValue1> ValueOrAsync(Func<TValue1> fallbackFactory)
        {
            var result = await awaitableResult.ConfigureAwait(false);
            return result.ValueOr(fallbackFactory);
        }
    }
}