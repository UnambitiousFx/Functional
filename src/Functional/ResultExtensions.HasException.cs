namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Determines whether the result contains an error with an exception to the specified type.
    /// </summary>
    /// <typeparam name="TException">The type of exception to check for.</typeparam>
    /// <param name="result">The result to check for exceptions.</param>
    /// <returns>true if the result contains an error with an exception to the specified type; otherwise, false.</returns>
    public static bool HasException<TException>(this Result result)
        where TException : Exception
    {
        if (result.IsSuccess)
        {
            return false;
        }

        return !result.TryGet(out var error) && ContainsException<TException>(error);
    }
}
