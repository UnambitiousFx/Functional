# UnambitiousFx.Functional — Guidelines

Lightweight, performance-focused functional programming library for .NET. Provides type-safe primitives (`Result<T>`, `Maybe<T>`) with minimal allocations. Namespace: `UnambitiousFx.Functional`. Solution: `Functional.slnx`.

## Layout

| Path | Purpose |
|------|---------|
| `src/Functional` | Core library — `Result`/`Result<T>` (`Result.cs`), `Maybe<T>` (`Maybe.cs`), extensions |
| `src/Functional.AspNetCore` | ASP.NET Core integration |
| `src/Functional.xunit` | Fluent assertions for testing functional types |
| `test/` | Test projects (`Functional.Tests`, `Functional.AspNetCore.Tests`, `Functional.xunit.Tests`) |
| `benchmarks/FunctionalBenchmark` | BenchmarkDotNet benchmarks |
| `examples/` | `BasicUsage`, `AspNetCoreExample` |

Note: `OneOf` is the external NuGet package (v3.0.271), not a library-owned type.

## Principles

- **Zero-allocation**: minimize heap allocs in hot paths; use `readonly struct` for core types.
- **Railway-oriented**: compose with `Bind`/`Map`/`Tap`; control flow over exceptions.
- **Type-safe**: lean on the type system for compile-time correctness.
- **Modern C#**: file-scoped namespaces, pattern matching, records, collection expressions.

## Conventions

- File-scoped namespaces (`namespace UnambitiousFx.Functional;`).
- Naming: `PascalCase` (public/types/methods), `camelCase` (params/locals), `_camelCase` (private fields), `IPascalCase` (interfaces).
- Always use braces, even single statements.
- XML docs (`<summary>`/`<param>`/`<returns>`) on all public APIs.
- Prefer `ValueTask<T>` over `Task<T>` for likely-synchronous methods.
- Multi-target `net8.0;net9.0;net10.0` (latest net10.0); `LangVersion=latest`, `Nullable` + `ImplicitUsings` enabled — set in `build.props`.
- Central Package Management: add/bump versions in `Directory.Packages.props`, not in `.csproj`.

## Testing

Target 80%+ coverage. **Must use `Functional.xunit`** for asserting on `Result`/`Maybe`/`OneOf`.

- Name tests `MethodName_Scenario_ExpectedBehavior`.
- AAA = Gherkin (Given-When-Then); comment the sections.
- `[Theory]` + `[InlineData]` for multi-scenario logic.
- Cover edge cases: nulls, empty collections, boundaries, error states.
- Test public API behavior, not internal/private state.
- `NSubstitute` for external deps only; use real instances of core functional types.

Fluent assertions (`Functional.xunit`):

- `Result`: `result.ShouldBe().Success()` / `.Failure()`
- `Maybe`: `maybe.ShouldBe().Some()` / `.None()`
- `OneOf`: `oneOf.ShouldBe().First()` / `.Second()` …
- Chain `.And(v => …)` / `.Where(v => …)`; failures: `.AndMessage("…")` / `.AndCode("…")`.

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

## Performance

For hot-path changes, add/update benchmarks in `benchmarks/FunctionalBenchmark` (BenchmarkDotNet).

## Build & Test

```bash
dotnet build Functional.slnx
dotnet test
```

## Commits & Branches

semantic-release derives the next version from commit messages (see `CONTRIBUTING.md`), so these rules decide what gets released.

- Commits and PR titles follow [Conventional Commits](https://www.conventionalcommits.org/): `type(scope)?: summary`, imperative, lowercase, no trailing period.
- `feat` → minor, `fix`/`perf` → patch. `docs`, `test`, `refactor`, `build`, `ci`, `chore`, `style`, `revert` → no release.
- Breaking change: `type!:` or a `BREAKING CHANGE:` footer → major. Required for any public API break.
- PRs are squash-merged with the PR title as the commit message: the PR title is what counts (checked by `pr-title.yml`).
- Branches: `<type>/<issue>-<slug>`, or `<type>/<slug>` without an issue; kebab-case, `type` from the list above (e.g. `feat/148-fluentvalidation`, `fix/95-hide-500-details`, `ci/semantic-release`).

## Labels

Always label a PR or issue when opening it (`gh pr create --label`, `gh issue create --label`). Use existing labels only (`gh label list`), never create new ones, and add every label that applies (a `feat!:` gets `enhancement` + `breaking change`).

| Change | Label |
|---|---|
| `feat` / feature request | `enhancement` |
| `fix` / bug report | `bug` |
| `docs` | `documentation` |
| `ci` | `ci` |
| dependency update | `dependencies` |
| `!` / `BREAKING CHANGE` | `breaking change` |
