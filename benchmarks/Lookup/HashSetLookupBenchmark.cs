using BenchmarkDotNet.Attributes;

namespace PracticalDotNetCollections.Benchmarks;

[MemoryDiagnoser]
public class HashSetLookupBenchmark {
    private HashSet<int> _set = null!;
    private List<int> _list = null!;

    [Params(10, 100, 1_000, 10_000, 100_000)]
    public int N;

    [GlobalSetup]
    public void Setup() {
        _set = Enumerable.Range(0, N).ToHashSet();
        _list = Enumerable.Range(0, N).ToList();
    }

    [Benchmark]
    public bool HashSetLookup()
        => _set.Contains(N / 2);

    [Benchmark]
    public bool ListLookup()
        => _list.Contains(N / 2);
}
