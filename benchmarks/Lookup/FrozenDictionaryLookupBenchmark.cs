using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.Collections.Frozen;
using System.Linq;

namespace PracticalDotNetCollections.Benchmarks;

[MemoryDiagnoser]
public class FrozenDictionaryLookupBenchmark
{
    private FrozenDictionary<int, int> _frozen = null!;
    private Dictionary<int, int> _dict = null!;

    [Params(10, 100, 1_000, 10_000, 100_000)]
    public int N;

    [GlobalSetup]
    public void Setup()
    {
        var data = Enumerable.Range(0, N).ToDictionary(x => x, x => x);
        _frozen = data.ToFrozenDictionary();
        _dict = data;
    }

    [Benchmark]
    public bool FrozenDictionaryLookup() => _frozen.ContainsKey(N / 2);

    [Benchmark]
    public bool DictionaryLookup() => _dict.ContainsKey(N / 2);
}
