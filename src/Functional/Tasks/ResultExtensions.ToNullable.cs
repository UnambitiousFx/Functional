namespace UnambitiousFx.Functional.Tasks;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Async ToNullable returning nullable value(s) when success, null when failure.
    /// </summary>
    public static async Task<TValue1?> ToNullableAsync<TValue1>(this Task<Result<TValue1>> awaitable)
        where TValue1 : notnull
    {
        var result = await awaitable.ConfigureAwait(false);
        return result.ToNullable();
    }
}