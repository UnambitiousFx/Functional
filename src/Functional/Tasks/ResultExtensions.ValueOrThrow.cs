using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.Tasks;

public static partial class ResultExtensions
{
    extension<TValue1>(Task<Result<TValue1>> resultTask) where TValue1 : notnull
    {
        /// <summary>
        ///     Async ValueOrThrow throwing aggregated exception when failure.
        /// </summary>
        public async Task<TValue1> ValueOrThrowAsync()
        {
            var result = await resultTask.ConfigureAwait(false);
            return result.ValueOrThrow();
        }

        /// <summary>
        ///     Async ValueOrThrow using exception factory when failure.
        /// </summary>
        public async Task<TValue1> ValueOrThrowAsync(Func<Error, Exception> exceptionFactory)
        {
            var result = await resultTask.ConfigureAwait(false);
            return result.ValueOrThrow(exceptionFactory);
        }
    }
}