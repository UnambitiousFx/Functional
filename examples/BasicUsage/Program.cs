using UnambitiousFx.Functional;
using UnambitiousFx.Functional.Failures;

Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║  🚂 Welcome to the Railway-Oriented Programming Express 🚂     ║");
Console.WriteLine("║     All aboard for error handling in style!                     ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
Console.WriteLine();

// ============================================================================
// 1. Result<T> - Basic Success and Failure
// ============================================================================
Console.WriteLine("1. Result<T> - Basic Success and Failure");
Console.WriteLine(new string('-', 60));

var successResult = Result.Success(42);
var failureResult = Result.Failure<int>(new Failure("Something went wrong"));

Console.WriteLine($"Success: IsSuccess={successResult.IsSuccess}, Value={successResult.Match(s => s.ToString(), _ => "N/A")}");
Console.WriteLine($"Failure: IsFailure={failureResult.IsFailure}, Message={failureResult.Match(_ => "N/A", f => f.Message)}");
Console.WriteLine();

// ============================================================================
// 2. Pattern Matching with Match()
// ============================================================================
Console.WriteLine("2. Pattern Matching with Match() - The Ultimate Destiny Decider");
Console.WriteLine(new string('-', 60));

var divisionResult = Divide(10, 2);
var message = divisionResult.Match(
    success: value => $"✓ Math works! 10 / 2 = {value} (shocking)",
    failure: error => $"✗ Someone broke math: {error.Message}"
);
Console.WriteLine(message);

var divisionByZero = Divide(10, 0);
var errorMessage = divisionByZero.Match(
    success: value => $"✓ Math works! Result: {value}",
    failure: error => $"✗ RIP in peace calculator: {error.Message}"
);
Console.WriteLine(errorMessage);
Console.WriteLine();

// ============================================================================
// 3. Maybe<T> - Optional Values (The Schrödinger's Variable)
// ============================================================================
Console.WriteLine("3. Maybe<T> - Optional Values (Schrödinger's Variable)");
Console.WriteLine(new string('-', 60));

var someValue = Maybe.Some(42);
var noneValue = Maybe.None<int>();

var someMessage = someValue.Match(
    some: value => $"✓ Found it! The meaning of life is {value}",
    none: () => "✗ Houston, we have a void"
);
Console.WriteLine(someMessage);

var noneMessage = noneValue.Match(
    some: value => $"✓ Found value: {value}",
    none: () => "✗ It's gone. Poof. Like my productivity on Monday mornings."
);
Console.WriteLine(noneMessage);
Console.WriteLine();

// ============================================================================
// 4. Railway-Oriented Programming: Chaining with Bind()
// ============================================================================
Console.WriteLine("4. Railway-Oriented Programming (The Functional Choo-Choo Train)");
Console.WriteLine(new string('-', 60));

var chainedResult = Result.Success("10")
    .Bind(ParseInt)          // 🚂 Station 1: Parse the string
    .Bind(value => Multiply(value, 2))  // 🚂 Station 2: Double it
    .Bind(value => Divide(value, 2));   // 🚂 Station 3: Halve it (back where we started!)

chainedResult.Match(
    success: value => Console.WriteLine($"✓ Train arrived safely! \"10\" → 🔢 → 🔀 → ➗ = {value}"),
    failure: error => Console.WriteLine($"✗ Train derailed: {error.Message}")
);
Console.WriteLine();

// ============================================================================
// 5. Ensure() - Validation with Predicates
// ============================================================================
Console.WriteLine("5. Ensure() - Validation with Predicates");
Console.WriteLine(new string('-', 60));

var ageValidation = Result.Success(25)
    .Ensure(age => age >= 18, _ => new ValidationFailure(["User must be 18 or older"]))
    .Ensure(age => age <= 120, _ => new ValidationFailure(["Invalid age"]))
    .Match(
        success: age => $"✓ Age {age} is valid",
        failure: error => $"✗ Validation failed: {error.Message}"
    );
Console.WriteLine(ageValidation);

var invalidAgeValidation = Result.Success(15)
    .Ensure(age => age >= 18, _ => new ValidationFailure(["User must be 18 or older"]))
    .Match(
        success: age => $"✓ Age {age} is valid",
        failure: error => $"✗ Validation failed: {error.Message}"
    );
Console.WriteLine(invalidAgeValidation);
Console.WriteLine();

// ============================================================================
// 6. Recover() - Error Handling and Recovery (The Undo Button)
// ============================================================================
Console.WriteLine("6. Recover() - Error Handling (ctrl+z for your code)");
Console.WriteLine(new string('-', 60));

var recovering = Divide(10, 0)
    .Recover(failure =>
    {
        Console.WriteLine($"  💥 CRASH! {failure.Message}");
        Console.WriteLine($"  🔧 Emergency repairs initiated...");
        return 0.0; // Panic mode: return zero
    })
    .Match(
        success: value => $"✓ Crisis averted! Returned {value}",
        failure: error => $"✗ Still on fire: {error.Message}"
    );
Console.WriteLine(recovering);
Console.WriteLine();

// ============================================================================
// 7. Tap() - Side Effects Without Changing Value (The Spy Tactic)
// ============================================================================
Console.WriteLine("7. Tap() - Side Effects (Sneakily observe without touching)");
Console.WriteLine(new string('-', 60));

var withSideEffect = Result.Success(42)
    .Tap(value => Console.WriteLine($"  👀 Logged: Processing the legendary number {value}"))
    .Tap(value => Console.WriteLine($"  📊 Analytics: Confirmed value > 0: {value > 0}"))
    .Tap(_ => Console.WriteLine($"  🎬 Camera rolling..."))
    .Map(value => value * 2);

withSideEffect.Match(
    success: value => Console.WriteLine($"✓ Action! Final value: {value}"),
    failure: error => Console.WriteLine($"✗ Cut! Failed: {error.Message}")
);
Console.WriteLine();

// ============================================================================
// 8. Filter() - Filtering Values
// ============================================================================
Console.WriteLine("8. Filter() - Filtering Values");
Console.WriteLine(new string('-', 60));

var filterEven = Maybe.Some(4)
    .Filter(x => x % 2 == 0)
    .Match(
        some: value => $"✓ {value} is even",
        none: () => "✗ Number is not even"
    );
Console.WriteLine(filterEven);

var filterOdd = Maybe.Some(3)
    .Filter(x => x % 2 == 0)
    .Match(
        some: value => $"✓ {value} is even",
        none: () => "✗ Number is not even"
    );
Console.WriteLine(filterOdd);
Console.WriteLine();

// ============================================================================
// 9. Different Failure Types (The Error Bingo Card)
// ============================================================================
Console.WriteLine("9. Different Failure Types (Error Taxonomy 101)");
Console.WriteLine(new string('-', 60));

var notFound = Result.Failure<string>(new NotFoundFailure("User", "user-123"));
Console.WriteLine($"  👻 NotFound: {notFound.Match(_ => "", f => f.Message)}");

var validation = Result.Failure<string>(new ValidationFailure(["Email is invalid", "Name is required"]));
Console.WriteLine($"  ❌ Validation: {validation.Match(_ => "", f => f.Message)}");

var unauthorized = Result.Failure<string>(new UnauthorizedFailure("Admin role required"));
Console.WriteLine($"  🚫 Unauthorized: {unauthorized.Match(_ => "", f => f.Message)}");

var conflict = Result.Failure<string>(new ConflictFailure("Email already exists"));
Console.WriteLine($"  💥 Conflict: {conflict.Match(_ => "", f => f.Message)}");

var timeout = Result.Failure<string>(new TimeoutFailure(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(35)));
Console.WriteLine($"  ⏱️  Timeout: {timeout.Match(_ => "", f => f.Message)}");
Console.WriteLine();

// ============================================================================
// 10. Metadata - Attaching Context to Results
// ============================================================================
Console.WriteLine("10. Metadata - Attaching Context to Results");
Console.WriteLine(new string('-', 60));

var resultWithMetadata = Result.Success(100)
    .WithMetadata("RequestId", "req-12345")
    .WithMetadata("Timestamp", DateTime.UtcNow.ToString("O"))
    .WithMetadata("UserId", "user-42");

var metadataInfo = resultWithMetadata.Match(
    success: value => $"✓ Value: {value}",
    failure: _ => "✗ Failed"
);
Console.WriteLine(metadataInfo);
Console.WriteLine($"  Metadata: {resultWithMetadata.Metadata}");
Console.WriteLine();

// ============================================================================
// 11. IfSuccess/IfFailure - Conditional Execution
// ============================================================================
Console.WriteLine("11. IfSuccess/IfFailure - Conditional Execution");
Console.WriteLine(new string('-', 60));

var successCase = Result.Success(42);
successCase.IfSuccess(value => Console.WriteLine($"  ✓ Success branch: Value is {value}"));
successCase.IfFailure(error => Console.WriteLine($"  ✗ Failure branch: {error.Message}"));

var failureCase = Result.Failure<int>(new Failure("Oops"));
Console.WriteLine("  ---");
failureCase.IfSuccess(value => Console.WriteLine($"  ✓ Success branch: Value is {value}"));
failureCase.IfFailure(error => Console.WriteLine($"  ✗ Failure branch: {error.Message}"));
Console.WriteLine();

// ============================================================================
// 12. Maybe<T> - Chaining with Bind()
// ============================================================================
Console.WriteLine("12. Maybe<T> - Chaining with Bind()");
Console.WriteLine(new string('-', 60));

var user = FindUser("john");
var emailResult = user.Bind(u => u.Email);
var emailMessage = emailResult.Match(
    some: email => $"✓ Email found: {email}",
    none: () => "✗ User or email not found"
);
Console.WriteLine(emailMessage);

var unknownUser = FindUser("unknown");
var unknownEmail = unknownUser.Bind(u => u.Email);
var unknownMessage = unknownEmail.Match(
    some: email => $"✓ Email found: {email}",
    none: () => "✗ User or email not found"
);
Console.WriteLine(unknownMessage);
Console.WriteLine();

// ============================================================================
// 13. Map() - Transforming Values
// ============================================================================
Console.WriteLine("13. Map() - Transforming Values");
Console.WriteLine(new string('-', 60));

var doubled = Result.Success(5)
    .Map(x => x * 2)
    .Map(x => x + 3)
    .Match(
        success: value => $"✓ Transformed value: 5 → (×2) → (+3) = {value}",
        failure: error => $"✗ Failed: {error.Message}"
    );
Console.WriteLine(doubled);
Console.WriteLine();

// ============================================================================
// 14. Exception Wrapping
// ============================================================================
Console.WriteLine("14. Exception Wrapping");
Console.WriteLine(new string('-', 60));

var wrappedException = Result.Success("42")
    .Try(str =>
    {
        // This will throw, which Try() will catch and convert to a Failure
        return int.Parse(str);
    })
    .Match(
        success: value => $"✓ Parsed successfully: {value}",
        failure: error => $"✗ Parse failed: {error.Message}"
    );
Console.WriteLine(wrappedException);

var wrappedFailedException = Result.Success("not-a-number")
    .Try(str =>
    {
        // This will throw, which Try() will catch and convert to a Failure
        return int.Parse(str);
    })
    .Match(
        success: value => $"✓ Parsed successfully: {value}",
        failure: error => $"✗ Parse failed: {error.Message}"
    );
Console.WriteLine(wrappedFailedException);
Console.WriteLine();

Console.WriteLine();
Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║  🎉 All examples completed! You've leveled up! 🎉             ║");
Console.WriteLine("║  Next stop: Production-grade functional code!                  ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
Console.WriteLine();

// ============================================================================
// Helper Methods
// ============================================================================

static Result<double> Divide(int numerator, int denominator)
{
    if (denominator == 0)
        return Result.Failure<double>(new ValidationFailure(["Cannot divide by zero"]));

    return Result.Success((double)numerator / denominator);
}

static Result<int> Multiply(int value, int factor)
{
    return Result.Success(value * factor);
}

static Result<int> ParseInt(string value)
{
    if (int.TryParse(value, out var result))
        return Result.Success(result);

    return Result.Failure<int>(new ValidationFailure([$"'{value}' is not a valid integer"]));
}

static Maybe<User> FindUser(string username)
{
    var users = new Dictionary<string, User>
    {
        ["john"] = new("john", Maybe.Some("john@example.com")),
        ["jane"] = new("jane", Maybe.Some("jane@example.com")),
        ["bob"] = new("bob", Maybe.None<string>()), // Bob is mysterious - no email!
        ["nobody"] = new("nobody", Maybe.None<string>()), // Username checks out
    };

    return users.TryGetValue(username, out var user)
        ? Maybe.Some(user)
        : Maybe.None<User>();
}

// ============================================================================
// Domain Models
// ============================================================================

record User(string Username, Maybe<string> Email);
