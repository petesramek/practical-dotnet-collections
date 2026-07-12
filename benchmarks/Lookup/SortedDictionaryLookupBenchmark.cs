using BenchmarkDotNet.Attributes;

namespace PracticalDotNetCollections.Benchmarks.Lookup;

[MemoryDiagnoser]
public class SortedDictionaryLookupBenchmark {
    private SortedDictionary<int, int> _sorted = null!;
    private Dictionary<int, int> _dict = null!;

    [Params(10, 100, 1_000, 10_000, 100_000)]
    public int N;

    [GlobalSetup]
    public void Setup() {
        _sorted = new SortedDictionary<int, int>();
        _dict = new Dictionary<int, int>();

        for (int i = 0; i < N; i++) {
            _sorted[i] = i;
            _dict[i] = i;
        }
    }

    [Benchmark]
    public bool SortedDictionaryLookup() => _sorted.ContainsKey(N / 2);

    [Benchmark]
    public bool DictionaryLookup() => _dict.ContainsKey(N / 2);
}
