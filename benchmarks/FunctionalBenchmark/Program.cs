using BenchmarkDotNet.Running;
using UnambitiousFx.Benchmarks.FunctionalBenchmark;

BenchmarkSwitcher.FromTypes([
    typeof(ResultComparisonBenchmark),
    typeof(OneOfVsOneOfBenchmark)
]).Run(args);
