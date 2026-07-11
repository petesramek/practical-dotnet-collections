using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

namespace PracticalDotNetCollections.Benchmarks;

[MemoryDiagnoser]
public class SortedListAddBenchmark
{
    [Params(10, 100, 1_000, 10_000)]
    public int N;

    [Benchmark]
    public void SortedListAdd()
    {
        var list = new SortedList<int, int>();

        for (int i = 0; i < N; i++)
            list.Add(i, i);
    }

    [Benchmark]
    public void SortedDictionaryAdd()
    {
        var dict = new SortedDictionary<int, int>();

        for (int i = 0; i < N; i++)
            dict.Add(i, i);
    }
}
