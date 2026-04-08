using UnambitiousFx.Functional.AspNetCore.Mappers;

namespace UnambitiousFx.Functional.AspNetCore.Http;

/// <summary>
///     Extension methods for converting Result types to HTTP responses (IResult) for minimal APIs.
/// </summary>
public static class ResultHttpExtensions
{
    /// <summary>
    ///     Creates a fluent builder for converting a Result to an IResult with customizable HTTP response settings.
    /// </summary>
    /// <param name="result">The result to convert.</param>
    /// <param name="failureMapper">Optional custom failure mapper. Uses default mapper if not provided.</param>
    /// <returns>A <see cref="ResultHttpBuilder" /> for configuring the HTTP response.</returns>
    public static ResultHttpBuilder AsHttpBuilder(this Result         result,
                                                  IFailureHttpMapper? failureMapper = null)
    {
        return new ResultHttpBuilder(result, failureMapper ?? DefaultFailureHttpMapper.Instance);
    }

    /// <summary>
    ///     Creates a fluent builder for converting an asynchronous Result to an IResult with customizable HTTP response
    ///     settings.
    /// </summary>
    public static ResultHttpBuilder AsHttpBuilder(this ValueTask<Result> resultTask,
                                                  IFailureHttpMapper?    failureMapper = null)
    {
        return new ResultHttpBuilder(resultTask, failureMapper ?? DefaultFailureHttpMapper.Instance);
    }

    /// <summary>
    ///     Creates a fluent builder for converting a Result&lt;T&gt; to an IResult with customizable HTTP response settings.
    /// </summary>
    /// <param name="result">The result to convert.</param>
    /// <param name="failureMapper">Optional custom failure mapper. Uses default mapper if not provided.</param>
    /// <typeparam name="TValue">The type of the success value.</typeparam>
    /// <returns>A <see cref="ResultHttpBuilder{TValue}" /> for configuring the HTTP response.</returns>
    public static ResultHttpBuilder<TValue> AsHttpBuilder<TValue>(this Result<TValue> result,
                                                                  IFailureHttpMapper? failureMapper = null)
        where TValue : notnull
    {
        return new ResultHttpBuilder<TValue>(result, failureMapper ?? DefaultFailureHttpMapper.Instance);
    }

    /// <summary>
    ///     Creates a fluent builder for converting an asynchronous Result&lt;T&gt; to an IResult with customizable HTTP
    ///     response settings.
    /// </summary>
    public static ResultHttpBuilder<TValue> AsHttpBuilder<TValue>(this ValueTask<Result<TValue>> resultTask,
                                                                  IFailureHttpMapper?            failureMapper = null)
        where TValue : notnull
    {
        return new ResultHttpBuilder<TValue>(resultTask, failureMapper ?? DefaultFailureHttpMapper.Instance);
    }
}
