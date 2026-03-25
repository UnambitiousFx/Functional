namespace UnambitiousFx.Functional;

public static partial class MaybeExtensions
{
    /// <summary>
    ///     LINQ projection operator for <see cref="Maybe{TValue}" />.
    /// </summary>
    /// <typeparam name="TIn">The input value type.</typeparam>
    /// <typeparam name="TOut">The projected value type.</typeparam>
    /// <param name="maybe">The source maybe.</param>
    /// <param name="selector">The projection to apply when a value is present.</param>
    /// <returns>A projected maybe when some; otherwise none.</returns>
    public static Maybe<TOut> Select<TIn, TOut>(this Maybe<TIn> maybe, Func<TIn, TOut> selector)
        where TIn : notnull
        where TOut : notnull
    {
        return maybe.Map(selector);
    }

    /// <summary>
    ///     LINQ bind operator for <see cref="Maybe{TValue}" />.
    /// </summary>
    /// <typeparam name="TIn">The input value type.</typeparam>
    /// <typeparam name="TOut">The bound value type.</typeparam>
    /// <param name="maybe">The source maybe.</param>
    /// <param name="binder">The bind function to apply when a value is present.</param>
    /// <returns>A bound maybe when some; otherwise none.</returns>
    public static Maybe<TOut> SelectMany<TIn, TOut>(this Maybe<TIn> maybe, Func<TIn, Maybe<TOut>> binder)
        where TIn : notnull
        where TOut : notnull
    {
        return maybe.Bind(binder);
    }

    /// <summary>
    ///     LINQ bind operator with projection for <see cref="Maybe{TValue}" />.
    /// </summary>
    /// <typeparam name="TIn">The input value type.</typeparam>
    /// <typeparam name="TCollection">The bound value type.</typeparam>
    /// <typeparam name="TOut">The projected value type.</typeparam>
    /// <param name="maybe">The source maybe.</param>
    /// <param name="binder">The bind function to apply when a value is present.</param>
    /// <param name="projector">The projection to apply when both values are present.</param>
    /// <returns>A projected maybe when some; otherwise none.</returns>
    public static Maybe<TOut> SelectMany<TIn, TCollection, TOut>(
        this Maybe<TIn> maybe,
        Func<TIn, Maybe<TCollection>> binder,
        Func<TIn, TCollection, TOut> projector)
        where TIn : notnull
        where TCollection : notnull
        where TOut : notnull
    {
        return maybe.Bind(left => binder(left).Map(right => projector(left, right)));
    }

    /// <summary>
    ///     LINQ filter operator for <see cref="Maybe{TValue}" />.
    /// </summary>
    /// <typeparam name="TValue">The contained value type.</typeparam>
    /// <param name="maybe">The source maybe.</param>
    /// <param name="predicate">The predicate to evaluate when a value is present.</param>
    /// <returns>The original value when predicate is true; otherwise none.</returns>
    public static Maybe<TValue> Where<TValue>(this Maybe<TValue> maybe, Func<TValue, bool> predicate)
        where TValue : notnull
    {
        return maybe.Filter(predicate);
    }
}