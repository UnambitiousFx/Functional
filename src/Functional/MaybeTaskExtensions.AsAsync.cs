namespace UnambitiousFx.Functional;

public static partial class MaybeTaskExtensions
{
    /// <summary>
    /// Converts a <see cref="ValueTask{TResult}" /> that yields a <see cref="Maybe{TValue}" />
    /// into a <see cref="MaybeTask{TValue}" /> for improved composability.
    /// </summary>
    /// <typeparam name="TValue">The type of the value that may be present in the <see cref="Maybe{TValue}" />.</typeparam>
    /// <param name="maybeTask">
    /// A <see cref="ValueTask{TResult}" /> containing a <see cref="Maybe{TValue}" />
    /// representing an optional value that can either be present or absent.
    /// </param>
    /// <returns>
    /// A <see cref="MaybeTask{TValue}" /> wrapping the input <see cref="ValueTask{TResult}" />.
    /// </returns>
    public static MaybeTask<TValue> AsAsync<TValue>(this ValueTask<Maybe<TValue>> maybeTask) where TValue : notnull
    {
        return new MaybeTask<TValue>(maybeTask);
    }
}
