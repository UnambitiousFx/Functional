using BenchmarkDotNet.Running;
using UnambitiousFx.Benchmarks.FunctionalBenchmark;

BenchmarkSwitcher.FromTypes([
                      typeof(ResultComparisonBenchmark),
#if NET11_0_OR_GREATER
                      typeof(UnionMatchingBenchmark),
#endif
                  ])
                 .Run(args);
