using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace PracticalDotNetCollections.Benchmarks.Memory;

/// <summary>
/// Benchmarks evaluating <see cref="ImmutableHashSet{T}"/> performance footprints against a standard <see cref="HashSet{T}"/>.
/// Measures item mutation loops, optimization paths using builders, and hot-path containment search speeds.
/// </summary>
[MemoryDiagnoser]
public class ImmutableHashSetBenchmark {
    private ImmutableHashSet<int> _immutableSet = ImmutableHashSet<int>.Empty;
    private HashSet<int> _hashSet = null!;
    private int[] _searchKeys = null!;
    private uint _searchIndex;

    /// <summary>
    /// The number of entries processed inside the sets.
    /// </summary>
    [Params(1_000, 10_000)]
    public int N;

    /// <summary>
    /// Sets up the pre-populated set data structures for read-only tracking loops.
    /// </summary>
    [GlobalSetup]
    public void Setup() {
        var random = new Random(42);
        var data = Enumerable.Range(0, 100_000).ToList();

        _immutableSet = data.ToImmutableHashSet();
        _hashSet = data.ToHashSet();

        // Generate randomized keys to guarantee a fair, non-biased lookup evaluation
        _searchKeys = Enumerable.Range(0, 1000).Select(_ => random.Next(0, 100_000)).ToArray();
    }

    /// <summary>
    /// Demonstrates the severe loop anti-pattern: allocating a new internal tree path on every entry addition.
    /// </summary>
    [Benchmark]
    public ImmutableHashSet<int> ImmutableHashSetAdd() {
        var set = ImmutableHashSet<int>.Empty;
        for (int i = 0; i < N; i++) {
            set = set.Add(i);
        }
        return set;
    }

    /// <summary>
    /// Demonstrates the optimization fix: using an internal builder block to construct an immutable structure with linear performance.
    /// </summary>
    [Benchmark]
    public ImmutableHashSet<int> ImmutableHashSetWithBuilderAdd() {
        var builder = ImmutableHashSet.CreateBuilder<int>();
        for (int i = 0; i < N; i++) {
            builder.Add(i);
        }
        return builder.ToImmutable();
    }

    /// <summary>
    /// Measures baseline sequential entry additions into a standard mutable HashSet.
    /// </summary>
    [Benchmark]
    public HashSet<int> HashSetAdd() {
        var set = new HashSet<int>();
        for (int i = 0; i < N; i++) {
            set.Add(i);
        }
        return set;
    }

    /// <summary>
    /// Measures containment lookup performance from an immutable hash set structure.
    /// </summary>
    [Benchmark]
    public bool ImmutableHashSetContains() {
        int key = _searchKeys[_searchIndex++ % (uint)_searchKeys.Length];
        return _immutableSet.Contains(key);
    }

    /// <summary>
    /// Measures containment lookup performance from a standard hash set baseline.
    /// </summary>
    [Benchmark]
    public bool HashSetContains() {
        int key = _searchKeys[_searchIndex++ % (uint)_searchKeys.Length];
        return _hashSet.Contains(key);
    }
}
