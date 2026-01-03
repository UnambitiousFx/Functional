using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
  
    /// <summary>
    ///     Matches the first error of the specified type in the result and executes the corresponding function.
    /// </summary>
    /// <typeparam name="TError">The type of error to match. Must be a class implementing Error.</typeparam>
    /// <typeparam name="TOut">The type of the output value.</typeparam>
    /// <param name="result">The result to match errors for.</param>
    /// <param name="onMatch">The function to execute when a matching error is found.</param>
    /// <param name="onElse">The function to execute when no matching error is found.</param>
    /// <returns>The result of executing either onMatch or onElse function.</returns>
    public static TOut MatchError<TError, TOut>(this Result result, Func<TError, TOut> onMatch, Func<TOut> onElse)
        where TError : class, IError
    {
        return result.GetError<TError>()
            .Match(onMatch, onElse);
    }
    
    
    /// <summary>
    ///     Matches the first error of the specified type in the result and executes the corresponding function.
    /// </summary>
    /// <typeparam name="TError">The type of error to match. Must be a class implementing Error.</typeparam>
    /// <typeparam name="TOut">The type of the output value.</typeparam>
    /// <typeparam name="TValue1">The type of the first value in the result.</typeparam>
    /// <param name="result">The result to match errors for.</param>
    /// <param name="onMatch">The function to execute when a matching error is found.</param>
    /// <param name="onElse">The function to execute when no matching error is found.</param>
    /// <returns>The result of executing either onMatch or onElse function.</returns>
    public static TOut MatchError<TError, TValue1, TOut>(this Result<TValue1> result, Func<TError, TOut> onMatch,
        Func<TOut> onElse)
        where TError : Error
        where TValue1 : notnull
    {
        return result.GetError<TError>()
            .Match(onMatch, onElse);
    }
}