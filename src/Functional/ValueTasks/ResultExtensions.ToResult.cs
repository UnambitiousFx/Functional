namespace UnambitiousFx.Functional.ValueTasks;

public static partial class ResultExtensions
{
    /// <summary>
    /// Asynchronously converts a <see cref="ValueTask{TResult}" /> of <see cref="Result{T}" /> to a <see cref="ValueTask{TResult}" /> of <see cref="Result" />.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value contained in the original result.
    /// </typeparam>
    /// <param name="awaitableResult">
    /// The <see cref="ValueTask{TResult}" /> of <see cref="Result{T}" /> to convert.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}" /> of <see cref="Result" />, representing the conversion of the original result.
    /// </returns>
    public static async ValueTask<Result> ToResultAsync<T>(this ValueTask<Result<T>> awaitableResult) where T : notnull
    {
        var result = await awaitableResult;
        return result.ToResult();
    }
}