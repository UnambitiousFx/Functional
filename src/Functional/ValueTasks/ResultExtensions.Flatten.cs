namespace UnambitiousFx.Functional.ValueTasks;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Async Flatten unwrapping a nested awaitable Result.
    /// </summary>
    /// <typeparam name="TValue1">Value type 1.</typeparam>
    /// <param name="awaitable">The awaitable nested result instance.</param>
    /// <returns>A task with the flattened result.</returns>
    public static async ValueTask<Result<TValue1>> FlattenAsync<TValue1>(
        this ValueTask<Result<Result<TValue1>>> awaitable) where TValue1 : notnull
    {
        var outer = await awaitable;
        return outer.Flatten();
    }
}