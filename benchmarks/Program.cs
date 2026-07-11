using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Collections.Generic;
using System.Linq;

namespace PracticalDotNetCollections.Benchmarks;

[MemoryDiagnoser]
public class DictionaryLookupBenchmark
{
    private Dictionary<int, int> _dictionary = null!;
    private List<int> _list = null!;

    [Params(1000, 10000, 100000)]
    public int N;

    [GlobalSetup]
    public void Setup()
    {
        _dictionary = Enumerable.Range(0, N).ToDictionary(x => x, x => x);
        _list = Enumerable.Range(0, N).ToList();
    }

    [Benchmark]
    public int DictionaryLookup()
    {
        return _dictionary[N / 2];
    }

    [Benchmark]
    public bool ListLookup()
    {
        return _list.Contains(N / 2);
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<DictionaryLookupBenchmark>();
    }
}
