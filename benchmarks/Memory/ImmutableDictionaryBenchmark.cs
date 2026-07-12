using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace PracticalDotNetCollections.Benchmarks.Memory;

/// <summary>
/// Benchmarks evaluating <see cref="ImmutableDictionary{TKey, TValue}"/> performance footprints against a standard <see cref="Dictionary{TKey, TValue}"/>.
/// Measures item mutation loops, optimization paths using builders, and hot-path search lookup speeds.
/// </summary>
[MemoryDiagnoser]
public class ImmutableDictionaryBenchmark {
    private ImmutableDictionary<int, int> _immutableDict = ImmutableDictionary<int, int>.Empty;
    private Dictionary<int, int> _dictionary = null!;
    private int[] _searchKeys = null!;
    private uint _searchIndex;

    /// <summary>
    /// The number of entries processed inside the maps.
    /// </summary>
    [Params(1_000, 10_000)]
    public int N;

    /// <summary>
    /// Sets up the pre-populated map data structures for read-only tracking loops.
    /// </summary>
    [GlobalSetup]
    public void Setup() {
        var random = new Random(42);
        var data = Enumerable.Range(0, 100_000).ToDictionary(x => x, x => x);

        _immutableDict = data.ToImmutableDictionary();
        _dictionary = data;

        // Generate randomized keys to guarantee a fair, non-biased lookup evaluation
        _searchKeys = Enumerable.Range(0, 1000).Select(_ => random.Next(0, 100_000)).ToArray();
    }

    /// <summary>
    /// Demonstrates the severe loop anti-pattern: allocating a new internal tree path on every entry addition.
    /// </summary>
    [Benchmark]
    public ImmutableDictionary<int, int> ImmutableDictionaryAdd() {
        var dict = ImmutableDictionary<int, int>.Empty;
        for (int i = 0; i < N; i++) {
            dict = dict.Add(i, i);
        }
        return dict;
    }

    /// <summary>
    /// Demonstrates the optimization fix: using an internal builder block to construct an immutable tree structure with linear performance.
    /// </summary>
    [Benchmark]
    public ImmutableDictionary<int, int> ImmutableDictionaryWithBuilderAdd() {
        var builder = ImmutableDictionary.CreateBuilder<int, int>();
        for (int i = 0; i < N; i++) {
            builder.Add(i, i);
        }
        return builder.ToImmutable();
    }

    /// <summary>
    /// Measures baseline sequential entry additions into a standard mutable Dictionary.
    /// </summary>
    [Benchmark]
    public Dictionary<int, int> DictionaryAdd() {
        var dict = new Dictionary<int, int>();
        for (int i = 0; i < N; i++) {
            dict[i] = i;
        }
        return dict;
    }

    /// <summary>
    /// Measures lookup performance from an immutable dictionary structure.
    /// </summary>
    [Benchmark]
    public int ImmutableDictionaryLookup() {
        int key = _searchKeys[_searchIndex++ % (uint)_searchKeys.Length];
        return _immutableDict[key];
    }

    /// <summary>
    /// Measures lookup performance from a standard dictionary baseline.
    /// </summary>
    [Benchmark]
    public int DictionaryLookup() {
        int key = _searchKeys[_searchIndex++ % (uint)_searchKeys.Length];
        return _dictionary[key];
    }
}
