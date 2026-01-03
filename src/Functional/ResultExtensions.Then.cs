namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Chains a transformation that returns a Result of the same type.
    /// </summary>
    /// <typeparam name="T1">Value type 1.</typeparam>
    /// <param name="result">The result instance.</param>
    /// <param name="then">The transformation function.</param>
    /// <param name="copyReasonsAndMetadata">Whether to copy reasons and metadata from original result.</param>
    /// <returns>A new result from the then function.</returns>
    public static Result<T1> Then<T1>(this Result<T1> result, Func<T1, Result<T1>> then,
        bool copyReasonsAndMetadata = true) where T1 : notnull
    {
        if (!result.TryGet(out T1? value))
        {
            return result;
        }

        var response = then(value);
        if (copyReasonsAndMetadata)
        {
            response = response.WithMetadata(result.Metadata);
        }

        return response;
    }
}
