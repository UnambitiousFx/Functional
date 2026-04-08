using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.AspNetCore.Mappers;

namespace UnambitiousFx.Functional.AspNetCore.Mvc;

/// <summary>
///     Provides extension methods for handling Result objects in the context of ASP.NET Core MVC.
///     These methods convert Result and Result&lt;T&gt; instances to IActionResult, supporting both
///     success and error scenarios with mapping to HTTP status codes.
/// </summary>
public static class ResultHttpExtensions {
    /// <summary>
    ///     Creates a fluent builder for converting a Result to an <see cref="IActionResult" />.
    /// </summary>
    public static ResultMvcBuilder AsActionResultBuilder(this Result         result,
                                                         IFailureHttpMapper? failureMapper = null) {
        return new ResultMvcBuilder(result, failureMapper ?? DefaultFailureHttpMapper.Instance);
    }

    /// <summary>
    ///     Creates a fluent builder for converting an asynchronous Result to an <see cref="IActionResult" />.
    /// </summary>
    public static ResultMvcBuilder AsActionResultBuilder(this ValueTask<Result> resultTask,
                                                         IFailureHttpMapper?    failureMapper = null) {
        return new ResultMvcBuilder(resultTask, failureMapper ?? DefaultFailureHttpMapper.Instance);
    }

    /// <summary>
    ///     Creates a fluent builder for converting a Result&lt;T&gt; to an <see cref="IActionResult" />.
    /// </summary>
    public static ResultMvcBuilder<TValue> AsActionResultBuilder<TValue>(this Result<TValue> result,
                                                                         IFailureHttpMapper? failureMapper = null)
        where TValue : notnull {
        return new ResultMvcBuilder<TValue>(result, failureMapper ?? DefaultFailureHttpMapper.Instance);
    }

    /// <summary>
    ///     Creates a fluent builder for converting an asynchronous Result&lt;T&gt; to an <see cref="IActionResult" />.
    /// </summary>
    public static ResultMvcBuilder<TValue> AsActionResultBuilder<TValue>(this ValueTask<Result<TValue>> resultTask,
                                                                         IFailureHttpMapper?            failureMapper = null)
        where TValue : notnull {
        return new ResultMvcBuilder<TValue>(resultTask, failureMapper ?? DefaultFailureHttpMapper.Instance);
    }
}
