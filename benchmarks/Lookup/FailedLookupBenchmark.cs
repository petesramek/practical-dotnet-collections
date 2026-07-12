using BenchmarkDotNet.Attributes;

namespace PracticalDotNetCollections.Benchmarks.Lookup;

[MemoryDiagnoser]
public class FailedLookupBenchmark {
    private List<int> _list = null!;
    private HashSet<int> _set = null!;
    private Dictionary<int, int> _dict = null!;

    [Params(10, 100, 1_000, 10_000, 100_000)]
    public int N;

    private int _missing;

    [GlobalSetup]
    public void Setup() {
        _list = Enumerable.Range(0, N).ToList();
        _set = new HashSet<int>(_list);
        _dict = _list.ToDictionary(x => x, x => x);

        _missing = -1; // guaranteed missing
    }

    [Benchmark]
    public bool List_FailedLookup()
        => _list.Contains(_missing);

    [Benchmark]
    public bool HashSet_FailedLookup()
        => _set.Contains(_missing);

    [Benchmark]
    public bool Dictionary_FailedLookup()
        => _dict.ContainsKey(_missing);
}
