using UnambitiousFx.Functional;
using UnambitiousFx.Functional.Errors;

Console.WriteLine("=== UnambitiousFx.Functional Examples ===\n");

// ============================================================================
// 1. Result<T> - Basic Success and Failure
// ============================================================================
Console.WriteLine("1. Result<T> - Basic Success and Failure");
Console.WriteLine("-".PadRight(50, '-'));

var successResult = Result.Success(42);
if (successResult.TryGet(out var successValue, out _))
{
    Console.WriteLine($"Success result: IsSuccess={successResult.IsSuccess}, Value={successValue}");
}

var failureResult = Result.Failure<int>(new Error("Something went wrong"));
Console.WriteLine($"Failure result: IsFaulted={failureResult.IsFaulted}");
Console.WriteLine();

// ============================================================================
// 2. Result<T> - Pattern Matching
// ============================================================================
Console.WriteLine("2. Result<T> - Pattern Matching");
Console.WriteLine("-".PadRight(50, '-'));

var divisionResult = Divide(10, 2);
var message = divisionResult.Match(
    success: value => $"Division succeeded: {value}",
    failure: error => $"Division failed: {error.Message}"
);
Console.WriteLine(message);

var divisionByZero = Divide(10, 0);
var errorMessage = divisionByZero.Match(
    success: value => $"Division succeeded: {value}",
    failure: error => $"Division failed: {error.Message}"
);
Console.WriteLine(errorMessage);
Console.WriteLine();

// ============================================================================
// 3. Maybe<T> - Optional Values
// ============================================================================
Console.WriteLine("3. Maybe<T> - Optional Values");
Console.WriteLine("-".PadRight(50, '-'));

var someValue = Maybe.Some(42);
var noneValue = Maybe.None<int>();

if (someValue.Some(out var someVal))
{
    Console.WriteLine($"Some value: IsSome={someValue.IsSome}, Value={someVal}");
}
Console.WriteLine($"None value: IsNone={noneValue.IsNone}");

var maybeMessage = someValue.Match(
    some: value => $"Found value: {value}",
    none: () => "No value found"
);
Console.WriteLine(maybeMessage);
Console.WriteLine();

// ============================================================================
// 4. Result<T> - Chaining Operations
// ============================================================================
Console.WriteLine("4. Result<T> - Chaining Operations");
Console.WriteLine("-".PadRight(50, '-'));

var chainedResult = Result.Success("10")
    .Bind(ParseInt)
    .Bind(value => MultiplyByTwo(value))
    .Bind(value => Divide(value, 2));

chainedResult.Match(
    success: value => Console.WriteLine($"Chained operations result: {value}"),
    failure: error => Console.WriteLine($"Chained operations failed: {error.Message}")
);
Console.WriteLine();

// ============================================================================
// 5. Error Types
// ============================================================================
Console.WriteLine("5. Error Types");
Console.WriteLine("-".PadRight(50, '-'));

var notFoundError = Result.Failure<string>(new NotFoundError("User", "123"));
if (!notFoundError.TryGet(out _, out var notFoundErr))
{
    Console.WriteLine($"NotFound error: {notFoundErr.Message}");
}

var validationError = Result.Failure<string>(new ValidationError(["Email is required"]));
if (!validationError.TryGet(out _, out var validationErr))
{
    Console.WriteLine($"Validation error: {validationErr.Message}");
}

var unauthorizedError = Result.Failure<string>(new UnauthorizedError("Insufficient permissions"));
if (!unauthorizedError.TryGet(out _, out var unauthorizedErr))
{
    Console.WriteLine($"Unauthorized error: {unauthorizedErr.Message}");
}
Console.WriteLine();

// ============================================================================
// 6. Metadata
// ============================================================================
Console.WriteLine("6. Metadata");
Console.WriteLine("-".PadRight(50, '-'));

var resultWithMetadata = Result.Success(100)
    .WithMetadata("RequestId", "req-123")
    .WithMetadata("Timestamp", DateTime.UtcNow.ToString("O"));

if (resultWithMetadata.TryGet(out var metaValue, out _))
{
    Console.WriteLine($"Result with metadata: Value={metaValue}");
}
Console.WriteLine($"Metadata: {resultWithMetadata.Metadata}");
Console.WriteLine();

// ============================================================================
// 7. Side Effects
// ============================================================================
Console.WriteLine("7. Side Effects");
Console.WriteLine("-".PadRight(50, '-'));

var successSideEffect = Result.Success(42);
successSideEffect.IfSuccess(value => Console.WriteLine($"  Success callback: {value}"));
successSideEffect.IfFailure(error => Console.WriteLine($"  Failure callback: {error.Message}"));

var failureSideEffect = Result.Failure<int>(new Error("Oops"));
failureSideEffect.IfSuccess(value => Console.WriteLine($"  Success callback: {value}"));
failureSideEffect.IfFailure(error => Console.WriteLine($"  Failure callback: {error.Message}"));
Console.WriteLine();

// ============================================================================
// 8. Maybe<T> - Chaining
// ============================================================================
Console.WriteLine("8. Maybe<T> - Chaining");
Console.WriteLine("-".PadRight(50, '-'));

var user = FindUser("john");
var email = user.Bind(u => u.Email);
email.IfSome(e => Console.WriteLine($"  User email: {e}"));
email.IfNone(() => Console.WriteLine("  User or email not found"));
Console.WriteLine();

Console.WriteLine("=== Examples Complete ===");

// ============================================================================
// Helper Methods
// ============================================================================

static Result<int> Divide(int numerator, int denominator)
{
    if (denominator == 0)
        return Result.Failure<int>(new Error("Cannot divide by zero"));

    return Result.Success(numerator / denominator);
}

static Result<int> ParseInt(string value)
{
    if (int.TryParse(value, out var result))
        return Result.Success(result);

    return Result.Failure<int>(new ValidationError([$"'{value}' is not a valid integer"]));
}

static Result<int> MultiplyByTwo(int value)
{
    return Result.Success(value * 2);
}

static Maybe<User> FindUser(string username)
{
    if (username == "john")
        return Maybe.Some(new User("john", Maybe.Some("john@example.com")));

    return Maybe.None<User>();
}

record User(string Username, Maybe<string> Email);
