namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for working with asynchronous Result operations.
/// </summary>
public static partial class ResultAsyncExtensions {
    /// <summary>
    ///     Asynchronously applies a binding function to the successful result of a <see cref="ValueTask{T}" />
    ///     containing a <see cref="Result{TIn}" />, transforming it into a new <see cref="Result{TOut}" />.
    /// </summary>
    /// <typeparam name="TIn">The type of the input value in the result.</typeparam>
    /// <typeparam name="TOut">The type of the output value in the transformed result.</typeparam>
    /// <param name="resultTask">The asynchronous task containing the result to bind.</param>
    /// <param name="bind">The binding function to apply to the successful result.</param>
    /// <returns>
    ///     A task representing the asynchronous operation, containing the transformed result.
    /// </returns>
    public static async ValueTask<Result<TOut>> Bind<TIn, TOut>(this ValueTask<Result<TIn>> resultTask,
                                                                Func<TIn, Result<TOut>>     bind)
        where TIn : notnull
        where TOut : notnull {
        var result = await resultTask;
        return result.Bind(bind);
    }

    /// <summary>
    ///     Chains the execution of an asynchronous Result computation with another asynchronous function
    ///     that produces a Result, enabling fluent error handling and continuation of operations.
    /// </summary>
    /// <typeparam name="TIn">The type of the value contained in the input Result.</typeparam>
    /// <typeparam name="TOut">The type of the value contained in the output Result.</typeparam>
    /// <param name="resultTask">The task that resolves to a Result containing the input value.</param>
    /// <param name="bind">
    ///     A function to invoke when the input Result is successful, which returns
    ///     an asynchronous Result containing the output value.
    /// </param>
    /// <returns>
    ///     A task that resolves to a Result containing the output value if the input Result was successful,
    ///     or propagates the failure from the input Result.
    /// </returns>
    public static async ValueTask<Result<TOut>> Bind<TIn, TOut>(this ValueTask<Result<TIn>>        resultTask,
                                                                Func<TIn, ValueTask<Result<TOut>>> bind)
        where TIn : notnull
        where TOut : notnull {
        var result = await resultTask;
        if (!result.TryGetValue(out var value)) {
            result.TryGetFailure(out var error);
            return Result.Failure<TOut>(error!);
        }

        return await bind(value);
    }

    /// <summary>
    ///     Asynchronously applies a binding function to a <see cref="ValueTask{TResult}" />
    ///     containing a <see cref="Result{TIn}" />, transforming it into a new <see cref="Result" />.
    /// </summary>
    /// <typeparam name="TIn">The type of the input value within the initial result.</typeparam>
    /// <param name="resultTask">The asynchronous task containing the initial <see cref="Result{TIn}" />.</param>
    /// <param name="bind">
    ///     The binding function to apply to the input value. It accepts a value of type
    ///     <typeparamref name="TIn" /> and returns a <see cref="Result" />.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation, containing the resulting <see cref="Result" />
    ///     after applying the binding function.
    /// </returns>
    public static async ValueTask<Result> Bind<TIn>(this ValueTask<Result<TIn>> resultTask,
                                                    Func<TIn, Result>           bind)
        where TIn : notnull {
        var result = await resultTask;
        return result.Bind(bind);
    }

    /// <summary>
    ///     Asynchronously applies a binding function to the successful result of a
    ///     <see cref="ValueTask{T}" /> containing a <see cref="Result{TIn}" />,
    ///     transforming it into a new <see cref="Result" />.
    /// </summary>
    /// <typeparam name="TIn">The type of the input value in the result.</typeparam>
    /// <param name="resultTask">The asynchronous task containing the result to bind.</param>
    /// <param name="bind">The binding function to apply to the successful result.</param>
    /// <returns>
    ///     A task representing the asynchronous operation, containing the transformed result.
    /// </returns>
    public static async ValueTask<Result> Bind<TIn>(this ValueTask<Result<TIn>>  resultTask,
                                                    Func<TIn, ValueTask<Result>> bind)
        where TIn : notnull {
        var result = await resultTask;
        if (!result.TryGetValue(out var value)) {
            result.TryGetFailure(out var error);
            return Result.Failure(error!);
        }

        return await bind(value);
    }

    /// <summary>
    ///     Asynchronously applies a binding function to a task containing a <see cref="Result{T}" />,
    ///     and returns a new <see cref="Result" /> based on the output of the binding function.
    /// </summary>
    /// <param name="resultTask">The task containing the initial result to bind.</param>
    /// <param name="bind">A function to transform the result value into a new <see cref="Result" />.</param>
    /// <returns>
    ///     A task representing the asynchronous operation, containing the transformed <see cref="Result" />.
    /// </returns>
    public static async ValueTask<Result> Bind(this ValueTask<Result> resultTask,
                                               Func<Result>           bind) {
        var result = await resultTask;
        return result.Bind(bind);
    }

    /// <summary>
    ///     Binds the result of the asynchronous operation, executing the provided function
    ///     if the original result is successful. If the original result is a failure,
    ///     the failure is returned without executing the provided function.
    /// </summary>
    /// <param name="resultTask">A ValueTask representing the original asynchronous operation.</param>
    /// <param name="bind">A function to bind and process the result if the original operation is successful.</param>
    /// <returns>
    ///     A ValueTask containing the resulting <see cref="Result" /> from either the execution of
    ///     the bind function or the original failure.
    /// </returns>
    public static async ValueTask<Result> Bind(this ValueTask<Result>  resultTask,
                                               Func<ValueTask<Result>> bind) {
        var result = await resultTask;
        if (result.IsFailure) {
            return result;
        }

        return await bind();
    }

    /// <summary>
    ///     Binds a function to the result of an asynchronous operation and returns a new asynchronous result.
    /// </summary>
    /// <typeparam name="TOut">The type of the output value contained in the new result.</typeparam>
    /// <param name="resultTask">The asynchronous task containing the result to bind.</param>
    /// <param name="bind">The function to apply to the result, returning a new result.</param>
    /// <returns>
    ///     A task representing the asynchronous operation, containing the result of the bind function application.
    /// </returns>
    public static async ValueTask<Result<TOut>> Bind<TOut>(this ValueTask<Result> resultTask,
                                                           Func<Result<TOut>>     bind)
        where TOut : notnull {
        var result = await resultTask;
        return result.Bind(bind);
    }

    /// <summary>
    ///     Binds an asynchronous operation to a new result-generating function.
    /// </summary>
    /// <typeparam name="TOut">The type of the value in the resulting <see cref="Result{TValue}" />.</typeparam>
    /// <param name="resultTask">The original asynchronous operation that provides a <see cref="Result" />.</param>
    /// <param name="bind">
    ///     A function that transforms the result from the original operation into a new <see cref="Result{TValue}" />
    ///     or an asynchronous operation producing a <see cref="Result{TValue}" />.
    /// </param>
    /// <returns>
    ///     A <see cref="ValueTask{TResult}" /> representing the combined asynchronous operation, which results in the new
    ///     <see cref="Result{TValue}" />.
    /// </returns>
    public static async ValueTask<Result<TOut>> Bind<TOut>(this ValueTask<Result>        resultTask,
                                                           Func<ValueTask<Result<TOut>>> bind)
        where TOut : notnull {
        var result = await resultTask;
        if (result.IsFailure) {
            result.TryGetFailure(out var error);
            return Result.Failure<TOut>(error!);
        }

        return await bind();
    }
}
