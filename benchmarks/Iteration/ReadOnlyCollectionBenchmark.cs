using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PracticalDotNetCollections.Benchmarks;

[MemoryDiagnoser]
public class ReadOnlyCollectionBenchmark
{
    private List<int> _list = null!;
    private ReadOnlyCollection<int> _readOnly = null!;

    [Params(10, 100, 1_000, 10_000, 100_000)]
    public int N;

    [GlobalSetup]
    public void Setup()
    {
        _list = new List<int>(N);
        for (int i = 0; i < N; i++)
            _list.Add(i);

        _readOnly = _list.AsReadOnly();
    }

    [Benchmark]
    public int IterateList()
    {
        var sum = 0;
        foreach (var item in _list)
            sum += item;
        return sum;
    }

    [Benchmark]
    public int IterateReadOnlyCollection()
    {
        var sum = 0;
        foreach (var item in _readOnly)
            sum += item;
        return sum;
    }
}
