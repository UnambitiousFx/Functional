using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional;

public readonly partial record struct Result
{
    /// <summary>
    ///     Pattern matches the result, executing the appropriate action.
    /// </summary>
    /// <param name="onSuccess">Action to execute if the result is successful.</param>
    /// <param name="onFailure">Action to execute if the result is a failure.</param>
    public void Match(Action onSuccess, Action<Failure> onFailure)
    {
        if (IsSuccess)
        {
            onSuccess();
        }
        else
        {
            onFailure(_error!);
        }
    }

    /// <summary>
    ///     Pattern matches the result, returning a value from the appropriate function.
    /// </summary>
    /// <typeparam name="TOut">The type of value to return.</typeparam>
    /// <param name="onSuccess">Function to invoke if the result is successful.</param>
    /// <param name="onFailure">Function to invoke if the result is a failure.</param>
    /// <returns>The result of invoking the appropriate function.</returns>
    public TOut Match<TOut>(Func<TOut> onSuccess, Func<Failure, TOut> onFailure)
    {
        return IsSuccess ? onSuccess() : onFailure(_error!);
    }
}

public readonly partial record struct Result<TValue>
{
    /// <summary>
    ///     Pattern matches the result, executing the appropriate action.
    /// </summary>
    /// <param name="success">Action to execute if the result is successful.</param>
    /// <param name="failure">Action to execute if the result is a failure.</param>
    public void Match(Action success, Action<Failure> failure)
    {
        if (IsSuccess)
        {
            success();
        }
        else
        {
            failure(_error!);
        }
    }

    /// <summary>
    ///     Pattern matches the result, returning a value from the appropriate function.
    /// </summary>
    /// <typeparam name="TOut">The type of value to return.</typeparam>
    /// <param name="success">Function to invoke if the result is successful.</param>
    /// <param name="failure">Function to invoke if the result is a failure.</param>
    /// <returns>The result of invoking the appropriate function.</returns>
    public TOut Match<TOut>(Func<TOut> success, Func<Failure, TOut> failure)
    {
        return IsSuccess ? success() : failure(_error!);
    }
}