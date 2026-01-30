using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using OneOf;

namespace UnambitiousFx.Benchmarks.FunctionalBenchmark;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
[Config(typeof(Config))]
public class OneOfVsOneOfBenchmark
{
    private const int A = 42;
    private const string Message = "test";

    private readonly OneOf<string, int> _theirFirst = OneOf<string, int>.FromT0(Message);
    private readonly OneOf<string, int> _theirSecond = OneOf<string, int>.FromT1(A);
    private readonly Functional.OneOf<string, int> _ourFirst = Functional.OneOf<string, int>.FromFirst(Message);
    private readonly Functional.OneOf<string, int> _ourSecond = Functional.OneOf<string, int>.FromSecond(A);

    public OneOfVsOneOfBenchmark()
    {
    }

    // Creation (First)
    [Benchmark(Description = "Create First (Our)")]
    public Functional.OneOf<string, int> Create_First_Our()
    {
        return Functional.OneOf<string, int>.FromFirst(Message);
    }

    [Benchmark(Description = "Create First (OneOf)")]
    public OneOf<string, int> Create_First_Their()
    {
        return OneOf<string, int>.FromT0(Message);
    }

    // Creation (Second)
    [Benchmark(Description = "Create Second (Our)")]
    public Functional.OneOf<string, int> Create_Second_Our()
    {
        return Functional.OneOf<string, int>.FromSecond(A);
    }

    [Benchmark(Description = "Create Second (OneOf)")]
    public OneOf<string, int> Create_Second_Their()
    {
        return OneOf<string, int>.FromT1(A);
    }

    // Match on First
    [Benchmark(Description = "Match First (Our)")]
    public int Match_First_Our()
    {
        return _ourFirst.Match(_ => 1, v => v + 1);
    }

    [Benchmark(Description = "Match First (OneOf)")]
    public int Match_First_Their()
    {
        return _theirFirst.Match(_ => 1, v => v + 1);
    }

    // Match on Second
    [Benchmark(Description = "Match Second (Our)")]
    public int Match_Second_Our()
    {
        return _ourSecond.Match(_ => 1, v => v + 1);
    }

    [Benchmark(Description = "Match Second (OneOf)")]
    public int Match_Second_Their()
    {
        return _theirSecond.Match(_ => 1, v => v + 1);
    }

    // TryGet/TryPick on First
    [Benchmark(Description = "TryGet First (Our)")]
    public bool TryGet_First_Our()
    {
        return _ourFirst.TryGetFirst(out _);
    }

    [Benchmark(Description = "TryPick First (OneOf)")]
    public bool TryGet_First_Their()
    {
        return _theirFirst.TryPickT0(out _, out _);
    }

    // TryGet/TryPick on Second
    [Benchmark(Description = "TryGet Second (Our)")]
    public bool TryGet_Second_Our()
    {
        return _ourSecond.TryGetSecond(out _);
    }

    [Benchmark(Description = "TryPick Second (OneOf)")]
    public bool TryGet_Second_Their()
    {
        return _theirSecond.TryPickT1(out _, out _);
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
