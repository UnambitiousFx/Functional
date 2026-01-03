namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Flattens a nested Result, removing one level of nesting.
    /// </summary>
    /// <typeparam name="TValue1">Value type 1.</typeparam>
    /// <param name="result">The nested result instance.</param>
    /// <returns>The inner result.</returns>
    public static Result<TValue1> Flatten<TValue1>(this Result<Result<TValue1>> result) where TValue1 : notnull
    {
        return result.Bind(inner => inner);
    }
}