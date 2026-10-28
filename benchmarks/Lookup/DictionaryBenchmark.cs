using BenchmarkDotNet.Attributes;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PracticalDotNetCollections.Benchmarks.Lookup;

/// <summary>
/// Struct without equality overrides used to demonstrate boxing overhead.
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
/// Examines lookup scaling, key lookups versus lists, multi-lookup update patterns, and struct key boxing overhead.
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
    /// ContainsKey, indexer get, indexer set: three hash lookups per update (a fourth is the final read of the result).
    /// </summary>
    [Benchmark]
    public int FourLookupUpdate() {
        int key = _searchKeys[_searchIndex++ % (uint)_searchKeys.Length];
        if (_dictionary.ContainsKey(key)) {
            _dictionary[key] = _dictionary[key] + 1;
        }
        return _dictionary[key];
    }

    /// <summary>
    /// TryGetValue followed by an indexer set: two hash lookups per update.
    /// </summary>
    [Benchmark]
    public int TwoLookupTryGetValueUpdate() {
        int key = _searchKeys[_searchIndex++ % (uint)_searchKeys.Length];
        int value = 0;
        if (_dictionary.TryGetValue(key, out value)) {
            value++;
            _dictionary[key] = value;
        }
        return value;
    }

    /// <summary>
    /// CollectionsMarshal.GetValueRefOrNullRef: a single hash lookup, then the value is updated in place through a ref.
    /// </summary>
    [Benchmark]
    public int SingleLookupGetValueRefUpdate() {
        int key = _searchKeys[_searchIndex++ % (uint)_searchKeys.Length];
        ref int value = ref CollectionsMarshal.GetValueRefOrNullRef(_dictionary, key);
        if (!Unsafe.IsNullRef(ref value)) {
            value++;
            return value;
        }
        return 0;
    }

    /// <summary>
    /// Lookups using a struct key without IEquatable: the default comparer falls back to ValueType.Equals/GetHashCode and boxes.
    /// </summary>
    [Benchmark]
    public int LookupWithUnoptimizedStructKey() {
        var key = _unoptimizedSearchKeys[_searchIndex++ % (uint)_unoptimizedSearchKeys.Length];
        return _unoptimizedStructMap[key];
    }

    /// <summary>
    /// Lookups using a struct key implementing IEquatable, which avoids boxing.
    /// </summary>
    [Benchmark]
    public int LookupWithOptimizedStructKey() {
        var key = _optimizedSearchKeys[_searchIndex++ % (uint)_optimizedSearchKeys.Length];
        return _optimizedStructMap[key];
    }
}
