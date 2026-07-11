using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace PracticalDotNetCollections.Benchmarks;

[MemoryDiagnoser]
public class ImmutableArrayBenchmark
{
    [Params(10, 100, 1_000, 10_000)]
    public int N;

    [Benchmark]
    public ImmutableArray<int> ImmutableArrayAdd()
    {
        var array = ImmutableArray<int>.Empty;

        for (int i = 0; i < N; i++)
            array = array.Add(i);

        return array;
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
