using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PracticalDotNetCollections.Benchmarks.Lookup;

/// <summary>
/// Benchmarks evaluating performance trade-offs between a direct <see cref="Dictionary{TKey, TValue}"/> and a wrapped <see cref="ReadOnlyDictionary{TKey, TValue}"/>.
/// Measures proxy lookup speeds alongside iteration loop cost variations caused by interface-bound type enumerations.
/// </summary>
[MemoryDiagnoser]
public class ReadOnlyDictionaryBenchmark {
    private Dictionary<int, int> _dict = null!;
    private ReadOnlyDictionary<int, int> _readOnly = null!;
    private int[] _searchKeys = null!;
    private uint _searchIndex;

    /// <summary>
    /// The number of entries processed inside the map collections.
    /// </summary>
    [Params(1_000, 10_000)]
    public int N;

    /// <summary>
    /// Sets up and populates the base tracking data models and randomized search pathways.
    /// </summary>
    [GlobalSetup]
    public void Setup() {
        var random = new Random(42);

        _dict = Enumerable.Range(0, N).ToDictionary(x => x, x => x);
        _readOnly = new ReadOnlyDictionary<int, int>(_dict);

        // Generate randomized keys to guarantee a fair, non-biased lookup evaluation
        _searchKeys = Enumerable.Range(0, 1000).Select(_ => random.Next(0, N)).ToArray();
    }

    /// <summary>
    /// Measures read lookup speed of a standard Dictionary baseline using direct flat hash mapping.
    /// </summary>
    [Benchmark]
    public bool DictionaryLookup() {
        int key = _searchKeys[_searchIndex++ % (uint)_searchKeys.Length];
        return _dict.ContainsKey(key);
    }

    /// <summary>
    /// Measures read lookup speed of a ReadOnlyDictionary passing requests down its internal delegate proxy chain.
    /// </summary>
    [Benchmark]
    public bool ReadOnlyDictionaryLookup() {
        int key = _searchKeys[_searchIndex++ % (uint)_searchKeys.Length];
        return _readOnly.ContainsKey(key);
    }

    /// <summary>
    /// Measures iteration speed across a direct Dictionary using its native value-type struct enumerator.
    /// </summary>
    [Benchmark]
    public int IterateDictionaryForeach() {
        int sum = 0;
        foreach (var pair in _dict) {
            sum += pair.Value;
        }
        return sum;
    }

    /// <summary>
    /// Measures iteration speed across a wrapped ReadOnlyDictionary, exposing virtual interface dispatch bottlenecks.
    /// </summary>
    [Benchmark]
    public int IterateReadOnlyDictionaryForeach() {
        int sum = 0;
        foreach (var pair in _readOnly) {
            sum += pair.Value;
        }
        return sum;
    }
}
