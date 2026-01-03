namespace UnambitiousFx.Functional.Errors;

/// <summary>
///     Exception that wraps a functional error, preserving the error code and metadata.
/// </summary>
public sealed class FunctionalException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="FunctionalException" /> class with an error.
    /// </summary>
    /// <param name="error">The error to wrap.</param>
    public FunctionalException(IError error) : base(error.Message)
    {
        Code = error.Code;
        foreach (var (key, value) in error.Metadata)
        {
            Data.Add(key, value);
        }
    }

    /// <summary>
    ///     Gets the error code from the wrapped error.
    /// </summary>
    public string Code { get; }
}
