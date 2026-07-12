using BenchmarkDotNet.Attributes;

namespace PracticalDotNetCollections.Benchmarks.Lookup;

/// <summary>
/// Struct without equality overrides used to demonstrate boxing and reflection overhead.
/// </summary>
public struct UnoptimizedKey {
    public int Id;
    public UnoptimizedKey(int id) => Id = id;
}

/// <summary>
/// Struct with explicit equality overrides providing direct, allocation-free evaluation path.
/// </summary>
public struct OptimizedKey : IEquatable<OptimizedKey> {
    public int Id;
    public OptimizedKey(int id) => Id = id;

    public bool Equals(OptimizedKey other) => Id == other.Id;
    public override bool Equals(object? obj) => obj is OptimizedKey other && Equals(other);
    public override int GetHashCode() => Id;
}

/// <summary>
/// Benchmarks evaluating <see cref="Dictionary{TKey, TValue}"/> performance footprints.
/// Examines lookup scaling, key lookups versus lists, double-lookup traps, and struct key boxing overhead.
/// </summary>
[MemoryDiagnoser]
public class DictionaryBenchmark {
    private Dictionary<int, int> _dictionary = null!;
    private List<int> _list = null!;
    private int[] _searchKeys = null!;

    // Changed to uint to guarantee it remains positive during numeric overflow wrap-arounds
    private uint _searchIndex;

    private Dictionary<UnoptimizedKey, int> _unoptimizedStructMap = null!;
    private Dictionary<OptimizedKey, int> _optimizedStructMap = null!;
    private UnoptimizedKey[] _unoptimizedSearchKeys = null!;
    private OptimizedKey[] _optimizedSearchKeys = null!;

    /// <summary>
    /// The number of entries processed inside the collections.
    /// </summary>
    [Params(10_000, 100_000)]
    public int N;

    /// <summary>
    /// Sets up and populates maps, lists, and randomized search indices.
    /// </summary>
    [GlobalSetup]
    public void Setup() {
        var random = new Random(42);

        _dictionary = Enumerable.Range(0, N).ToDictionary(x => x);
        _list = Enumerable.Range(0, N).ToList();

        _searchKeys = Enumerable.Range(0, 1000).Select(_ => random.Next(0, N)).ToArray();

        _unoptimizedStructMap = Enumerable.Range(0, N).ToDictionary(x => new UnoptimizedKey(x), x => x);
        _optimizedStructMap = Enumerable.Range(0, N).ToDictionary(x => new OptimizedKey(x), x => x);

        _unoptimizedSearchKeys = _searchKeys.Select(k => new UnoptimizedKey(k)).ToArray();
        _optimizedSearchKeys = _searchKeys.Select(k => new OptimizedKey(k)).ToArray();
    }

    /// <summary>
    /// Measures flat hash table read scaling performance via localized target mapping.
    /// </summary>
    [Benchmark]
    public int DictionaryLookup() {
        int key = _searchKeys[_searchIndex++ % (uint)_searchKeys.Length];
        return _dictionary[key];
    }

    /// <summary>
    /// Measures sequential search degradation across array elements.
    /// </summary>
    [Benchmark]
    public bool ListLookup() {
        int key = _searchKeys[_searchIndex++ % (uint)_searchKeys.Length];
        return _list.Contains(key);
    }

    /// <summary>
    /// Demonstrates the common mistake: calling ContainsKey before accessing an indexer, forcing two hash table searches.
    /// </summary>
    [Benchmark]
    public int DoubleLookupUpdate() {
        int key = _searchKeys[_searchIndex++ % (uint)_searchKeys.Length];
        if (_dictionary.ContainsKey(key)) {
            _dictionary[key] = _dictionary[key] + 1;
        }
        return _dictionary[key];
    }

    /// <summary>
    /// Demonstrates the optimization fix: using TryGetValue to extract and update an item in a single pass.
    /// </summary>
    [Benchmark]
    public int SingleLookupTryGetValueUpdate() {
        int key = _searchKeys[_searchIndex++ % (uint)_searchKeys.Length];
        if (_dictionary.TryGetValue(key, out int value)) {
            _dictionary[key] = value + 1;
        }
        return value;
    }

    /// <summary>
    /// Highlights lookups using a raw struct key, triggering heavy internal CPU boxing and runtime reflection.
    /// </summary>
    [Benchmark]
    public int LookupWithUnoptimizedStructKey() {
        var key = _unoptimizedSearchKeys[_searchIndex++ % (uint)_unoptimizedSearchKeys.Length];
        return _unoptimizedStructMap[key];
    }

    /// <summary>
    /// Highlights lookups using an optimized struct key implementing IEquatable, keeping evaluations lock-free and allocation-free.
    /// </summary>
    [Benchmark]
    public int LookupWithOptimizedStructKey() {
        var key = _optimizedSearchKeys[_searchIndex++ % (uint)_optimizedSearchKeys.Length];
        return _optimizedStructMap[key];
    }
}
