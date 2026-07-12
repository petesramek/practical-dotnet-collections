using BenchmarkDotNet.Attributes;
using System.Collections.Immutable;

namespace PracticalDotNetCollections.Benchmarks.Memory;

[MemoryDiagnoser]
public class ImmutableDictionaryBenchmark {
    [Params(10, 100, 1_000, 10_000)]
    public int N;

    [Benchmark]
    public ImmutableDictionary<int, int> ImmutableDictionaryAdd() {
        var dict = ImmutableDictionary<int, int>.Empty;

        for (int i = 0; i < N; i++)
            dict = dict.Add(i, i);

        return dict;
    }

    [Benchmark]
    public Dictionary<int, int> DictionaryAdd() {
        var dict = new Dictionary<int, int>();

        for (int i = 0; i < N; i++)
            dict[i] = i;

        return dict;
    }
}
