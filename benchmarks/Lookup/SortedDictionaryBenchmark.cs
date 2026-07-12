using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PracticalDotNetCollections.Benchmarks.Lookup;

/// <summary>
/// Benchmarks evaluating <see cref="SortedDictionary{TKey, TValue}"/> performance footprints against a standard hash-based <see cref="Dictionary{TKey, TValue}"/>.
/// Measures pointer-heavy Red-Black tree insertion overhead against flat hash bucket layouts alongside lookup scaling differences.
/// </summary>
[MemoryDiagnoser]
public class SortedDictionaryBenchmark {
    private SortedDictionary<int, int> _sorted = null!;
    private Dictionary<int, int> _dict = null!;
    private int[] _searchKeys = null!;
    private int[] _insertKeys = null!;
    private uint _searchIndex;

    /// <summary>
    /// The number of entries processed inside the map collections.
    /// </summary>
    [Params(1_000, 10_000)]
    public int N;

    /// <summary>
    /// Sets up and populates the long-lived tree and hash instances for reading evaluation tracks.
    /// </summary>
    [GlobalSetup]
    public void Setup() {
        var random = new Random(42);
        _sorted = new SortedDictionary<int, int>();
        _dict = new Dictionary<int, int>();

        for (int i = 0; i < N; i++) {
            _sorted[i] = i;
            _dict[i] = i;
        }

        // Generate randomized keys to guarantee a fair, non-biased lookup evaluation
        _searchKeys = Enumerable.Range(0, 1000).Select(_ => random.Next(0, N)).ToArray();

        // Generate a distinct random sequence to measure raw, non-sequential insertion costs
        _insertKeys = Enumerable.Range(0, 1000).Select(_ => random.Next(0, 100_000)).ToArray();
    }

    /// <summary>
    /// Measures the overhead of inserting unsorted keys into a self-balancing Red-Black binary tree.
    /// </summary>
    [Benchmark]
    public SortedDictionary<int, int> SortedDictionaryAdd() {
        var map = new SortedDictionary<int, int>();
        for (int i = 0; i < _insertKeys.Length; i++) {
            int val = _insertKeys[i];
            map[val] = val;
        }
        return map;
    }

    /// <summary>
    /// Measures the baseline cost of inserting the same unsorted keys into a standard hash bucket array layout.
    /// </summary>
    [Benchmark]
    public Dictionary<int, int> DictionaryAdd() {
        var map = new Dictionary<int, int>();
        for (int i = 0; i < _insertKeys.Length; i++) {
            int val = _insertKeys[i];
            map[val] = val;
        }
        return map;
    }

    /// <summary>
    /// Measures lookup performance across a self-balancing binary search tree structure.
    /// </summary>
    [Benchmark]
    public bool SortedDictionaryLookup() {
        int key = _searchKeys[_searchIndex++ % (uint)_searchKeys.Length];
        return _sorted.ContainsKey(key);
    }

    /// <summary>
    /// Measures baseline lookup performance from an O(1) hash table layout.
    /// </summary>
    [Benchmark]
    public bool DictionaryLookup() {
        int key = _searchKeys[_searchIndex++ % (uint)_searchKeys.Length];
        return _dict.ContainsKey(key);
    }
}
