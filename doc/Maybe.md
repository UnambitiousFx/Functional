# Maybe<T>

`Maybe<T>` is a functional type that represents an optional value: either **Some** value of type `T`, or **None** (the
absence of a value). It provides a type-safe alternative to null references and enables composable operations on
potentially missing values.

## Core Concepts

### When to Use

- **Null Safety**: Replace nullable types with explicit optionality
- **API Design**: Signal that a value may or may not be present (e.g., configuration values, cache lookups)
- **Database Queries**: Represent optional results (e.g., `FindById` returning `Maybe<User>`)
- **Validation**: Chain operations that may fail to produce a value
- **Parsing**: Handle optional or invalid input gracefully

### Key Characteristics

- **Zero-Allocation**: Implemented as `readonly record struct` for minimal overhead
- **Type Safety**: Forces explicit handling of the None case at compile time
- **Composable**: Supports functional composition with `Map`, `Bind`, `Filter`, etc.
- **LINQ Integration**: Supports query syntax via `Select`, `SelectMany`, and `Where`

## Creating Maybe Values

### Static Factory Methods

```csharp
// Create a value
Maybe<int> some = Maybe.Some(42);

// Create an empty value
Maybe<int> none = Maybe.None<int>();
```

### Implicit Conversion

```csharp
// Null converts to None
Maybe<string> none = null;

// Non-null values convert to Some
Maybe<string> some = "hello";
```

## Pattern Matching

### Match

Execute different code paths based on the state:

```csharp
var message = maybe.Match(
    some: value => $"Found: {value}",
    none: () => "Not found"
);
```

### Switch

Perform side effects based on the state:

```csharp
maybe.Switch(
    some: value => Console.WriteLine($"Value: {value}"),
    none: () => Console.WriteLine("No value")
);
```

### IfSome / IfNone

Execute conditional side effects:

```csharp
maybe.IfSome(value => Console.WriteLine($"Value: {value}"));
maybe.IfNone(() => Console.WriteLine("No value"));
```

### Some (Pattern)

Use pattern matching with an out parameter:

```csharp
if (maybe.Some(out var value))
{
    // Use value here
}
```

## Transformations

### Map

Transform the contained value if present:

```csharp
Maybe<int> number = Maybe.Some(5);
Maybe<string> text = number.Map(n => n.ToString());
// Result: Some("5")

Maybe<int> none = Maybe.None<int>();
Maybe<string> stillNone = none.Map(n => n.ToString());
// Result: None
```

### Bind (FlatMap)

Chain operations that return `Maybe`:

```csharp
Maybe<int> ParseInt(string input) =>
    int.TryParse(input, out var result) ? Maybe.Some(result) : Maybe.None<int>();

Maybe<int> Divide(int numerator, int denominator) =>
    denominator == 0 ? Maybe.None<int>() : Maybe.Some(numerator / denominator);

Maybe<string> input = Maybe.Some("10");
Maybe<int> result = input
    .Bind(ParseInt)
    .Bind(x => Divide(100, x));
// Result: Some(10)

Maybe<string> invalid = Maybe.Some("abc");
Maybe<int> failed = invalid.Bind(ParseInt);
// Result: None
```

### Filter / Where

Keep the value only if it satisfies a predicate:

```csharp
Maybe<int> number = Maybe.Some(42);
Maybe<int> filtered = number.Filter(n => n > 50);
// Result: None

Maybe<int> passed = number.Filter(n => n > 40);
// Result: Some(42)
```

## Side Effects

### Tap

Execute side effects while preserving the value:

```csharp
Maybe<int> result = maybe
    .Tap(value => Console.WriteLine($"Processing: {value}"))
    .Map(x => x * 2)
    .Tap(value => Console.WriteLine($"Result: {value}"));
```

### TapSome / TapNone

Execute conditional side effects:

```csharp
maybe
    .TapSome(value => _logger.LogInformation("Found: {Value}", value))
    .TapNone(() => _logger.LogWarning("Value not found"));
```

## Fallback Values

### ValueOr

Extract the value or provide a fallback:

```csharp
int value = maybe.ValueOr(0);
int computed = maybe.ValueOr(() => ExpensiveDefault());
```

### OrElse

Provide an alternative `Maybe`:

```csharp
Maybe<int> result = primary
    .OrElse(secondary)
    .OrElse(() => FetchFromDatabase());
```

## Conversion to Result

Convert a `Maybe` to a `Result` by providing an error for the None case:

```csharp
Result<User> result = maybeUser.ToResult("User not found");
Result<User> withError = maybeUser.ToResult(new NotFoundError("User"));
Result<User> withFactory = maybeUser.ToResult(() => CreateError());
```

## LINQ Query Syntax

`Maybe` supports LINQ query expressions:

```csharp
var result =
    from user in GetUser(id)
    from profile in GetProfile(user.ProfileId)
    where profile.IsActive
    select new UserWithProfile(user, profile);
```

This is equivalent to:

```csharp
var result = GetUser(id)
    .Bind(user => GetProfile(user.ProfileId)
        .Map(profile => (user, profile)))
    .Filter(tuple => tuple.profile.IsActive)
    .Map(tuple => new UserWithProfile(tuple.user, tuple.profile));
```

## Async Support

All major operations have async overloads for `ValueTask<Maybe<T>>` and `Task<Maybe<T>>`:

```csharp
ValueTask<Maybe<User>> userTask = GetUserAsync(id);

// Async Map
Maybe<string> name = await userTask.Map(u => u.Name);

// Async Bind
Maybe<Profile> profile = await userTask.Bind(u => GetProfileAsync(u.ProfileId));

// Async Tap
await userTask.TapSome(async u => await LogAsync(u));

// Async Switch
await userTask.Switch(
    some: async u => await SaveAsync(u),
    none: async () => await LogErrorAsync()
);

// Async ToResult
Result<User> result = await userTask.ToResult("User not found");
```

## Properties and Methods

### Properties

| Property | Type      | Description                                       |
|----------|-----------|---------------------------------------------------|
| `IsSome` | `bool`    | `true` if the value is present                    |
| `IsNone` | `bool`    | `true` if the value is absent                     |
| `Case`   | `TValue?` | Returns the value if present, otherwise `default` |

### Core Methods

| Method       | Signature                            | Description                                 |
|--------------|--------------------------------------|---------------------------------------------|
| `Some<T>()`  | `bool Some(out T value)`             | Pattern matching with out parameter         |
| `Match<T>()` | `T Match(Func<TValue, T>, Func<T>)`  | Execute one of two functions based on state |
| `Match()`    | `void Match(Action<TValue>, Action)` | Execute one of two actions based on state   |
| `IfSome()`   | `void IfSome(Action<TValue>)`        | Execute action if value is present          |
| `IfNone()`   | `void IfNone(Action)`                | Execute action if value is absent           |

### Extension Methods

| Method         | Description                              |
|----------------|------------------------------------------|
| `Switch()`     | Alias for `Match` with actions           |
| `Map<TOut>()`  | Transform the value if present           |
| `Bind<TOut>()` | Chain operations returning `Maybe`       |
| `Filter()`     | Keep value only if predicate is true     |
| `Tap()`        | Execute side effect and return unchanged |
| `TapSome()`    | Execute side effect only when Some       |
| `TapNone()`    | Execute side effect only when None       |
| `ValueOr()`    | Extract value or provide fallback        |
| `OrElse()`     | Provide alternative `Maybe`              |
| `ToResult()`   | Convert to `Result<T>` with error        |
| `Select()`     | LINQ projection (alias for `Map`)        |
| `SelectMany()` | LINQ bind (alias for `Bind`)             |
| `Where()`      | LINQ filter (alias for `Filter`)         |

### Async Extension Methods

All methods support both `ValueTask<Maybe<T>>` and `Task<Maybe<T>>`.

| Method                  | Signature                                                                  | Description                         |
|-------------------------|----------------------------------------------------------------------------|-------------------------------------|
| `Map<TOut>()`           | `ValueTask<Maybe<TOut>> Map(Func<TIn, TOut>)`                              | Transform the value if present      |
| `Map<TOut>()` (async)   | `ValueTask<Maybe<TOut>> Map(Func<TIn, ValueTask<TOut>>)`                   | Transform with async function       |
| `Bind<TOut>()`          | `ValueTask<Maybe<TOut>> Bind(Func<TIn, Maybe<TOut>>)`                      | Chain operations returning `Maybe`  |
| `Bind<TOut>()` (async)  | `ValueTask<Maybe<TOut>> Bind(Func<TIn, ValueTask<Maybe<TOut>>>)`           | Chain async operations              |
| `Filter()`              | `ValueTask<Maybe<TIn>> Filter(Func<TIn, bool>)`                            | Filter with predicate               |
| `Filter()` (async)      | `ValueTask<Maybe<TIn>> Filter(Func<TIn, ValueTask<bool>>)`                 | Filter with async predicate         |
| `TapSome()`             | `ValueTask<Maybe<TIn>> TapSome(Action<TIn>)`                               | Execute side effect when Some       |
| `TapSome()` (async)     | `ValueTask<Maybe<TIn>> TapSome(Func<TIn, ValueTask>)`                      | Execute async side effect when Some |
| `TapNone()`             | `ValueTask<Maybe<TIn>> TapNone(Action)`                                    | Execute side effect when None       |
| `TapNone()` (async)     | `ValueTask<Maybe<TIn>> TapNone(Func<ValueTask>)`                           | Execute async side effect when None |
| `ValueOr()`             | `ValueTask<TIn> ValueOr(TIn fallback)`                                     | Extract value or fallback           |
| `ValueOr()` (factory)   | `ValueTask<TIn> ValueOr(Func<TIn>)`                                        | Extract value or compute fallback   |
| `ValueOr()` (async)     | `ValueTask<TIn> ValueOr(Func<ValueTask<TIn>>)`                             | Extract value or async fallback     |
| `OrElse()`              | `ValueTask<Maybe<TIn>> OrElse(Maybe<TIn>)`                                 | Fallback Maybe if None              |
| `OrElse()` (factory)    | `ValueTask<Maybe<TIn>> OrElse(Func<Maybe<TIn>>)`                           | Compute fallback Maybe if None      |
| `OrElse()` (async)      | `ValueTask<Maybe<TIn>> OrElse(Func<ValueTask<Maybe<TIn>>>)`                | Async fallback Maybe if None        |
| `Match<TOut>()`         | `ValueTask<TOut> Match(Func<TIn, TOut>, Func<TOut>)`                       | Match and return result             |
| `Match<TOut>()` (async) | `ValueTask<TOut> Match(Func<TIn, ValueTask<TOut>>, Func<ValueTask<TOut>>)` | Async match with result             |
| `Switch()`              | `ValueTask Switch(Action<TIn>, Action)`                                    | Execute actions based on state      |
| `Switch()` (async)      | `ValueTask Switch(Func<TIn, ValueTask>, Func<ValueTask>)`                  | Execute async actions               |
| `ToResult()`            | `ValueTask<Result<TIn>> ToResult(Failure)`                                 | Convert to `Result<T>`              |
| `ToResult()` (factory)  | `ValueTask<Result<TIn>> ToResult(Func<Failure>)`                           | Convert with failure factory        |
| `ToResult()` (message)  | `ValueTask<Result<TIn>> ToResult(string)`                                  | Convert with error message          |
| `ToResult()` (async)    | `ValueTask<Result<TIn>> ToResult(Func<ValueTask<Failure>>)`                | Convert with async failure factory  |

## Best Practices

### Do

- ✅ Use `Maybe<T>` for optional values that genuinely may or may not exist
- ✅ Chain operations with `Bind` and `Map` for clean, composable code
- ✅ Use LINQ query syntax for complex chains with multiple binds
- ✅ Prefer `ValueOr` for simple defaults and `OrElse` for complex fallback logic
- ✅ Convert to `Result<T>` when you need to provide error context

### Don't

- ❌ Use `Maybe<T>` for error conditions—use `Result<T>` instead
- ❌ Mix `Maybe<T>` with nullable types unnecessarily
- ❌ Use `Case` property directly—prefer `Match`, `ValueOr`, or pattern matching
- ❌ Nest `Maybe<Maybe<T>>`—use `Bind` to flatten

## Real-World Examples

### Configuration Value

```csharp
public Maybe<int> GetMaxRetries() =>
    _configuration["MaxRetries"]
        .Map(int.TryParse)
        .Filter(x => x > 0);

int maxRetries = GetMaxRetries().ValueOr(3);
```

### Database Query

```csharp
public async ValueTask<Maybe<User>> FindUserByEmail(string email)
{
    var user = await _dbContext.Users
        .Where(u => u.Email == email)
        .FirstOrDefaultAsync();

    return user; // Implicit conversion from User? to Maybe<User>
}

var result = await FindUserByEmail("user@example.com")
    .Map(u => new UserDto(u.Id, u.Name))
    .ToResult("User not found");
```

### Chaining Operations

```csharp
var orderTotal = await GetUser(userId)
    .Bind(user => GetCart(user.CartId))
    .Filter(cart => cart.Items.Any())
    .Map(cart => cart.Items.Sum(i => i.Price))
    .TapSome(total => _logger.LogInformation("Order total: {Total}", total))
    .ValueOr(0m);
```

### LINQ Query with Multiple Sources

```csharp
var profileData =
    from user in GetUser(id)
    from settings in GetSettings(user.SettingsId)
    from avatar in GetAvatar(user.AvatarId)
    where settings.IsPublic
    select new PublicProfile(user.Name, avatar.Url, settings);
```

## See Also

- [Result<T>](Result.md) - For operations that can fail with errors
- [OneOf<T1, T2, ...>](OneOf.md) - For discriminated unions with multiple cases
