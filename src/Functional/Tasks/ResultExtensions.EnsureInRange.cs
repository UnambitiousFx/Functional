namespace UnambitiousFx.Functional.Tasks;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Ensures a numeric Task-wrapped result value is within the specified range (inclusive).
    /// </summary>
    /// <typeparam name="T">The result value type which must implement IComparable.</typeparam>
    /// <param name="resultTask">The Task-wrapped result instance.</param>
    /// <param name="min">The minimum allowed value (inclusive).</param>
    /// <param name="max">The maximum allowed value (inclusive).</param>
    /// <param name="message">Optional validation error message.</param>
    /// <param name="field">Optional field name for the error message.</param>
    /// <returns>The original result if the value is in range; otherwise a failure with ValidationError.</returns>
    public static async Task<Result<T>> EnsureInRange<T>(this Task<Result<T>> resultTask, T min, T max,
        string? message = null, string? field = null)
        where T : notnull, IComparable<T>
    {
        var result = await resultTask;
        return result.EnsureInRange(min, max, message, field);
    }

    /// <summary>
    ///     Ensures a numeric Task-wrapped result value is greater than the specified minimum.
    /// </summary>
    /// <typeparam name="T">The result value type which must implement IComparable.</typeparam>
    /// <param name="resultTask">The Task-wrapped result instance.</param>
    /// <param name="min">The minimum value (exclusive).</param>
    /// <param name="message">Optional validation error message.</param>
    /// <param name="field">Optional field name for the error message.</param>
    /// <returns>The original result if the value is greater than min; otherwise a failure with ValidationError.</returns>
    public static async Task<Result<T>> EnsureGreaterThan<T>(this Task<Result<T>> resultTask, T min,
        string? message = null, string? field = null)
        where T : notnull, IComparable<T>
    {
        var result = await resultTask;
        return result.EnsureGreaterThan(min, message, field);
    }

    /// <summary>
    ///     Ensures a numeric Task-wrapped result value is less than the specified maximum.
    /// </summary>
    /// <typeparam name="T">The result value type which must implement IComparable.</typeparam>
    /// <param name="resultTask">The Task-wrapped result instance.</param>
    /// <param name="max">The maximum value (exclusive).</param>
    /// <param name="message">Optional validation error message.</param>
    /// <param name="field">Optional field name for the error message.</param>
    /// <returns>The original result if the value is less than max; otherwise a failure with ValidationError.</returns>
    public static async Task<Result<T>> EnsureLessThan<T>(this Task<Result<T>> resultTask, T max,
        string? message = null, string? field = null)
        where T : notnull, IComparable<T>
    {
        var result = await resultTask;
        return result.EnsureLessThan(max, message, field);
    }
}
