using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Executes different side effects based on success or failure, then returns the original result.
    /// </summary>
    /// <param name="result">The result instance.</param>
    /// <param name="onSuccess">Action to execute on success.</param>
    /// <param name="onFailure">Action to execute on failure.</param>
    /// <returns>The original result unchanged.</returns>
    public static Result TapBoth(this Result result, Action onSuccess, Action<Error> onFailure)
    {
        result.Match(onSuccess, onFailure);
        return result;
    }

    /// <summary>
    ///     Executes different side effects based on success or failure, then returns the original result.
    /// </summary>
    /// <typeparam name="TValue1">Value type 1.</typeparam>
    /// <param name="result">The result instance.</param>
    /// <param name="onSuccess">Action to execute on success.</param>
    /// <param name="onFailure">Action to execute on failure.</param>
    /// <returns>The original result unchanged.</returns>
    public static Result<TValue1> TapBoth<TValue1>(this Result<TValue1> result, Action<TValue1> onSuccess,
        Action<Error> onFailure) where TValue1 : notnull
    {
        result.Match(onSuccess, onFailure);
        return result;
    }
}