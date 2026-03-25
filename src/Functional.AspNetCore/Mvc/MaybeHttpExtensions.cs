using Microsoft.AspNetCore.Mvc;

namespace UnambitiousFx.Functional.AspNetCore.Mvc;

/// <summary>
///     Extension methods for converting Maybe types to MVC action results.
/// </summary>
public static class MaybeHttpExtensions
{
    private static Func<IActionResult> BuildDefaultNoneMapper(ResultHttpAdapterPolicy? policy)
    {
        var effectivePolicy = policy ?? ResultHttpAdapterPolicy.Default;
        return effectivePolicy.MaybeNoneBehavior switch
        {
            MaybeNoneHttpBehavior.NoContent => () => new NoContentResult(),
            _                               => () => new NotFoundResult()
        };
    }

    /// <summary>
    ///     Converts an asynchronous Maybe value to an <see cref="IActionResult" />.
    /// </summary>
    public static async ValueTask<IActionResult> ToActionResult<TValue>(this ValueTask<Maybe<TValue>> maybeTask,
                                                                        Func<TValue, IActionResult>?  someHttpMapper = null,
                                                                        Func<IActionResult>?          noneHttpMapper = null,
                                                                        ResultHttpAdapterPolicy?      policy         = null)
        where TValue : notnull
    {
        var maybe = await maybeTask;
        return maybe.ToActionResult(someHttpMapper, noneHttpMapper, policy);
    }

    /// <summary>
    ///     Converts an asynchronous Maybe value to an <see cref="IActionResult" />.
    /// </summary>
    public static ValueTask<IActionResult> ToActionResult<TValue>(this Task<Maybe<TValue>>     maybeTask,
                                                                  Func<TValue, IActionResult>? someHttpMapper = null,
                                                                  Func<IActionResult>?         noneHttpMapper = null,
                                                                  ResultHttpAdapterPolicy?     policy         = null)
        where TValue : notnull
    {
        return new ValueTask<Maybe<TValue>>(maybeTask).ToActionResult(someHttpMapper, noneHttpMapper, policy);
    }

    /// <param name="maybe">The maybe value to convert.</param>
    /// <typeparam name="TValue">The contained value type.</typeparam>
    extension<TValue>(Maybe<TValue> maybe)
        where TValue : notnull
    {
        /// <summary>
        ///     Converts a Maybe value to an <see cref="IActionResult" />.
        ///     Defaults to 200 OK for Some and 404 NotFound for None.
        /// </summary>
        /// <param name="someHttpMapper">Optional mapper for Some values.</param>
        /// <param name="noneHttpMapper">Optional mapper for None values.</param>
        /// <param name="policy">Optional adapter policy controlling default None behavior.</param>
        /// <returns>The mapped action result.</returns>
        public IActionResult ToActionResult(Func<TValue, IActionResult>? someHttpMapper = null,
                                            Func<IActionResult>?         noneHttpMapper = null,
                                            ResultHttpAdapterPolicy?     policy         = null)
        {
            someHttpMapper ??= v => new OkObjectResult(v);
            noneHttpMapper ??= BuildDefaultNoneMapper(policy);
            return maybe.Match(someHttpMapper, noneHttpMapper);
        }
    }
}
