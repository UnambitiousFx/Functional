namespace UnambitiousFx.Functional;

public static partial class ResultTaskExtensions
{
    /// <summary>
    ///     Transforms the result into a new success result using the specified mapping function.
    /// </summary>
    /// <typeparam name="TOut">The type of the output value.</typeparam>
    /// <param name="result">The current result instance.</param>
    /// <param name="map">The mapping function to be applied.</param>
    /// <returns>A new result containing the transformed value.</returns>
    public static ResultTask<TOut> Map<TOut>(this ResultTask result, Func<TOut> map) where TOut : notnull
    {
        return MapCore(result, map).AsAsync();

        static async ValueTask<Result<TOut>> MapCore(ResultTask self, Func<TOut> map)
        {
            var source = await self;
            return source.Match(
                () => Result.Success(map()).WithMetadata(source.Metadata),
                error => Result.Failure<TOut>(error).WithMetadata(source.Metadata));
        }
    }

    /// <summary>
    ///     Transforms the result of the asynchronous operation into a new result using a specified mapping function.
    /// </summary>
    /// <typeparam name="TOut">The type of the output value.</typeparam>
    /// <param name="result">The current result instance to be transformed.</param>
    /// <param name="map">The mapping function to transform the result.</param>
    /// <returns>A new result containing the transformed value.</returns>
    public static ResultTask<TOut> Map<TOut>(this ResultTask result, Func<ValueTask<TOut>> map) where TOut : notnull
    {
        return MapCore(result, map).AsAsync();

        static async ValueTask<Result<TOut>> MapCore(ResultTask self, Func<ValueTask<TOut>> map)
        {
            var source = await self;
            return await source.Match<ValueTask<Result<TOut>>>(
                async () =>
                {
                    var mapped = await map();
                    return Result.Success(mapped).WithMetadata(source.Metadata);
                },
                error => ValueTask.FromResult(Result.Failure<TOut>(error).WithMetadata(source.Metadata)));
        }
    }

    /// <summary>
    ///     Transforms the current result into a new result by applying the specified mapping function
    ///     to the success value of the result.
    /// </summary>
    /// <typeparam name="TValue">The type of the input value in the original result.</typeparam>
    /// <typeparam name="TOut">The type of the output value in the transformed result.</typeparam>
    /// <param name="result">The current result instance to be transformed.</param>
    /// <param name="map">The mapping function to be applied to the success value of the result.</param>
    /// <returns>A new result containing the transformed value.</returns>
    public static ResultTask<TOut> Map<TValue, TOut>(this ResultTask<TValue> result, Func<TValue, TOut> map)
        where TValue : notnull where TOut : notnull
    {
        return MapCore(result, map).AsAsync();

        static async ValueTask<Result<TOut>> MapCore(ResultTask<TValue> self, Func<TValue, TOut> map)
        {
            var source = await self;
            return source.Match(
                value => Result.Success(map(value)).WithMetadata(source.Metadata),
                error => Result.Failure<TOut>(error).WithMetadata(source.Metadata));
        }
    }

    /// <summary>
    ///     Transforms the result into a new success result using the specified asynchronous mapping function.
    /// </summary>
    /// <typeparam name="TValue">The type of the input value in the result.</typeparam>
    /// <typeparam name="TOut">The type of the output value.</typeparam>
    /// <param name="result">The current result instance.</param>
    /// <param name="map">The asynchronous mapping function to be applied.</param>
    /// <returns>A new result containing the transformed value.</returns>
    public static ResultTask<TOut> Map<TValue, TOut>(this ResultTask<TValue> result, Func<TValue, ValueTask<TOut>> map)
        where TValue : notnull where TOut : notnull
    {
        return MapCore(result, map).AsAsync();

        static async ValueTask<Result<TOut>> MapCore(ResultTask<TValue> self, Func<TValue, ValueTask<TOut>> map)
        {
            var source = await self;
            return await source.Match<ValueTask<Result<TOut>>>(
                async value =>
                {
                    var mapped = await map(value);
                    return Result.Success(mapped).WithMetadata(source.Metadata);
                },
                error => ValueTask.FromResult(Result.Failure<TOut>(error).WithMetadata(source.Metadata)));
        }
    }
}