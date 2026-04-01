using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for working with asynchronous Result operations.
/// </summary>
public static partial class ResultAsyncExtensions {
    /// <summary>
    ///     Executes the specified action if the encapsulated result is successful, and returns the original result.
    /// </summary>
    /// <param name="resultTask">The ValueTask representing the asynchronous operation of the result.</param>
    /// <param name="tap">An action to execute if the encapsulated result is successful.</param>
    /// <returns>The original result ValueTask encapsulating the value or failure, unchanged.</returns>
    public static async ValueTask<Result> Tap(this ValueTask<Result> resultTask,
                                              Action                 tap) {
        var result = await resultTask;
        return result.Tap(tap);
    }

    /// <summary>
    ///     Executes the specified action or asynchronous operation if the result represents a successful state,
    ///     and returns the original result.
    /// </summary>
    /// <param name="resultTask">The ValueTask representing the asynchronous operation of the result.</param>
    /// <param name="tap">An asynchronous function to invoke if the result represents a successful state.</param>
    /// <returns>The original result Task encapsulating the value or failure, unchanged.</returns>
    public static async ValueTask<Result> Tap(this ValueTask<Result> resultTask,
                                              Func<ValueTask>        tap) {
        var result = await resultTask;
        if (result.IsSuccess) {
            await tap();
        }

        return result;
    }

    /// <summary>
    ///     Executes the specified action if the result encapsulates a failure, and returns the original result.
    /// </summary>
    /// <param name="resultTask">The ValueTask representing the asynchronous operation of the result.</param>
    /// <param name="tap">An action to execute if the result encapsulates a failure.</param>
    /// <returns>The original result Task encapsulating the value or failure, unchanged.</returns>
    public static async ValueTask<Result> TapFailure(this ValueTask<Result> resultTask,
                                                     Action<Failure>        tap) {
        var result = await resultTask;
        return result.TapFailure(tap);
    }

    /// <summary>
    ///     Executes the specified asynchronous operation if the encapsulated result represents a failure,
    ///     passing the failure value to the operation, and returns the original result.
    /// </summary>
    /// <param name="resultTask">The ValueTask representing the asynchronous operation of the result.</param>
    /// <param name="tap">
    ///     A function that takes a failure as an argument and returns a ValueTask to execute
    ///     when the encapsulated result represents a failure.
    /// </param>
    /// <returns>The original result Task encapsulating the value or failure, unchanged.</returns>
    public static async ValueTask<Result> TapFailure(this ValueTask<Result>   resultTask,
                                                     Func<Failure, ValueTask> tap) {
        var result = await resultTask;
        if (result.TryGetFailure(out var error)) {
            await tap(error);
        }

        return result;
    }

    /// <summary>
    ///     Executes the specified action or asynchronous operation on the encapsulated value
    ///     if the result is successful, and returns the original result.
    /// </summary>
    /// <typeparam name="TIn">The type of the value encapsulated by the result.</typeparam>
    /// <param name="resultTask">The ValueTask representing the asynchronous operation of the result.</param>
    /// <param name="tap">
    ///     An action or a function returning a ValueTask to execute on the encapsulated value if the result is
    ///     successful.
    /// </param>
    /// <returns>The original result Task encapsulating the value or failure, unchanged.</returns>
    public static async ValueTask<Result<TIn>> Tap<TIn>(this ValueTask<Result<TIn>> resultTask,
                                                        Action<TIn>                 tap)
        where TIn : notnull {
        var result = await resultTask;
        return result.Tap(tap);
    }

    /// <summary>
    ///     Executes the specified action or asynchronous operation and returns the original result.
    /// </summary>
    /// <param name="resultTask">The ValueTask representing the asynchronous operation of the result.</param>
    /// <param name="tap">An action or a function that returns a ValueTask to invoke regardless of the result state.</param>
    /// <returns>The original result task encapsulating the value or failure, unchanged.</returns>
    public static async ValueTask<Result<TIn>> Tap<TIn>(this ValueTask<Result<TIn>> resultTask,
                                                        Func<TIn, ValueTask>        tap)
        where TIn : notnull {
        var result = await resultTask;
        if (result.TryGetValue(out var value)) {
            await tap(value);
        }

        return result;
    }

    /// <summary>
    ///     Executes the specified action or asynchronous operation when invoked
    ///     and returns the original result encapsulated within the ValueTask.
    /// </summary>
    /// <typeparam name="TIn">The type of the encapsulated value within the result.</typeparam>
    /// <param name="resultTask">The ValueTask representing the original asynchronous result.</param>
    /// <param name="tap">An action to execute or a function that returns a ValueTask to invoke.</param>
    /// <returns>The original result encapsulated within a ValueTask, unchanged.</returns>
    public static async ValueTask<Result<TIn>> Tap<TIn>(this ValueTask<Result<TIn>> resultTask,
                                                        Action                      tap)
        where TIn : notnull {
        var result = await resultTask;
        return result.Tap(tap);
    }

    /// <summary>
    ///     Executes the specified action or asynchronous operation for the encapsulated value
    ///     within the result, if the result indicates success, and returns the original result.
    /// </summary>
    /// <typeparam name="TIn">The type of the value encapsulated by the result.</typeparam>
    /// <param name="resultTask">The ValueTask representing the asynchronous operation of the result.</param>
    /// <param name="tap">
    ///     An action to execute or a function that returns a ValueTask to invoke if the result indicates
    ///     success.
    /// </param>
    /// <returns>The original result Task encapsulating the value or failure, unchanged.</returns>
    public static async ValueTask<Result<TIn>> Tap<TIn>(this ValueTask<Result<TIn>> resultTask,
                                                        Func<ValueTask>             tap)
        where TIn : notnull {
        var result = await resultTask;
        if (result.IsSuccess) {
            await tap();
        }

        return result;
    }

    /// <summary>
    ///     Executes the specified action or asynchronous operation if the predicate evaluates to true
    ///     for the value encapsulated by the result, and returns the original result.
    /// </summary>
    /// <typeparam name="TIn">The type of the value encapsulated by the result.</typeparam>
    /// <param name="resultTask">The ValueTask representing the asynchronous operation of the result.</param>
    /// <param name="predicate">
    ///     A function that evaluates whether the action or asynchronous operation should be executed,
    ///     based on the encapsulated value.
    /// </param>
    /// <param name="tap">
    ///     An action to execute or a function that returns a ValueTask to invoke if the predicate evaluates to
    ///     true.
    /// </param>
    /// <returns>The original result Task encapsulating the value or failure, unchanged.</returns>
    public static async ValueTask<Result<TIn>> TapIf<TIn>(this ValueTask<Result<TIn>> resultTask,
                                                          Func<TIn, bool>             predicate,
                                                          Action<TIn>                 tap)
        where TIn : notnull {
        var result = await resultTask;
        return result.TapIf(predicate, tap);
    }

    /// <summary>
    ///     Executes the specified action asynchronously if the result encapsulates a success value and the given predicate
    ///     evaluates to true.
    ///     This method does not modify the result, allowing for conditional execution of additional logic in a functional way.
    /// </summary>
    /// <typeparam name="TIn">The type of the value encapsulated by the result.</typeparam>
    /// <param name="resultTask">The ValueTask representing the asynchronous operation of the result.</param>
    /// <param name="predicate">A function to determine whether the action should be executed, based on the encapsulated value.</param>
    /// <param name="tap">
    ///     An asynchronous function to execute if the predicate evaluates to true. The success value will be
    ///     passed as a parameter.
    /// </param>
    /// <returns>The original result Task encapsulating the value or failure, unchanged.</returns>
    public static async ValueTask<Result<TIn>> TapIf<TIn>(this ValueTask<Result<TIn>> resultTask,
                                                          Func<TIn, bool>             predicate,
                                                          Func<TIn, ValueTask>        tap)
        where TIn : notnull {
        var result = await resultTask;
        if (result.TryGetValue(out var value) &&
            predicate(value)) {
            await tap(value);
        }

        return result;
    }

    /// <summary>
    ///     Executes the specified action conditionally based on the provided predicate, if the result encapsulates a value.
    ///     This method enables conditional execution of side effects when the embedded value matches the specified condition.
    /// </summary>
    /// <typeparam name="TIn">The type of the value encapsulated by the result.</typeparam>
    /// <param name="resultTask">The ValueTask representing the asynchronous operation of the result.</param>
    /// <param name="predicate">A function that defines the condition to evaluate on the encapsulated value.</param>
    /// <param name="tap">An action to execute if the predicate evaluates to true for the encapsulated value.</param>
    /// <returns>The original result Task encapsulating the value, unchanged.</returns>
    public static async ValueTask<Result<TIn>> TapIf<TIn>(this ValueTask<Result<TIn>> resultTask,
                                                          Func<TIn, bool>             predicate,
                                                          Action                      tap)
        where TIn : notnull {
        var result = await resultTask;
        return result.TapIf(predicate, tap);
    }

    /// <summary>
    ///     Executes the specified action if the result encapsulates a value and the predicate evaluates to true.
    ///     This method allows conditional tapping into the result value while leaving the result unchanged.
    /// </summary>
    /// <typeparam name="TIn">The type of the value encapsulated by the result.</typeparam>
    /// <param name="resultTask">The ValueTask representing the asynchronous operation of the result.</param>
    /// <param name="predicate">
    ///     A function that evaluates the encapsulated value and determines whether the action should be
    ///     executed.
    /// </param>
    /// <param name="tap">
    ///     An asynchronous function to execute if the predicate evaluates to true. The encapsulated value will
    ///     be passed as a parameter.
    /// </param>
    /// <returns>A ValueTask encapsulating the original result, unchanged.</returns>
    public static async ValueTask<Result<TIn>> TapIf<TIn>(this ValueTask<Result<TIn>> resultTask,
                                                          Func<TIn, bool>             predicate,
                                                          Func<ValueTask>             tap)
        where TIn : notnull {
        var result = await resultTask;
        if (result.TryGetValue(out var value) &&
            predicate(value)) {
            await tap();
        }

        return result;
    }

    /// <summary>
    ///     Executes the specified action if the result encapsulates a failure and returns the original result.
    ///     This method does not modify the result, allowing you to handle failures in a functional way.
    /// </summary>
    /// <typeparam name="TIn">The type of the value encapsulated by the result.</typeparam>
    /// <param name="resultTask">The ValueTask representing the asynchronous operation of the result.</param>
    /// <param name="tap">An action to execute if the result is a failure. The failure instance will be passed as a parameter.</param>
    /// <returns>The original result Task encapsulating the value or failure, unchanged.</returns>
    public static async ValueTask<Result<TIn>> TapFailure<TIn>(this ValueTask<Result<TIn>> resultTask,
                                                               Action<Failure>             tap)
        where TIn : notnull {
        var result = await resultTask;
        return result.TapFailure(tap);
    }

    /// <summary>
    ///     Executes the specified asynchronous function if the result represents a failure.
    ///     Returns the original result regardless of whether the function was invoked or not.
    /// </summary>
    /// <typeparam name="TIn">The type of the value contained in the result, if any.</typeparam>
    /// <param name="resultTask">A task representing the result to inspect.</param>
    /// <param name="tap">
    ///     An asynchronous function to execute when the result is a failure.
    ///     It receives the failure as its parameter.
    /// </param>
    /// <returns>A task that represents the asynchronous operation, containing the original result.</returns>
    public static async ValueTask<Result<TIn>> TapFailure<TIn>(this ValueTask<Result<TIn>> resultTask,
                                                               Func<Failure, ValueTask>    tap)
        where TIn : notnull {
        var result = await resultTask;
        if (result.TryGetFailure(out var error)) {
            await tap(error);
        }

        return result;
    }
}
