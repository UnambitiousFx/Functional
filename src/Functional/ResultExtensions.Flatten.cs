namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Flattens a nested Result, removing one level of nesting.
    /// </summary>
    /// <typeparam name="TValue">Value type 1.</typeparam>
    /// <param name="result">The nested result instance.</param>
    /// <returns>The inner result.</returns>
    public static Result<TValue> Flatten<TValue>(this Result<Result<TValue>> result) where TValue : notnull
    {
        return result.Bind(inner => inner);
    }
}