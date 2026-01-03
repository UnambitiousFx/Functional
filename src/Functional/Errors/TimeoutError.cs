namespace UnambitiousFx.Functional.Errors;

/// <summary>Represents an operation exceeding the configured execution timeout.</summary>
public sealed record TimeoutError(
    TimeSpan ConfiguredTimeout,
    TimeSpan Elapsed)
    : Error(ErrorCodes.Timeout,
        $"Operation exceeded timeout of {ConfiguredTimeout.TotalMilliseconds}ms after {Elapsed.TotalMilliseconds}ms.")
{
}
