using BenchmarkDotNet.Running;
using UnambitiousFx.Benchmarks.FunctionalBenchmark;

BenchmarkSwitcher.FromTypes([
    typeof(ResultComparisonBenchmark)
]).Run(args);
