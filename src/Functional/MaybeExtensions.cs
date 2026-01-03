namespace UnambitiousFx.Functional;

/// <summary>
/// Provides extension methods for working with <see cref="Maybe{TValue}"/> instances.
/// </summary>
public static partial class MaybeExtensions
{
    /// <summary>
    /// Transforms the value inside the Maybe using a function that returns another Maybe, flattening the result.
    /// </summary>
    /// <typeparam name="TIn">The type of the input value.</typeparam>
    /// <typeparam name="TOut">The type of the output value.</typeparam>
    /// <param name="maybe">The Maybe instance to bind.</param>
    /// <param name="bind">The function to apply to the value if present.</param>
    /// <returns>The result of applying the bind function if a value is present; otherwise, None.</returns>
    public static Maybe<TOut> Bind<TIn, TOut>(this Maybe<TIn> maybe,
        Func<TIn, Maybe<TOut>> bind)
        where TIn : notnull
        where TOut : notnull
    {
        return maybe.Match(bind, Maybe.None<TOut>);
    }
}
