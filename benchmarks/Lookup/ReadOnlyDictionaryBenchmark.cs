using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PracticalDotNetCollections.Benchmarks;

[MemoryDiagnoser]
public class ReadOnlyDictionaryBenchmark
{
    private Dictionary<int, int> _dict = null!;
    private ReadOnlyDictionary<int, int> _readOnly = null!;

    [Params(10, 100, 1_000, 10_000, 100_000)]
    public int N;

    [GlobalSetup]
    public void Setup()
    {
        _dict = new Dictionary<int, int>(N);
        for (int i = 0; i < N; i++)
            _dict[i] = i;

        _readOnly = new ReadOnlyDictionary<int, int>(_dict);
    }

    [Benchmark]
    public bool DictionaryLookup() => _dict.ContainsKey(N / 2);

    [Benchmark]
    public bool ReadOnlyDictionaryLookup() => _readOnly.ContainsKey(N / 2);
}
