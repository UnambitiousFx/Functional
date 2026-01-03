namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Returns the value as nullable if success; otherwise default.
    /// </summary>
    /// <typeparam name="TValue1">Value type 1.</typeparam>
    /// <param name="result">The result instance.</param>
    /// <returns>The nullable value or null/default.</returns>
    public static TValue1? ToNullable<TValue1>(this Result<TValue1> result) where TValue1 : notnull
    {
        return result.TryGet(out TValue1? value)
            ? value
            : default;
    }
}