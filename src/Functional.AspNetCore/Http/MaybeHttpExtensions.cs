using IHttpResult = Microsoft.AspNetCore.Http.IResult;

namespace UnambitiousFx.Functional.AspNetCore.Http;

/// <summary>
///     Extension methods for converting Maybe types to HTTP responses (IResult) for minimal APIs.
/// </summary>
public static class MaybeHttpExtensions {
    /// <summary>
    ///     Creates a fluent builder for converting a <see cref="Maybe{TValue}" /> to an <see cref="IHttpResult" /> with
    ///     customizable HTTP response settings.
    /// </summary>
    /// <param name="maybe">The Maybe value to convert.</param>
    /// <typeparam name="TValue">The type of the contained value.</typeparam>
    /// <returns>A <see cref="MaybeHttpBuilder{TValue}" /> for configuring the HTTP response.</returns>
    public static MaybeHttpBuilder<TValue> AsHttpBuilder<TValue>(this Maybe<TValue> maybe)
        where TValue : notnull {
        return new MaybeHttpBuilder<TValue>(maybe);
    }

    /// <summary>
    ///     Creates a fluent builder for converting an asynchronous <see cref="Maybe{TValue}" /> to an
    ///     <see cref="IHttpResult" /> with customizable HTTP response settings.
    /// </summary>
    /// <param name="maybeTask">The asynchronous Maybe value to convert.</param>
    /// <typeparam name="TValue">The type of the contained value.</typeparam>
    /// <returns>A <see cref="MaybeHttpBuilder{TValue}" /> for configuring the HTTP response.</returns>
    public static MaybeHttpBuilder<TValue> AsHttpBuilder<TValue>(this ValueTask<Maybe<TValue>> maybeTask)
        where TValue : notnull {
        return new MaybeHttpBuilder<TValue>(maybeTask);
    }
}
