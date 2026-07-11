using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace PracticalDotNetCollections.Benchmarks;

[MemoryDiagnoser]
public class ImmutableListBenchmark
{
    [Params(10, 100, 1_000, 10_000)]
    public int N;

    [Benchmark]
    public ImmutableList<int> ImmutableListAdd()
    {
        var list = ImmutableList<int>.Empty;

        for (int i = 0; i < N; i++)
            list = list.Add(i);

        return list;
    }

    [Benchmark]
    public List<int> ListAdd()
    {
        var list = new List<int>();

        for (int i = 0; i < N; i++)
            list.Add(i);

        return list;
    }
}
