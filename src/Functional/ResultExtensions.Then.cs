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
        if (!result.TryGet(out T1? value)) return result;

        var response = then(value);
        if (copyReasonsAndMetadata) response = response.WithMetadata(result.Metadata);

        return response;
    }

    /// <summary>
    ///     Chains a transformation that returns a Result from the provided function.
    /// </summary>
    /// <typeparam name="TValue">The type of the value contained in the result.</typeparam>
    /// <param name="result">The result instance on which the transformation is applied.</param>
    /// <param name="then">The transformation function to execute when the result is successful.</param>
    /// <param name="copyReasonsAndMetadata">
    ///     Determines whether to copy the reasons and metadata from the original result to the new result.
    /// </param>
    /// <returns>
    ///     A new result obtained from the transformation function, while retaining metadata if specified.
    /// </returns>
    public static Result<TValue> Then<TValue>(this Result<TValue> result, Func<TValue, Result> then,
        bool copyReasonsAndMetadata = true) where TValue : notnull
    {
        if (!result.TryGet(out TValue? value)) return result;

        var thenResult = then(value);
        if (thenResult.TryGet(out var error)) return result;
        var failResult = Result.Failure<TValue>(error);
        return copyReasonsAndMetadata ? failResult.WithMetadata(result.Metadata) : failResult;
    }
}