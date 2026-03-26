# Result

The `Result` type represents the outcome of an operation that can either succeed or fail with an error. It embodies
railway-oriented programming, providing a type-safe alternative to exceptions for control flow.

## Table of Contents

- [Core Types](#core-types)
- [Properties and Methods](#properties-and-methods)
- [Creating Results](#creating-results)
- [Pattern Matching](#pattern-matching)
- [Transformations](#transformations)
- [Error Handling](#error-handling)
- [Utilities](#utilities)
- [Metadata](#metadata)
- [LINQ Support](#linq-support)
- [Best Practices](#best-practices)

## Core Types

### `Result`

A non-generic result representing success or failure without a value.

```csharp
public readonly partial record struct Result : IResult
```

**Properties:**

- `IsSuccess`: `bool` - Indicates whether the operation succeeded
- `IsFailure`: `bool` - Indicates whether the operation failed
- `Metadata`: `IReadOnlyMetadata` - Associated metadata

**Key Methods:**

- `TryGetError(out Failure? error)`: Attempts to extract the error if failed
- `Deconstruct(out Failure? error)`: Deconstructs the result into its error component

### `Result<TValue>`

A generic result representing success with a value or failure with an error.

```csharp
public readonly partial record struct Result<TValue> : IResult
    where TValue : notnull
```

**Properties:**

- `IsSuccess`: `bool` - Indicates whether the operation succeeded
- `IsFailure`: `bool` - Indicates whether the operation failed
- `Metadata`: `IReadOnlyMetadata` - Associated metadata

**Key Methods:**

- `TryGet(out TValue? value, out Failure? error)`: Attempts to extract the success value and error
- `TryGetValue(out TValue? value)`: Attempts to extract the success value
- `TryGetError(out Failure? error)`: Attempts to extract the error if failed
- `Deconstruct(out TValue? value, out Failure? error)`: Deconstructs into value and error
- `ToResult()`: Converts to an untyped `Result`

## Properties and Methods

### Properties

| Property    | Type                | Description                               |
|-------------|---------------------|-------------------------------------------|
| `IsSuccess` | `bool`              | Indicates whether the operation succeeded |
| `IsFailure` | `bool`              | Indicates whether the operation failed    |
| `Metadata`  | `IReadOnlyMetadata` | Associated metadata                       |

### Core Methods

| Method                                          | Description                                                             |
|-------------------------------------------------|-------------------------------------------------------------------------|
| `TryGetError(out Failure? error)`               | Attempts to extract the error if failed                                 |
| `TryGet(out TValue? value, out Failure? error)` | Attempts to extract the success value and error (`Result<TValue>` only) |
| `TryGetValue(out TValue? value)`                | Attempts to extract the success value (`Result<TValue>` only)           |
| `Match(success, failure)`                       | Pattern matches the result, executing the appropriate function          |
| `Switch(success, failure)`                      | Alias for `Match`, executes actions based on state                      |
| `IfSuccess(action)`                             | Executes the action if the result is successful                         |
| `IfFailure(action)`                             | Executes the action if the result is a failure                          |
| `Deconstruct(...)`                              | Deconstructs the result into its components                             |
| `WithMetadata(...)`                             | Creates a new result with added/merged metadata                         |
| `ToResult()`                                    | Converts `Result<TValue>` to untyped `Result`                           |

### Extension Methods

| Method                    | Signature                                           | Description                                                  |
|---------------------------|-----------------------------------------------------|--------------------------------------------------------------|
| **Transformations**       |                                                     |                                                              |
| `Bind`                    | `Bind(Func<Result>)`                                | Chains operations that return `Result`, propagating failures |
| `Bind`                    | `Bind(Func<TValue, Result<TOut>>)`                  | Chains operations with value transformation                  |
| `Map`                     | `Map(Func<TOut>)`                                   | Transforms success values without changing wrapper           |
| `Map`                     | `Map(Func<TValue, TOut>)`                           | Transforms success values                                    |
| `Then`                    | `Then(Func<Result>)`                                | Chains transformations returning same type                   |
| `Then`                    | `Then(Func<TValue, Result<TValue>>)`                | Chains transformations maintaining type                      |
| `Flatten`                 | `Flatten()`                                         | Removes one level of nesting from `Result<Result<T>>`        |
| **Error Handling**        |                                                     |                                                              |
| `Recover`                 | `Recover(TValue fallback)`                          | Provides fallback value when result fails                    |
| `Recover`                 | `Recover(Func<Failure, TValue>)`                    | Provides fallback via factory function                       |
| `Compensate`              | `Compensate(Func<Failure, Result>)`                 | Executes rollback on failure, aggregates errors              |
| `Compensate`              | `Compensate(Func<Result>)`                          | Executes rollback on failure                                 |
| `Try`                     | `Try(Action)`                                       | Executes action that may throw, catches exceptions           |
| `Try`                     | `Try(Func<TValue, TOut>)`                           | Executes function that may throw                             |
| `Ensure`                  | `Ensure(Func<TValue, bool>, Func<TValue, Failure>)` | Validates values with predicate                              |
| **Side Effects**          |                                                     |                                                              |
| `Tap`                     | `Tap(Action)`                                       | Executes side effect on success                              |
| `Tap`                     | `Tap(Action<TValue>)`                               | Executes side effect with value access                       |
| `TapIf`                   | `TapIf(Func<TValue, bool>, Action)`                 | Conditional side effect                                      |
| `TapError`                | `TapError(Action<Failure>)`                         | Executes side effect on failure                              |
| **Value Extraction**      |                                                     |                                                              |
| `ValueOr`                 | `ValueOr(TValue fallback)`                          | Extracts value or returns fallback                           |
| `ValueOr`                 | `ValueOr(Func<TValue>)`                             | Extracts value or computes fallback                          |
| `ValueOrDefault`          | `ValueOrDefault()`                                  | Extracts value or returns type default                       |
| `ValueOrThrow`            | `ValueOrThrow()`                                    | Extracts value or throws aggregated exception                |
| `ValueOrThrow`            | `ValueOrThrow(Func<Failure, Exception>)`            | Extracts value or throws custom exception                    |
| **Utilities**             |                                                     |                                                              |
| `Combine`                 | `Combine()`                                         | Combines multiple results, aggregating errors                |
| **LINQ Support**          |                                                     |                                                              |
| `Select`                  | `Select(Func<TIn, TOut>)`                           | LINQ projection operator (alias for `Map`)                   |
| `SelectMany`              | `SelectMany(Func<TIn, Result<TOut>>)`               | LINQ bind operator (alias for `Bind`)                        |
| `Where`                   | `Where(Func<TValue, bool>)`                         | LINQ filter operator (alias for `Ensure`)                    |
| **Specialized Factories** |                                                     |                                                              |
| `FailNotFound`            | `FailNotFound(resource, identifier)`                | Creates `NotFoundFailure` result                             |
| `FailValidation`          | `FailValidation(message)`                           | Creates `ValidationFailure` result                           |
| `FailUnauthenticated`     | `FailUnauthenticated(reason)`                       | Creates `UnauthenticatedFailure` result                      |
| `FailUnauthorized`        | `FailUnauthorized(reason)`                          | Creates `UnauthorizedFailure` result                         |
| `FailConflict`            | `FailConflict(message)`                             | Creates `ConflictFailure` result                             |

### Async Extension Methods

All extension methods support asynchronous operations via `ValueTask<Result<T>>` and `Task<Result<T>>`. Each async
extension provides both sync and async function overloads.

| Method                     | Signature                                                           | Description                               |
|----------------------------|---------------------------------------------------------------------|-------------------------------------------|
| **Async Transformations**  |                                                                     |                                           |
| `Map`                      | `Map(Func<TIn, TOut>)`                                              | Transform success value (sync mapper)     |
| `Map`                      | `Map(Func<TIn, ValueTask<TOut>>)`                                   | Transform success value (async mapper)    |
| `Bind`                     | `Bind(Func<TIn, Result<TOut>>)`                                     | Chain operations (sync)                   |
| `Bind`                     | `Bind(Func<TIn, ValueTask<Result<TOut>>>)`                          | Chain async operations                    |
| `Tap`                      | `Tap(Action<TIn>)`                                                  | Execute side effect on success (sync)     |
| `Tap`                      | `Tap(Func<TIn, ValueTask>)`                                         | Execute async side effect on success      |
| **Async Pattern Matching** |                                                                     |                                           |
| `Match`                    | `Match(Func<TIn, TOut>, Func<Failure, TOut>)`                       | Match with sync functions                 |
| `Match`                    | `Match(Func<TIn, ValueTask<TOut>>, Func<Failure, ValueTask<TOut>>)` | Match with async functions                |
| `Switch`                   | `Switch(Action<TIn>, Action<Failure>)`                              | Execute actions based on state            |
| `Switch`                   | `Switch(Func<TIn, ValueTask>, Func<Failure, ValueTask>)`            | Execute async actions                     |
| **Async Value Extraction** |                                                                     |                                           |
| `ValueOr`                  | `ValueOr(TValue fallback)`                                          | Extract value or use fallback             |
| `ValueOr`                  | `ValueOr(Func<TValue>)`                                             | Extract value or compute fallback (sync)  |
| `ValueOr`                  | `ValueOr(Func<ValueTask<TValue>>)`                                  | Extract value or compute fallback (async) |
| `ValueOrDefault`           | `ValueOrDefault()`                                                  | Extract value or type default             |
| **Async Error Handling**   |                                                                     |                                           |
| `Ensure`                   | `Ensure(Func<TIn, bool>, Func<TIn, Failure>)`                       | Validate with sync predicate              |
| `Ensure`                   | `Ensure(Func<TIn, ValueTask<bool>>, Func<TIn, Failure>)`            | Validate with async predicate             |
| `Recover`                  | `Recover(Func<Failure, TIn>)`                                       | Recover from failure (sync)               |
| `Recover`                  | `Recover(Func<Failure, ValueTask<TIn>>)`                            | Recover from failure (async)              |
| `Compensate`               | `Compensate(Func<Failure, Result>)`                                 | Execute rollback on failure (sync)        |
| `Compensate`               | `Compensate(Func<Failure, ValueTask<Result>>)`                      | Execute rollback on failure (async)       |
| **Async Side Effects**     |                                                                     |                                           |
| `TapFailure`               | `TapFailure(Action<Failure>)`                                       | Execute side effect on failure (sync)     |
| `TapFailure`               | `TapFailure(Func<Failure, ValueTask>)`                              | Execute side effect on failure (async)    |

## Creating Results

### Success Results

```csharp
// Non-generic success
var result = Result.Success();
var result = Result.Ok();

// Generic success with value
var result = Result.Success(42);
var result = Result.Ok(42);

// Implicit conversion from value
Result<int> result = 42;
```

### Failure Results

```csharp
// From exception
var result = Result.Failure(new Exception("Error"));
var result = Result.Fail(new Exception("Error"));

// From Failure instance
var result = Result.Failure(new ValidationFailure("Invalid input"));
var result = Result.Fail(new ValidationFailure("Invalid input"));

// From message (wraps in ExceptionalFailure)
var result = Result.Failure("Something went wrong");
var result = Result.Fail("Something went wrong");

// From multiple errors (creates AggregateFailure)
var result = Result.Failure(error1, error2, error3);
var result = Result.Fail(error1, error2, error3);

// Implicit conversion from Failure
Result result = new ValidationFailure("Invalid");
Result<int> result = new NotFoundFailure("User", "123");
```

### Specialized Failure Factories

```csharp
// Not Found
var result = Result.FailNotFound("User", "123");
var result = Result.FailNotFound<User>("User", "123", "Custom message");

// Validation
var result = Result.FailValidation("Invalid email format");
var result = Result.FailValidation<Email>("Invalid email format");

// Unauthenticated
var result = Result.FailUnauthenticated("Token expired");
var result = Result.FailUnauthenticated<User>("Token expired");

// Unauthorized
var result = Result.FailUnauthorized("Insufficient permissions");
var result = Result.FailUnauthorized<Resource>("Insufficient permissions");

// Conflict
var result = Result.FailConflict("Resource already exists");
var result = Result.FailConflict<Resource>("Resource already exists");
```

## Pattern Matching

### Match (Non-generic Result)

Execute actions based on success or failure:

```csharp
result.Match(
    onSuccess: () => Console.WriteLine("Success!"),
    onFailure: error => Console.WriteLine($"Error: {error.Message}")
);

// Return a value
var message = result.Match(
    onSuccess: () => "Operation completed",
    onFailure: error => $"Failed: {error.Message}"
);
```

### Match (Generic Result)

```csharp
// With value access
result.Match(
    success: value => Console.WriteLine($"Got: {value}"),
    failure: error => Console.WriteLine($"Error: {error.Message}")
);

// Without value access
result.Match(
    success: () => Console.WriteLine("Success!"),
    failure: error => Console.WriteLine($"Error: {error.Message}")
);

// Return a value
var output = result.Match(
    success: value => value.ToString(),
    failure: error => $"Error: {error.Message}"
);
```

### Switch

Alias for `Match`:

```csharp
result.Switch(
    onSuccess: () => DoSomething(),
    onFailure: error => HandleError(error)
);
```

### IfSuccess / IfFailure

Execute side effects conditionally:

```csharp
result.IfSuccess(() => Console.WriteLine("Succeeded"));
result.IfSuccess(value => Console.WriteLine($"Value: {value}"));
result.IfFailure(error => LogError(error));
```

## Transformations

### Bind

Chain operations that return `Result`, propagating failures (monadic bind):

```csharp
// Non-generic to non-generic
Result result = GetData()
    .Bind(() => ProcessData())
    .Bind(() => SaveData());

// Non-generic to generic
Result<int> result = GetData()
    .Bind(() => CalculateValue());

// Generic to generic
Result<User> result = GetUserId()
    .Bind(id => FetchUser(id))
    .Bind(user => ValidateUser(user));

// Generic to non-generic
Result result = GetUser()
    .Bind(user => DeleteUser(user));

// Async variants
var result = await GetData()
    .Bind(async () => await ProcessDataAsync());

var result = await GetUserId()
    .Bind(async id => await FetchUserAsync(id));
```

### Map

Transform success values without changing the Result wrapper:

```csharp
// Transform value
Result<string> result = Result.Success(42)
    .Map(x => x.ToString());

// Map from non-generic
Result<int> result = Result.Success()
    .Map(() => 42);

// Chain transformations
var result = GetUser()
    .Map(user => user.Email)
    .Map(email => email.ToLowerInvariant());
```

### Then

Chain transformations that return a Result of the same type:

```csharp
var result = GetUser()
    .Then(user => ValidateUser(user))
    .Then(user => EnrichUser(user));

// Chain Result<T> with Result
var result = GetUser()
    .Then(user => CheckPermissions(user)); // Returns Result
```

### Flatten

Remove one level of nesting from nested Results:

```csharp
Result<Result<int>> nested = GetNestedResult();
Result<int> flattened = nested.Flatten();
```

## Error Handling

### Recover

Provide fallback values when a result fails:

```csharp
// With fallback value
var value = GetUser()
    .Recover(defaultUser);

// With factory function
var value = GetUser()
    .Recover(error => CreateGuestUser());
```

### Compensate

Execute rollback logic on failure, combining errors if rollback also fails:

```csharp
var result = CreateOrder()
    .Compensate(originalError => RollbackOrder());

// Aggregates both errors if rollback fails
var result = CreateOrder()
    .Compensate(() => RollbackInventory());
```

### Try

Execute code that may throw exceptions, converting them to Result failures:

```csharp
// Execute action
var result = Result.Success()
    .Try(() => RiskyOperation());

// Transform value with potential exception
var result = GetInput()
    .Try(input => ParseInput(input)); // May throw
```

### Ensure

Validate result values with a predicate:

```csharp
var result = GetUser()
    .Ensure(
        user => user.Age >= 18,
        user => new ValidationFailure($"User must be 18+")
    );
```

## Utilities

### Tap

Execute side effects without changing the result:

```csharp
// Tap on success
var result = GetUser()
    .Tap(user => LogUser(user))
    .Tap(() => IncrementCounter());

// Conditional tap
var result = GetUser()
    .TapIf(
        user => user.IsAdmin,
        user => LogAdminAccess(user)
    );

// Tap on error
var result = GetUser()
    .TapError(error => LogError(error));
```

### ValueOr

Extract value with fallback for failures (terminates the railway):

```csharp
// With fallback value
var value = GetUser().ValueOr(defaultUser);

// With factory
var value = GetUser().ValueOr(() => CreateGuestUser());

// Get default value of type
var value = GetUser().ValueOrDefault(); // Returns null if failed

// Throw on failure
var value = GetUser().ValueOrThrow(); // Throws aggregated exception

// Throw custom exception
var value = GetUser().ValueOrThrow(
    error => new CustomException(error.Message)
);
```

### Combine

Combine multiple results, aggregating errors:

```csharp
// Combine non-generic results
IEnumerable<Result> results = GetResults();
Result combined = results.Combine();
// Success if all succeeded, otherwise AggregateFailure

// Combine generic results
IEnumerable<Result<int>> results = GetValues();
Result<IEnumerable<int>> combined = results.Combine();
// Success with all values, or AggregateFailure
```

## Metadata

Results support attaching metadata for contextual information:

```csharp
// Add single key-value pair
var result = Result.Success()
    .WithMetadata("RequestId", "12345");

// Add multiple pairs
var result = Result.Success()
    .WithMetadata(
        ("RequestId", "12345"),
        ("UserId", "user-123")
    );

// Merge metadata
var result = Result.Success()
    .WithMetadata(existingMetadata);

// Use builder
var result = Result.Success()
    .WithMetadata(builder => {
        builder.Add("RequestId", "12345");
        builder.Add("Timestamp", DateTime.UtcNow);
    });

// Metadata is preserved through transformations
var result = GetData()
    .WithMetadata("Operation", "Fetch")
    .Bind(() => ProcessData()) // Metadata flows through
    .Map(x => x.ToString());
```

## LINQ Support

Results support LINQ query syntax:

```csharp
// Select (Map)
var result = from user in GetUser()
             select user.Name;

// SelectMany (Bind)
var result = from userId in GetUserId()
             from user in FetchUser(userId)
             select user;

// Where (filter with validation)
var result = from user in GetUser()
             where user.Age >= 18
             select user;
// Fails with ValidationFailure if predicate is false

// Complex queries
var result = from order in GetOrder()
             from customer in GetCustomer(order.CustomerId)
             where customer.IsActive
             from address in GetAddress(customer.AddressId)
             select new OrderDetails(order, customer, address);
```

## Best Practices

### 1. Use Railway-Oriented Programming

Chain operations and let failures propagate automatically:

```csharp
// Good
var result = GetUserId()
    .Bind(id => FetchUser(id))
    .Map(user => user.Email)
    .Ensure(email => email.Contains("@"),
            _ => new ValidationFailure("Invalid email"));

// Avoid manually checking each step
var userIdResult = GetUserId();
if (userIdResult.IsFailure) return userIdResult.ToResult();
// ...
```

### 2. Prefer Specific Failure Types

Use specialized failure factories for better semantics:

```csharp
// Good
return Result.FailNotFound("User", userId);
return Result.FailValidation("Email is required");

// Less clear
return Result.Failure("User not found");
```

### 3. Use Metadata for Context

Attach contextual information without polluting the result:

```csharp
return GetUser(id)
    .WithMetadata("UserId", id)
    .WithMetadata("Timestamp", DateTime.UtcNow);
```

### 4. Handle Errors at Boundaries

Keep most code using `Result`, convert to exceptions only at system boundaries:

```csharp
// In API controller
public IActionResult GetUser(string id)
{
    return GetUserById(id)
        .Match(
            success: user => Ok(user),
            failure: error => error switch
            {
                NotFoundFailure => NotFound(error.Message),
                ValidationFailure => BadRequest(error.Message),
                _ => StatusCode(500, error.Message)
            }
        );
}
```

### 5. Leverage LINQ for Readability

Use LINQ syntax for complex chains:

```csharp
var result =
    from order in GetOrder(orderId)
    from customer in GetCustomer(order.CustomerId)
    from payment in ProcessPayment(order.Total)
    select new { order, customer, payment };
```

### 6. Use Tap for Side Effects

Don't break the chain for logging or other side effects:

```csharp
var result = GetUser(id)
    .Tap(user => _logger.LogInformation("User loaded: {Id}", user.Id))
    .Map(user => user.ToDto())
    .TapError(error => _logger.LogError("Failed: {Error}", error.Message));
```

### 7. Compensate for Transactional Rollbacks

Use `Compensate` for saga patterns or compensating transactions:

```csharp
var result = ReserveInventory(productId, quantity)
    .Bind(() => ChargePayment(amount))
    .Compensate(error => ReleaseInventory(productId, quantity));
```

### 8. Combine for Parallel Operations

Use `Combine` when validating multiple independent results:

```csharp
var results = new[]
{
    ValidateName(input.Name),
    ValidateEmail(input.Email),
    ValidateAge(input.Age)
};

return results.Combine(); // Returns all validation errors if any fail
```

### 9. Type Safety with Constraints

`TValue` must be `notnull` - design your domain types accordingly:

```csharp
// Good
Result<User> GetUser(string id);
Result<IEnumerable<Order>> GetOrders();

// Not allowed
Result<string?> GetNullableValue(); // Compile error
```

### 10. Implicit Conversions

Leverage implicit conversions for cleaner code:

```csharp
public Result<User> CreateUser(string name)
{
    if (string.IsNullOrEmpty(name))
        return new ValidationFailure("Name required"); // Implicit conversion

    var user = new User(name);
    return user; // Implicit conversion
}
```
