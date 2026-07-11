using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.Collections.Frozen;
using System.Linq;

namespace PracticalDotNetCollections.Benchmarks;

[MemoryDiagnoser]
public class FrozenSetLookupBenchmark
{
    private FrozenSet<int> _frozen = null!;
    private HashSet<int> _hash = null!;

    [Params(10, 100, 1_000, 10_000, 100_000)]
    public int N;

    [GlobalSetup]
    public void Setup()
    {
        var data = Enumerable.Range(0, N);
        _frozen = data.ToFrozenSet();
        _hash = data.ToHashSet();
    }

    [Benchmark]
    public bool FrozenSetContains() => _frozen.Contains(N / 2);

    [Benchmark]
    public bool HashSetContains() => _hash.Contains(N / 2);
}
