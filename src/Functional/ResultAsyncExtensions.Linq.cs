using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for performing LINQ operations and transformations
///     on asynchronous <see cref="Result{TValue}" /> values.
/// </summary>
public static partial class ResultAsyncExtensions {
    /// <summary>
    ///     Asynchronously transforms the value contained within a <see cref="Result{TIn}" /> into a new
    ///     <see cref="Result{TOut}" />
    ///     using the provided mapping function.
    /// </summary>
    /// <typeparam name="TIn">The type of the input value within the source <see cref="Result{TIn}" />.</typeparam>
    /// <typeparam name="TOut">The type of the value within the output <see cref="Result{TOut}" />.</typeparam>
    /// <param name="resultTask">A task representing the source <see cref="Result{TIn}" />.</param>
    /// <param name="selector">
    ///     The function to apply to the value contained within the <see cref="Result{TIn}" />
    ///     to produce a value of type <typeparamref name="TOut" />.
    /// </param>
    /// <returns>An asynchronous operation producing a <see cref="Result{TOut}" /> with the transformed value.</returns>
    public static async ValueTask<Result<TOut>> Select<TIn, TOut>(this ValueTask<Result<TIn>> resultTask,
                                                                  Func<TIn, TOut>             selector)
        where TIn : notnull
        where TOut : notnull {
        var result = await resultTask;
        return result.Map(selector);
    }

    /// <summary>
    ///     Projects the result of a task containing a <see cref="Result{TIn}" /> into a new task containing
    ///     a <see cref="Result{TOut}" /> by applying a transformation function to the value of the result.
    /// </summary>
    /// <typeparam name="TIn">The type of the input value contained in the result.</typeparam>
    /// <typeparam name="TOut">The type of the output value produced by the selector function.</typeparam>
    /// <param name="resultTask">The task that contains the <see cref="Result{TIn}" /> to transform.</param>
    /// <param name="selector">A function to transform the input value into an output value.</param>
    /// <returns>
    ///     A task that contains a <see cref="Result{TOut}" />. If the input result is successful,
    ///     the output contains the transformed value. If the input result is a failure, the output remains a failure with the
    ///     same error.
    /// </returns>
    public static async ValueTask<Result<TOut>> Select<TIn, TOut>(this ValueTask<Result<TIn>> resultTask,
                                                                  Func<TIn, ValueTask<TOut>>  selector)
        where TIn : notnull
        where TOut : notnull {
        var result = await resultTask;
        if (!result.TryGetValue(out var value)) {
            result.TryGetFailure(out var error);
            return Result.Failure<TOut>(error!);
        }

        return Result.Success(await selector(value));
    }

    /// <summary>
    ///     Applies a function to the value contained in a successful asynchronous Result instance,
    ///     and flattens the resulting nested Result into a single Result instance.
    /// </summary>
    /// <typeparam name="TIn">The type of the input value contained in the original Result.</typeparam>
    /// <typeparam name="TOut">The type of the value contained in the resulting Result.</typeparam>
    /// <param name="resultTask">The asynchronous Result instance to be transformed.</param>
    /// <param name="binder">The function to apply to the value. It must return a Result.</param>
    /// <returns>
    ///     A flattened asynchronous Result instance after applying the transformation,
    ///     or an error if the original Result is in an error state.
    /// </returns>
    public static async ValueTask<Result<TOut>> SelectMany<TIn, TOut>(this ValueTask<Result<TIn>> resultTask,
                                                                      Func<TIn, Result<TOut>>     binder)
        where TIn : notnull
        where TOut : notnull {
        var result = await resultTask;
        return result.Bind(binder);
    }

    /// <summary>
    ///     Applies an asynchronous transformation function to the value encapsulated in a
    ///     <see cref="Result{TIn}" />, flattening the result into a single asynchronous <see cref="Result{TOut}" />.
    /// </summary>
    /// <typeparam name="TIn">The type of the input value in the original <see cref="Result{TIn}" />.</typeparam>
    /// <typeparam name="TOut">The type of the output value in the resulting <see cref="Result{TOut}" />.</typeparam>
    /// <param name="resultTask">An asynchronous operation producing a <see cref="Result{TIn}" />.</param>
    /// <param name="binder">
    ///     A function to transform the input value into an asynchronous <see cref="Result{TOut}" />.
    /// </param>
    /// <returns>
    ///     A <see cref="ValueTask{TResult}" /> producing a <see cref="Result{TOut}" /> that represents the
    ///     result of applying the transformation function to the input value, or propagates the failure if
    ///     the original result is not successful.
    /// </returns>
    public static async ValueTask<Result<TOut>> SelectMany<TIn, TOut>(this ValueTask<Result<TIn>>        resultTask,
                                                                      Func<TIn, ValueTask<Result<TOut>>> binder)
        where TIn : notnull
        where TOut : notnull {
        var result = await resultTask;
        if (!result.TryGetValue(out var value)) {
            result.TryGetFailure(out var error);
            return Result.Failure<TOut>(error!);
        }

        return await binder(value);
    }

    /// <summary>
    ///     Projects each element of a <see cref="Result{TValue}" /> into a new form by incorporating
    ///     intermediate results and combining them using a specified projection function.
    /// </summary>
    /// <typeparam name="TIn">The type of the input value.</typeparam>
    /// <typeparam name="TCollection">The type of the intermediate result value.</typeparam>
    /// <typeparam name="TOut">The type of the result value of the projection.</typeparam>
    /// <param name="resultTask">
    ///     A <see cref="ValueTask{TResult}" /> containing a <see cref="Result{TValue}" /> to process.
    /// </param>
    /// <param name="binder">
    ///     A function to generate a <see cref="Result{TValue}" /> for the intermediate result based on the value of the input
    ///     result.
    /// </param>
    /// <param name="projector">
    ///     A function that projects the input value and intermediate result into the final result.
    /// </param>
    /// <returns>
    ///     A <see cref="ValueTask{TResult}" /> containing a <see cref="Result{TValue}" /> with the final projected value,
    ///     or an error state if any part of the operation fails.
    /// </returns>
    public static async ValueTask<Result<TOut>> SelectMany<TIn, TCollection, TOut>(this ValueTask<Result<TIn>>    resultTask,
                                                                                   Func<TIn, Result<TCollection>> binder,
                                                                                   Func<TIn, TCollection, TOut>   projector)
        where TIn : notnull
        where TCollection : notnull
        where TOut : notnull {
        var result = await resultTask;
        return result.Bind(left => binder(left)
                              .Map(right => projector(left, right)));
    }

    /// <summary>
    ///     Projects the result of applying a binder function to the value
    ///     contained within the asynchronous result, and then applies a
    ///     projector function to combine the original value and the result
    ///     of the binder function. Returns a new asynchronous result containing the projection.
    /// </summary>
    /// <typeparam name="TIn">The type of the input value contained within the original result.</typeparam>
    /// <typeparam name="TCollection">The type of the intermediate result produced by the binder function.</typeparam>
    /// <typeparam name="TOut">The type of the final result after applying the projector function.</typeparam>
    /// <param name="resultTask">The asynchronous result to be transformed.</param>
    /// <param name="binder">
    ///     A function that takes the input value and returns an asynchronous result of an intermediate
    ///     collection.
    /// </param>
    /// <param name="projector">
    ///     A function that takes the original value and the intermediate collection and produces the final
    ///     result.
    /// </param>
    /// <returns>A new asynchronous result containing the projected value.</returns>
    public static async ValueTask<Result<TOut>> SelectMany<TIn, TCollection, TOut>(this ValueTask<Result<TIn>>               resultTask,
                                                                                   Func<TIn, ValueTask<Result<TCollection>>> binder,
                                                                                   Func<TIn, TCollection, TOut>              projector)
        where TIn : notnull
        where TCollection : notnull
        where TOut : notnull {
        var result = await resultTask;
        if (!result.TryGetValue(out var left)) {
            result.TryGetFailure(out var error);
            return Result.Failure<TOut>(error!);
        }

        var collectionResult = await binder(left);
        return collectionResult.Map(right => projector(left, right));
    }

    /// <summary>
    ///     Filters the asynchronous result based on the specified predicate.
    /// </summary>
    /// <typeparam name="TValue">The type of the value within the result.</typeparam>
    /// <param name="resultTask">The asynchronous result to evaluate.</param>
    /// <param name="predicate">A function to test each value for a condition.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains
    ///     the filtered result if the predicate is satisfied; otherwise, it returns a failed result.
    /// </returns>
    public static async ValueTask<Result<TValue>> Where<TValue>(this ValueTask<Result<TValue>> resultTask,
                                                                Func<TValue, bool>             predicate)
        where TValue : notnull {
        var result = await resultTask;
        return result.Where(predicate);
    }

    /// <summary>
    ///     Filters the result of a <see cref="ValueTask{TResult}" /> based on a predicate function.
    ///     If the result contains a value and the predicate returns false, a failure is returned.
    /// </summary>
    /// <typeparam name="TValue">The type of the value contained within the <see cref="Result{TValue}" />.</typeparam>
    /// <param name="resultTask">The asynchronous result to be processed.</param>
    /// <param name="predicate">An asynchronous function that determines whether the value satisfies a condition.</param>
    /// <returns>
    ///     A <see cref="ValueTask{TResult}" /> that resolves to the original result if the predicate returns true,
    ///     or a failure result with a validation error if the predicate returns false.
    /// </returns>
    public static async ValueTask<Result<TValue>> Where<TValue>(this ValueTask<Result<TValue>> resultTask,
                                                                Func<TValue, ValueTask<bool>>  predicate)
        where TValue : notnull {
        var result = await resultTask;
        if (!result.TryGetValue(out var value)) {
            return result;
        }

        return await predicate(value)
                   ? result
                   : Result.Failure<TValue>(new ValidationFailure("Result.Where predicate returned false."));
    }
}
