using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;

namespace PracticalDotNetCollections.Benchmarks.Lookup;

/// <summary>
/// Benchmarks evaluating <see cref="FrozenDictionary{TKey, TValue}"/> performance footprints against a standard <see cref="Dictionary{TKey, TValue}"/>.
/// Measures upfront construction overhead versus read-only optimization speeds across scaling lookups.
/// </summary>
[MemoryDiagnoser]
public class FrozenDictionaryBenchmark {
    private FrozenDictionary<int, int> _frozen = null!;
    private Dictionary<int, int> _dict = null!;
    private int[] _searchKeys = null!;
    private Dictionary<int, int> _sourceData = null!;
    private uint _searchIndex;

    /// <summary>
    /// The number of entries processed inside the maps.
    /// </summary>
    [Params(10_000, 100_000)]
    public int N;

    /// <summary>
    /// Generates the base datasets and randomized search tracks.
    /// </summary>
    [GlobalSetup]
    public void Setup() {
        var random = new Random(42);

        _sourceData = Enumerable.Range(0, N).ToDictionary(x => x, x => x);
        _frozen = _sourceData.ToFrozenDictionary();
        _dict = _sourceData;

        // Generate randomized keys to guarantee a fair, non-biased lookup evaluation
        _searchKeys = Enumerable.Range(0, 1000).Select(_ => random.Next(0, N)).ToArray();
    }

    /// <summary>
    /// Measures the compilation cost and memory allocations required to build a specialized FrozenDictionary.
    /// </summary>
    [Benchmark]
    public FrozenDictionary<int, int> CreateFrozenDictionary() {
        return _sourceData.ToFrozenDictionary();
    }

    /// <summary>
    /// Measures the read lookup speed of a FrozenDictionary using specialized internal hash mapping structures.
    /// </summary>
    [Benchmark]
    public bool FrozenDictionaryLookup() {
        int key = _searchKeys[_searchIndex++ % (uint)_searchKeys.Length];
        return _frozen.ContainsKey(key);
    }

    /// <summary>
    /// Measures the read lookup speed of a standard Dictionary baseline.
    /// </summary>
    [Benchmark]
    public bool DictionaryLookup() {
        int key = _searchKeys[_searchIndex++ % (uint)_searchKeys.Length];
        return _dict.ContainsKey(key);
    }
}
