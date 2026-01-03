namespace UnambitiousFx.Functional.Tasks;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Asynchronously determines whether the result contains an exception of the specified type.
    /// </summary>
    /// <typeparam name="TException">The type of exception to check for.</typeparam>
    /// <param name="awaitableResult">The awaitable result to check for exceptions.</param>
    /// <returns>A task with true if the result contains an exception of the specified type; otherwise, false.</returns>
    public static async Task<bool> HasExceptionAsync<TException>(this Task<Result> awaitableResult)
        where TException : Exception
    {
        var result = await awaitableResult;
        return result.HasException<TException>();
    }
}