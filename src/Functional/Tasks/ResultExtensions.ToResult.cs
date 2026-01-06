namespace UnambitiousFx.Functional.Tasks;

public static partial class ResultExtensions
{
    /// <summary>
    /// Asynchronously converts a <see cref="Task{TResult}" /> of <see cref="Result{T}" /> to a <see cref="Task{TResult}" /> of <see cref="Result" />.
    /// </summary>
    /// <param name="awaitableResult">
    /// The <see cref="Task{TResult}" /> of <see cref="Result{T}" /> to convert.
    /// </param>
    /// <typeparam name="T">
    /// The type of the value contained in the original result.
    /// </typeparam>
    /// <returns>
    /// A <see cref="Task{TResult}" /> of <see cref="Result" />, representing the conversion of the original result.
    /// </returns>
    public static async Task<Result> ToResultAsync<T>(this Task<Result<T>> awaitableResult) where T : notnull
    {
        var result = await awaitableResult;
        return result.ToResult();
    }
}