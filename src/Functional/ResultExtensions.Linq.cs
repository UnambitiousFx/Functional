using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    /// <summary>
    ///     LINQ projection operator for <see cref="Result{TValue}" />.
    /// </summary>
    /// <typeparam name="TIn">The input success type.</typeparam>
    /// <typeparam name="TOut">The projected success type.</typeparam>
    /// <param name="result">The source result.</param>
    /// <param name="selector">The projection to apply when successful.</param>
    /// <returns>A projected result when successful; otherwise the original failure.</returns>
    public static Result<TOut> Select<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> selector)
        where TIn : notnull
        where TOut : notnull
    {
        return result.Map(selector);
    }

    /// <summary>
    ///     LINQ bind operator for <see cref="Result{TValue}" />.
    /// </summary>
    /// <typeparam name="TIn">The input success type.</typeparam>
    /// <typeparam name="TOut">The bound success type.</typeparam>
    /// <param name="result">The source result.</param>
    /// <param name="binder">The bind function to apply when successful.</param>
    /// <returns>A bound result when successful; otherwise the original failure.</returns>
    public static Result<TOut> SelectMany<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> binder)
        where TIn : notnull
        where TOut : notnull
    {
        return result.Bind(binder);
    }

    /// <summary>
    ///     LINQ bind operator with projection for <see cref="Result{TValue}" />.
    /// </summary>
    /// <typeparam name="TIn">The input success type.</typeparam>
    /// <typeparam name="TCollection">The bound success type.</typeparam>
    /// <typeparam name="TOut">The projected output success type.</typeparam>
    /// <param name="result">The source result.</param>
    /// <param name="binder">The bind function to apply when successful.</param>
    /// <param name="projector">The projection to apply when both results are successful.</param>
    /// <returns>A projected result when successful; otherwise the original failure.</returns>
    public static Result<TOut> SelectMany<TIn, TCollection, TOut>(
        this Result<TIn> result,
        Func<TIn, Result<TCollection>> binder,
        Func<TIn, TCollection, TOut> projector)
        where TIn : notnull
        where TCollection : notnull
        where TOut : notnull
    {
        return result.Bind(left => binder(left).Map(right => projector(left, right)));
    }

    /// <summary>
    ///     LINQ filter operator for <see cref="Result{TValue}" />.
    /// </summary>
    /// <typeparam name="TValue">The success type.</typeparam>
    /// <param name="result">The source result.</param>
    /// <param name="predicate">The predicate to evaluate on success values.</param>
    /// <returns>
    ///     The original success when the predicate is true; otherwise a validation failure.
    /// </returns>
    public static Result<TValue> Where<TValue>(this Result<TValue> result, Func<TValue, bool> predicate)
        where TValue : notnull
    {
        return result.Ensure(
            predicate,
            _ => new ValidationFailure("Result.Where predicate returned false."));
    }
}