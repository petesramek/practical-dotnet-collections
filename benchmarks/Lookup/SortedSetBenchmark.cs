using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PracticalDotNetCollections.Benchmarks.Lookup;

/// <summary>
/// Benchmarks evaluating <see cref="SortedSet{T}"/> performance footprints against a standard hash-based <see cref="HashSet{T}"/>.
/// Measures pointer-heavy Red-Black tree insertion mutations against flat hash bucket layouts alongside lookup scaling differences.
/// </summary>
[MemoryDiagnoser]
public class SortedSetBenchmark {
    private SortedSet<int> _sortedSet = null!;
    private HashSet<int> _hashSet = null!;
    private int[] _searchKeys = null!;
    private int[] _insertKeys = null!;
    private uint _searchIndex;

    /// <summary>
    /// The number of unique entries processed inside the set structures.
    /// </summary>
    [Params(1_000, 10_000)]
    public int N;

    /// <summary>
    /// Sets up and populates the long-lived tree and hash instances for reading evaluation tracks.
    /// </summary>
    [GlobalSetup]
    public void Setup() {
        var random = new Random(42);
        _sortedSet = new SortedSet<int>();
        _hashSet = new HashSet<int>();

        for (int i = 0; i < N; i++) {
            _sortedSet.Add(i);
            _hashSet.Add(i);
        }

        // Generate randomized keys to guarantee a fair, non-biased lookup evaluation
        _searchKeys = Enumerable.Range(0, 1000).Select(_ => random.Next(0, N)).ToArray();

        // Generate a distinct random sequence to measure raw, non-sequential insertion costs
        _insertKeys = Enumerable.Range(0, 1000).Select(_ => random.Next(0, 100_000)).ToArray();
    }

    /// <summary>
    /// Measures the overhead of inserting unique, unsorted items into a self-balancing Red-Black binary tree.
    /// </summary>
    [Benchmark]
    public SortedSet<int> SortedSetAdd() {
        var set = new SortedSet<int>();
        for (int i = 0; i < _insertKeys.Length; i++) {
            set.Add(_insertKeys[i]);
        }
        return set;
    }

    /// <summary>
    /// Measures the baseline cost of inserting the same unique, unsorted items into a standard hash bucket array layout.
    /// </summary>
    [Benchmark]
    public HashSet<int> HashSetAdd() {
        var set = new HashSet<int>();
        for (int i = 0; i < _insertKeys.Length; i++) {
            set.Add(_insertKeys[i]);
        }
        return set;
    }

    /// <summary>
    /// Measures search containment performance across a self-balancing binary search tree structure.
    /// </summary>
    [Benchmark]
    public bool SortedSetContains() {
        int key = _searchKeys[_searchIndex++ % (uint)_searchKeys.Length];
        return _sortedSet.Contains(key);
    }

    /// <summary>
    /// Measures baseline search containment performance from an O(1) hash bucket array layout.
    /// </summary>
    [Benchmark]
    public bool HashSetContains() {
        int key = _searchKeys[_searchIndex++ % (uint)_searchKeys.Length];
        return _hashSet.Contains(key);
    }
}
