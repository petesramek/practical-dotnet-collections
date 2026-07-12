using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;

namespace PracticalDotNetCollections.Benchmarks.Lookup;

/// <summary>
/// Benchmarks evaluating <see cref="FrozenSet{T}"/> performance footprints against a standard <see cref="HashSet{T}"/>.
/// Measures upfront compilation overhead versus inclusion checking speeds across scaling lookups.
/// </summary>
[MemoryDiagnoser]
public class FrozenSetBenchmark {
    private FrozenSet<int> _frozen = null!;
    private HashSet<int> _hash = null!;
    private int[] _searchKeys = null!;
    private List<int> _sourceData = null!;
    private uint _searchIndex;

    /// <summary>
    /// The number of entries processed inside the sets.
    /// </summary>
    [Params(10_000, 100_000)]
    public int N;

    /// <summary>
    /// Generates the base datasets and randomized search tracks.
    /// </summary>
    [GlobalSetup]
    public void Setup() {
        var random = new Random(42);

        _sourceData = Enumerable.Range(0, N).ToList();
        _frozen = _sourceData.ToFrozenSet();
        _hash = _sourceData.ToHashSet();

        // Generate randomized keys to guarantee a fair, non-biased lookup evaluation
        _searchKeys = Enumerable.Range(0, 1000).Select(_ => random.Next(0, N)).ToArray();
    }

    /// <summary>
    /// Measures the compilation cost and memory allocations required to build a specialized FrozenSet.
    /// </summary>
    [Benchmark]
    public FrozenSet<int> CreateFrozenSet() {
        return _sourceData.ToFrozenSet();
    }

    /// <summary>
    /// Measures the search containment check speed of a FrozenSet using optimized internal layouts.
    /// </summary>
    [Benchmark]
    public bool FrozenSetContains() {
        int key = _searchKeys[_searchIndex++ % (uint)_searchKeys.Length];
        return _frozen.Contains(key);
    }

    /// <summary>
    /// Measures the search containment check speed of a standard HashSet baseline.
    /// </summary>
    [Benchmark]
    public bool HashSetContains() {
        int key = _searchKeys[_searchIndex++ % (uint)_searchKeys.Length];
        return _hash.Contains(key);
    }
}
