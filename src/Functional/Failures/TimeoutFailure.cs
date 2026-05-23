namespace UnambitiousFx.Functional.Failures;

/// <summary>Represents an operation exceeding the configured execution timeout.</summary>
public record TimeoutFailure(TimeSpan ConfiguredTimeout,
                             TimeSpan Elapsed)
    : Failure(FailureCodes.Timeout,
              $"Operation exceeded timeout of {ConfiguredTimeout.TotalMilliseconds}ms after {Elapsed.TotalMilliseconds}ms.")
{
}
