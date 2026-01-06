namespace UnambitiousFx.Functional.Tasks;

public static partial class ResultExtensions
{
    /// <summary>
    /// Transforms a <see cref="Task{TResult}"/> containing a <see cref="Result{TValue}"/>
    /// to a new <see cref="Task{TResult}"/> with a specified output value of type <typeparamref name="TOut"/>
    /// upon success, while retaining the original error information upon failure.
    /// </summary>
    /// <typeparam name="TValue">The type of the value contained in the original result.</typeparam>
    /// <typeparam name="TOut">The type of the output value to transform to.</typeparam>
    /// <param name="awaitableResult">
    /// The <see cref="Task{TResult}"/> containing a <see cref="Result{TValue}"/> to be transformed.
    /// </param>
    /// <param name="value">The new value of type <typeparamref name="TOut"/> to associate with the resulting success.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> containing a <see cref="Result{TOut}"/> with the specified output value
    /// if the original result succeeded, or the original failure information if it failed.
    /// </returns>
    public static async Task<Result<TOut>> AsAsync<TValue, TOut>(this Task<Result<TValue>> awaitableResult, TOut value)
        where TValue : notnull where TOut : notnull
    {
        var result = await awaitableResult;
        return result.As(value);
    }
}