using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace PracticalDotNetCollections.Benchmarks;

[MemoryDiagnoser]
public class ImmutableHashSetBenchmark
{
    [Params(10, 100, 1_000, 10_000)]
    public int N;

    [Benchmark]
    public ImmutableHashSet<int> ImmutableHashSetAdd()
    {
        var set = ImmutableHashSet<int>.Empty;

        for (int i = 0; i < N; i++)
            set = set.Add(i);

        return set;
    }

    [Benchmark]
    public HashSet<int> HashSetAdd()
    {
        var set = new HashSet<int>();

        for (int i = 0; i < N; i++)
            set.Add(i);

        return set;
    }
}
