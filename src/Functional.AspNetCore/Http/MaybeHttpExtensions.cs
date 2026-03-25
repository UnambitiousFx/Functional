using IHttpResult = Microsoft.AspNetCore.Http.IResult;
using HttpResults = Microsoft.AspNetCore.Http.Results;

namespace UnambitiousFx.Functional.AspNetCore.Http;

/// <summary>
///     Extension methods for converting Maybe types to HTTP responses (IResult) for minimal APIs.
/// </summary>
public static class MaybeHttpExtensions
{
    private static Func<IHttpResult> BuildDefaultNoneMapper(ResultHttpAdapterPolicy? policy)
    {
        var effectivePolicy = policy ?? ResultHttpAdapterPolicy.Default;
        return effectivePolicy.MaybeNoneBehavior switch
        {
            MaybeNoneHttpBehavior.NoContent => HttpResults.NoContent,
            _                               => () => HttpResults.NotFound()
        };
    }

    /// <summary>
    ///     Converts an asynchronous Maybe value to an <see cref="IHttpResult" />.
    /// </summary>
    public static async ValueTask<IHttpResult> ToHttpResult<TValue>(this ValueTask<Maybe<TValue>> maybeTask,
                                                                    Func<TValue, IHttpResult>?    someHttpMapper = null,
                                                                    Func<IHttpResult>?            noneHttpMapper = null,
                                                                    ResultHttpAdapterPolicy?      policy         = null)
        where TValue : notnull
    {
        var maybe = await maybeTask;
        return maybe.ToHttpResult(someHttpMapper, noneHttpMapper, policy);
    }

    /// <summary>
    ///     Converts an asynchronous Maybe value to a 201 Created response when Some, otherwise to NotFound.
    /// </summary>
    public static async ValueTask<IHttpResult> ToCreatedHttpResult<TValue>(this ValueTask<Maybe<TValue>> maybeTask,
                                                                           Func<TValue, string>          locationFactory,
                                                                           Func<IHttpResult>?            noneHttpMapper = null,
                                                                           ResultHttpAdapterPolicy?      policy         = null)
        where TValue : notnull
    {
        var maybe = await maybeTask;
        return maybe.ToCreatedHttpResult(locationFactory, noneHttpMapper, policy);
    }

    /// <summary>
    ///     Converts an asynchronous Maybe value to an <see cref="IHttpResult" />.
    /// </summary>
    public static ValueTask<IHttpResult> ToHttpResult<TValue>(this Task<Maybe<TValue>>   maybeTask,
                                                              Func<TValue, IHttpResult>? someHttpMapper = null,
                                                              Func<IHttpResult>?         noneHttpMapper = null,
                                                              ResultHttpAdapterPolicy?   policy         = null)
        where TValue : notnull
    {
        return new ValueTask<Maybe<TValue>>(maybeTask).ToHttpResult(someHttpMapper, noneHttpMapper, policy);
    }

    /// <summary>
    ///     Converts an asynchronous Maybe value to a 201 Created response when Some, otherwise to NotFound.
    /// </summary>
    public static ValueTask<IHttpResult> ToCreatedHttpResult<TValue>(this Task<Maybe<TValue>> maybeTask,
                                                                     Func<TValue, string>     locationFactory,
                                                                     Func<IHttpResult>?       noneHttpMapper = null,
                                                                     ResultHttpAdapterPolicy? policy         = null)
        where TValue : notnull
    {
        return new ValueTask<Maybe<TValue>>(maybeTask).ToCreatedHttpResult(locationFactory, noneHttpMapper, policy);
    }

    /// <param name="maybe">The maybe value to convert.</param>
    /// <typeparam name="TValue">The contained value type.</typeparam>
    extension<TValue>(Maybe<TValue> maybe)
        where TValue : notnull
    {
        /// <summary>
        ///     Converts a Maybe value to an <see cref="IHttpResult" />.
        ///     Defaults to 200 OK for Some and 404 NotFound for None.
        /// </summary>
        /// <param name="someHttpMapper">Optional mapper for Some values.</param>
        /// <param name="noneHttpMapper">Optional mapper for None values.</param>
        /// <param name="policy">Optional adapter policy controlling default None behavior.</param>
        /// <returns>The mapped HTTP result.</returns>
        public IHttpResult ToHttpResult(Func<TValue, IHttpResult>? someHttpMapper = null,
                                        Func<IHttpResult>?         noneHttpMapper = null,
                                        ResultHttpAdapterPolicy?   policy         = null)
        {
            someHttpMapper ??= HttpResults.Ok;
            noneHttpMapper ??= BuildDefaultNoneMapper(policy);
            return maybe.Match(someHttpMapper, noneHttpMapper);
        }

        /// <summary>
        ///     Converts a Maybe value to a 201 Created response when Some, otherwise to NotFound.
        /// </summary>
        /// <param name="locationFactory">Function generating the Location URI for Some values.</param>
        /// <param name="noneHttpMapper">Optional mapper for None values.</param>
        /// <param name="policy">Optional adapter policy controlling default None behavior.</param>
        /// <returns>The mapped HTTP result.</returns>
        public IHttpResult ToCreatedHttpResult(Func<TValue, string>     locationFactory,
                                               Func<IHttpResult>?       noneHttpMapper = null,
                                               ResultHttpAdapterPolicy? policy         = null)
        {
            ArgumentNullException.ThrowIfNull(locationFactory);
            noneHttpMapper ??= BuildDefaultNoneMapper(policy);
            return maybe.Match(v => HttpResults.Created(locationFactory(v), v), noneHttpMapper);
        }
    }
}
