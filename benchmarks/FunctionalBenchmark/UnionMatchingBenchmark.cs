#if NET11_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using UnambitiousFx.Functional;
using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Benchmarks.FunctionalBenchmark;

/// <summary>
///     Proves the C# 15 union <c>switch</c> matching on <see cref="Result{TValue}" /> / <see cref="Maybe{TValue}" />
///     is allocation-free for value-type cases (routed through the non-boxing <c>TryGetValue</c> pattern),
///     on par with <c>Match</c>, while reading the <c>object</c>-typed <c>Value</c> boxes.
/// </summary>
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
[Config(typeof(Config))]
[ExcludeFromCodeCoverage]
public class UnionMatchingBenchmark
{
    private const int A = 42;

    private readonly Maybe<int>  _maybeSome = Maybe.Some(A);
    private readonly Result<int> _success   = Result.Success(A);

    // Result<int> success --------------------------------------------------

    [Benchmark(Description = "Result switch (union)")]
    public int Result_Switch()
    {
        return _success switch {
            int value => value + 1,
            Failure _ => -1,
        };
    }

    [Benchmark(Baseline = true, Description = "Result Match")]
    public int Result_Match()
    {
        return _success.Match(v => v + 1, _ => -1);
    }

    [Benchmark(Description = "Result .Value (boxes)")]
    public int Result_Value()
    {
        return _success.Value is int value
                   ? value + 1
                   : -1;
    }

    // Maybe<int> some ------------------------------------------------------

    [Benchmark(Description = "Maybe switch (union)")]
    public int Maybe_Switch()
    {
        return _maybeSome switch {
            int value => value + 1,
            null      => -1,
        };
    }

    [Benchmark(Description = "Maybe Match")]
    public int Maybe_Match()
    {
        return _maybeSome.Match(v => v + 1, () => -1);
    }

    [Benchmark(Description = "Maybe .Value (boxes)")]
    public int Maybe_Value()
    {
        return _maybeSome.Value is int value
                   ? value + 1
                   : -1;
    }

    private class Config : ManualConfig
    {
        public Config()
        {
            AddJob(Job.Default
                      .WithToolchain(InProcessEmitToolchain.Instance));
        }
    }
}
#endif
