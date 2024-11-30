using BenchmarkDotNet.Running;

var summary = BenchmarkRunner.Run<Benchmarks.BacktrackerBenchmarks>();
