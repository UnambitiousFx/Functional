# Guidelines for UnambitiousFx.Functional

This document provides guidelines and best practices for developing and maintaining the UnambitiousFx.Functional library.

## Project Overview

UnambitiousFx.Functional is a lightweight, performance-focused functional programming library for .NET. Its core goal is to provide type-safe, expressive functional primitives like `Result<T>`, `Maybe<T>`, and `OneOf<T>` while minimizing allocations and overhead.

### Core Principles

- **Zero-Allocation**: Strive for minimal heap allocations in hot paths. Use `readonly struct` for core types where appropriate.
- **Functional-First**: Embrace railway-oriented programming. Use composition (Bind, Map, Tap) over exceptions for control flow.
- **Type Safety**: Leverage the .NET type system and source generators to ensure correctness at compile time.
- **Modern C#**: Use modern C# features (file-scoped namespaces, pattern matching, records, collection expressions).

## Code Style and Conventions

- **Namespaces**: Use file-scoped namespaces (e.g., `namespace UnambitiousFx.Functional;`).
- **Naming**:
    - `PascalCase` for public members, types, and methods.
    - `camelCase` for parameters and local variables.
    - `_camelCase` for private fields.
    - `IPascalCase` for interfaces.
- **Braces**: Always use braces for control flow, even for single statements.
- **Documentation**: All public APIs must have XML documentation comments (`<summary>`, `<param>`, `<returns>`, etc.).
- **Async**: Prefer `ValueTask<T>` over `Task<T>` for methods that are likely to complete synchronously.

## Unit Testing Guidelines

Testing is a first-class citizen in this project. We aim for high coverage (80%+) and expressive, readable tests. For testing `Result`, `Maybe`, or `OneOf`, you must use `Functional.xunit` to ensure consistent and effective assertions.

### AAA Pattern (Gherkin style)

All tests should follow the **Arrange-Act-Assert (AAA)** pattern, which maps directly to the **Gherkin (Given-When-Then)** structure. Use comments to clearly separate these sections.

- **Arrange (Given)**: Set up the test conditions, initialize objects, and prepare input data.
- **Act (When)**: Execute the specific method or operation being tested.
- **Assert (Then)**: Verify that the outcome matches expectations using assertions.

#### Example

```csharp
[Fact]
public void Map_WithSuccessResult_TransformsValue()
{
    // Arrange (Given)
    var result = Result.Success(5);

    // Act (When)
    var mapped = result.Map(x => x * 2);

    // Assert (Then)
    mapped.ShouldBe()
        .Success()
        .And(value => Assert.Equal(10, value));
}
```

### Best Practices in .NET Testing

1.  **Descriptive Test Names**: Use the pattern `MethodName_Scenario_ExpectedBehavior`.
2.  **Theory Tests**: Use `[Theory]` and `[InlineData]` to test multiple scenarios with the same logic.
3.  **Edge Case Identification**: Always test nulls, empty collections, boundary conditions, and error states.
4.  **Use Custom Fluent Assertions**: Leverage the `Functional.xunit` library for expressive assertions on functional types (`Result`, `Maybe`, `OneOf`). This is the preferred way to ensure consistency across the test suite.
    - Use `result.ShouldBe().Success()` or `result.ShouldBe().Failure()` for `Result`.
    - Use `maybe.ShouldBe().Some()` or `maybe.ShouldBe().None()` for `Maybe`.
    - Use `oneOf.ShouldBe().First()`, `.Second()`, etc., for `OneOf`.
    - Chain with `.And(v => ...)` or `.Where(v => ...)` for further validation.
    - For errors, use `.AndMessage("expected message")` or `.AndCode("expected code")`.
5.  **Avoid Implementation Details**: Test the public API and its behavior, not the internal state or private methods.
6.  **Mocking**: Use `NSubstitute` when you need to mock external dependencies, but prefer using real instances of core functional types.

## Performance Testing

For changes in hot paths, add or update benchmarks in the `benchmarks/FunctionalBenchmark` project using **BenchmarkDotNet**.
