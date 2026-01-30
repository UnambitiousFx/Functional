using Ardalis.Result;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using FluentResult = FluentResults.Result;

namespace UnambitiousFx.Benchmarks.FunctionalBenchmark;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
[Config(typeof(Config))]
public class ResultComparisonBenchmark
{
    private const int A = 42;

    private const string ErrorMessage = "boom";

    private readonly Result<int> _ardalisFailure;
    private readonly Result<int> _ardalisSuccess;
    private readonly FluentResults.Result<int> _fluentFailure;
    private readonly FluentResults.Result<int> _fluentSuccess = FluentResult.Ok(A);

    private readonly Functional.Result<int> _ourFailure;

    private readonly Functional.Result<int> _ourSuccess = Functional.Result.Success(A);

    public ResultComparisonBenchmark()
    {
        // Use string-based failure to keep all libraries comparable
        _ourFailure = Functional.Result.Failure<int>(ErrorMessage);
        _fluentFailure = FluentResult.Fail<int>(ErrorMessage);
        _ardalisFailure = Result<int>.Error(ErrorMessage);
        _ardalisSuccess = Result<int>.Success(A);
    }

    // Creation (Success)
    [Benchmark(Description = "Create Success (Our)")]
    public Functional.Result<int> Create_Success_Our()
    {
        return Functional.Result.Success(A);
    }

    [Benchmark(Description = "Create Success (FluentResults)")]
    public FluentResults.Result<int> Create_Success_Fluent()
    {
        return FluentResult.Ok(A);
    }

    [Benchmark(Description = "Create Success (Ardalis)")]
    public Result<int> Create_Success_Ardalis()
    {
        return Result<int>.Success(A);
    }

    // Creation (Failure)
    [Benchmark(Description = "Create Failure (Our)")]
    public Functional.Result<int> Create_Failure_Our()
    {
        return Functional.Result.Failure<int>(ErrorMessage);
    }

    [Benchmark(Description = "Create Failure (FluentResults)")]
    public FluentResults.Result<int> Create_Failure_Fluent()
    {
        return FluentResult.Fail<int>(ErrorMessage);
    }

    [Benchmark(Description = "Create Failure (Ardalis)")]
    public Result<int> Create_Failure_Ardalis()
    {
        return Result<int>.Error(ErrorMessage);
    }

    // Match/Access on Success
    [Benchmark(Description = "Access Success (Our: Match)")]
    public int Access_Success_Our()
    {
        return _ourSuccess.Match(v => v + 1, _ => 0);
    }

    [Benchmark(Description = "Access Success (FluentResults: IsSuccess/Value)")]
    public int Access_Success_Fluent()
    {
        return _fluentSuccess.IsSuccess
            ? _fluentSuccess.Value + 1
            : 0;
    }

    [Benchmark(Description = "Access Success (Ardalis: IsSuccess/Value)")]
    public int Access_Success_Ardalis()
    {
        return _ardalisSuccess.IsSuccess
            ? _ardalisSuccess.Value + 1
            : 0;
    }

    // Match/Access on Failure
    [Benchmark(Description = "Access Failure (Our: Match)")]
    public int Access_Failure_Our()
    {
        return _ourFailure.Match(v => v + 1, _ => -1);
    }

    [Benchmark(Description = "Access Failure (FluentResults: IsSuccess/Value)")]
    public int Access_Failure_Fluent()
    {
        return _fluentFailure.IsSuccess
            ? _fluentFailure.Value + 1
            : -1;
    }

    [Benchmark(Description = "Access Failure (Ardalis: IsSuccess)")]
    public int Access_Failure_Ardalis()
    {
        return _ardalisFailure.IsSuccess
            ? _ardalisFailure.Value + 1
            : -1;
    }

    // Ok/TryGet-like on Success
    [Benchmark(Description = "Ok Success (Our)")]
    public int Ok_Success_Our()
    {
        _ourSuccess.TryGetValue(out var value);
        return value + 1;
    }

    [Benchmark(Description = "Ok Success (FluentResults: IsSuccess + Value)")]
    public int Ok_Success_Fluent()
    {
        if (_fluentSuccess.IsSuccess)
        {
            var value = _fluentSuccess.Value;
            return value + 1;
        }

        return 0;
    }

    [Benchmark(Description = "Ok Success (Ardalis: IsSuccess + Value)")]
    public int Ok_Success_Ardalis()
    {
        if (_ardalisSuccess.IsSuccess)
        {
            var value = _ardalisSuccess.Value;
            return value + 1;
        }

        return 0;
    }

    // Ok/TryGet-like on Failure
    [Benchmark(Description = "Ok Failure (Our)")]
    public bool Ok_Failure_Our()
    {
        return _ourFailure.TryGetValue(out var _);
    }

    [Benchmark(Description = "Ok Failure (FluentResults: IsSuccess)")]
    public bool Ok_Failure_Fluent()
    {
        return _fluentFailure.IsSuccess;
    }

    [Benchmark(Description = "Ok Failure (Ardalis: IsSuccess)")]
    public bool Ok_Failure_Ardalis()
    {
        return _ardalisFailure.IsSuccess;
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