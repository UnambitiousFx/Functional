namespace UnambitiousFx.Functional.ValueTasks;

public static partial class ResultExtensions
{
    /// <summary>
    /// Asynchronously converts a <see cref="ValueTask{TResult}" /> of <see cref="Result{T}" /> to a <see cref="ValueTask{TResult}" /> of <see cref="Result" />.
    /// </summary>
    /// <param name="result">
    /// The <see cref="ValueTask{TResult}" /> of <see cref="Result{T}" /> to convert.
    /// </param>
    /// <typeparam name="T">
    /// The type of the value contained in the original result.
    /// </typeparam>
    /// <returns>
    /// A <see cref="ValueTask{TResult}" /> of <see cref="Result" />, representing the conversion of the original result.
    /// </returns>
    public static ValueTask<Result> ToResultAsync<T>(this ValueTask<Result<T>> result) where T : notnull
    {
        return result.BindAsync(_ => ValueTask.FromResult(Result.Success()));
    }
}