using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace PracticalDotNetCollections.Benchmarks;

public class Program {
    public static void Main(string[] args) {
        var config = ManualConfig
            .Create(DefaultConfig.Instance)
            .AddJob(Job.Default.WithWarmupCount(3).WithIterationCount(8));

        BenchmarkSwitcher
            .FromAssembly(typeof(Program).Assembly)
            .Run(args, config);
    }
}
