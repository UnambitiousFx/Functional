using Microsoft.AspNetCore.Mvc;

namespace UnambitiousFx.Functional.AspNetCore.Mvc;

/// <summary>
///     Extension methods for converting Maybe types to MVC action results.
/// </summary>
public static class MaybeHttpExtensions {
    /// <summary>
    ///     Creates a fluent builder for converting a <see cref="Maybe{TValue}" /> to an <see cref="IActionResult" />.
    /// </summary>
    public static MaybeMvcBuilder<TValue> AsActionResultBuilder<TValue>(this Maybe<TValue> maybe)
        where TValue : notnull {
        return new MaybeMvcBuilder<TValue>(maybe);
    }

    /// <summary>
    ///     Creates a fluent builder for converting an asynchronous <see cref="Maybe{TValue}" /> to an
    ///     <see cref="IActionResult" />.
    /// </summary>
    public static MaybeMvcBuilder<TValue> AsActionResultBuilder<TValue>(this ValueTask<Maybe<TValue>> maybeTask)
        where TValue : notnull {
        return new MaybeMvcBuilder<TValue>(maybeTask);
    }
}
