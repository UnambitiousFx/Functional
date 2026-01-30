namespace UnambitiousFx.Functional;

public static partial class ResultTaskExtensions
{
    /// <summary>
    ///     Flattens a nested ResultTask, removing one level of nesting.
    /// </summary>
    /// <typeparam name="TValue">The type of the value contained within the ResultTask.</typeparam>
    /// <param name="result">The nested ResultTask instance.</param>
    /// <returns>A ResultTask containing the inner result.</returns>
    public static ResultTask<TValue> Flatten<TValue>(this ResultTask<Result<TValue>> result) where TValue : notnull
    {
        return FlattenCore(result).AsAsync();

        static async ValueTask<Result<TValue>> FlattenCore(ResultTask<Result<TValue>> self)
        {
            var source = await self;
            return source.Match(
                inner => inner.WithMetadata(source.Metadata),
                error => Result.Failure<TValue>(error).WithMetadata(source.Metadata));
        }
    }

    /// <summary>
    ///     Flattens a nested ResultTask, removing one level of nesting.
    /// </summary>
    /// <typeparam name="TValue">The type of the underlying value.</typeparam>
    /// <param name="result">The nested ResultTask instance.</param>
    /// <returns>A flattened ResultTask instance.</returns>
    public static ResultTask<TValue> Flatten<TValue>(this ResultTask<ResultTask<TValue>> result) where TValue : notnull
    {
        return FlattenCore(result).AsAsync();

        static async ValueTask<Result<TValue>> FlattenCore(ResultTask<ResultTask<TValue>> self)
        {
            var source = await self;
            return await source.Match<ValueTask<Result<TValue>>>(
                async innerTask =>
                {
                    var innerResult = await innerTask;
                    return innerResult.WithMetadata(source.Metadata);
                },
                error => ValueTask.FromResult(Result.Failure<TValue>(error).WithMetadata(source.Metadata)));
        }
    }
}