using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

namespace PracticalDotNetCollections.Benchmarks;

[MemoryDiagnoser]
public class ListCapacityBenchmark
{
    [Params(10, 100, 1_000, 10_000, 100_000)]
    public int N;

    private List<int> _default = null!;
    private List<int> _preallocated = null!;

    [IterationSetup]
    public void IterationSetup()
    {
        _default = new List<int>();
        _preallocated = new List<int>(N);
    }

    [Benchmark]
    public void DefaultListAdd()
    {
        for (int i = 0; i < N; i++)
            _default.Add(i);
    }

    [Benchmark]
    public void PreallocatedListAdd()
    {
        for (int i = 0; i < N; i++)
            _preallocated.Add(i);
    }
}
