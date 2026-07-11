using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

namespace PracticalDotNetCollections.Benchmarks;

[MemoryDiagnoser]
public class SortedSetLookupBenchmark
{
    private SortedSet<int> _sorted = null!;
    private HashSet<int> _hash = null!;

    [Params(10, 100, 1_000, 10_000, 100_000)]
    public int N;

    [GlobalSetup]
    public void Setup()
    {
        _sorted = new SortedSet<int>();
        _hash = new HashSet<int>();

        for (int i = 0; i < N; i++)
        {
            _sorted.Add(i);
            _hash.Add(i);
        }
    }

    [Benchmark]
    public bool SortedSetContains() => _sorted.Contains(N / 2);

    [Benchmark]
    public bool HashSetContains() => _hash.Contains(N / 2);
}
