namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for appending error messages to Result instances.
/// </summary>
public static partial class ResultExtensions
{
    /// <summary>
    /// Converts a <see cref="Result{T}"/> into a non-generic <see cref="Result"/> by matching
    /// the success or failure state of the original result and projecting it accordingly.
    /// </summary>
    /// <param name="result">
    /// The generic result instance to be converted. It contains either a success state with a value
    /// of type <typeparamref name="T"/> or a failure state with an error.
    /// </param>
    /// <typeparam name="T">
    /// The type of the value encapsulated in the original <see cref="Result{T}"/>. This type must be non-nullable.
    /// </typeparam>
    /// <returns>
    /// A non-generic <see cref="Result"/> that represents the same success or failure state as
    /// the original <see cref="Result{T}"/>.
    /// </returns>
    public static Result ToResult<T>(this Result<T> result) where T : notnull
    {
        return result.Match(Result.Success, Result.Failure);
    }
}
