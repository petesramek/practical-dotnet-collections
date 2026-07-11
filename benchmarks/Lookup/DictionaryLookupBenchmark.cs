using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace PracticalDotNetCollections.Benchmarks;

[MemoryDiagnoser]
public class DictionaryLookupBenchmark
{
    private Dictionary<int, int> _dictionary = null!;
    private List<int> _list = null!;

    [Params(10, 100, 1_000, 10_000, 100_000)]
    public int N;

    [GlobalSetup]
    public void Setup()
    {
        _dictionary = Enumerable.Range(0, N).ToDictionary(x => x);
        _list = Enumerable.Range(0, N).ToList();
    }

    [Benchmark]
    public int DictionaryLookup()
        => _dictionary[N / 2];

    [Benchmark]
    public bool ListLookup()
        => _list.Contains(N / 2);
}
