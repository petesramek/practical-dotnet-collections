using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PracticalDotNetCollections.Benchmarks.Lookup;

/// <summary>
/// Benchmarks evaluating <see cref="HashSet{T}"/> performance footprints against a standard <see cref="List{T}"/>.
/// Measures lookup scaling limits alongside hardware set mathematical intersection performance.
/// </summary>
[MemoryDiagnoser]
public class HashSetBenchmark {
    private HashSet<int> _set = null!;
    private List<int> _list = null!;
    private int[] _searchKeys = null!;
    private uint _searchIndex;

    private List<int> _sourceItems = null!;
    private HashSet<int> _filterSet = null!;
    private List<int> _filterList = null!;

    /// <summary>
    /// The number of entries processed inside the collections.
    /// </summary>
    [Params(10_000, 100_000)]
    public int N;

    /// <summary>
    /// Sets up and populates hash pools, lists, and randomized search indices.
    /// </summary>
    [GlobalSetup]
    public void Setup() {
        var random = new Random(42);

        _set = Enumerable.Range(0, N).ToHashSet();
        _list = Enumerable.Range(0, N).ToList();

        // Generate randomized keys to guarantee a fair, non-biased lookup evaluation
        _searchKeys = Enumerable.Range(0, 1000).Select(_ => random.Next(0, N)).ToArray();

        // Setup scenario data for intersection/deduplication checks
        _sourceItems = Enumerable.Range(0, N).Select(_ => random.Next(0, N * 2)).ToList();

        // Take a small fraction to intersect against
        var filterSource = Enumerable.Range(0, N / 10).Select(_ => random.Next(0, N * 2)).ToList();
        _filterSet = filterSource.ToHashSet();
        _filterList = filterSource;
    }

    /// <summary>
    /// Measures flat hash-pool containment scaling performance via bucket mapping logic.
    /// </summary>
    [Benchmark]
    public bool HashSetLookup() {
        int key = _searchKeys[_searchIndex++ % (uint)_searchKeys.Length];
        return _set.Contains(key);
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
    /// Demonstrates the common mistake: intersecting collections using a nested linear List check, resulting in O(N^2) complexity.
    /// </summary>
    [Benchmark]
    public List<int> ScenarioIntersectionListLoop() {
        var result = new List<int>();
        for (int i = 0; i < _sourceItems.Count; i++) {
            int item = _sourceItems[i];
            if (_filterList.Contains(item)) {
                result.Add(item);
            }
        }
        return result;
    }

    /// <summary>
    /// Demonstrates the optimization fix: leveraging mathematical hash bucket intersections to isolate duplicates near-instantaneously.
    /// </summary>
    [Benchmark]
    public HashSet<int> ScenarioIntersectionHashSetBulk() {
        var result = new HashSet<int>(_sourceItems);
        result.IntersectWith(_filterSet);
        return result;
    }
}
