using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PracticalDotNetCollections.Benchmarks.Add;

/// <summary>
/// Benchmarks evaluating <see cref="SortedList{TKey, TValue}"/> performance footprints against <see cref="SortedDictionary{TKey, TValue}"/>.
/// Measures array shifting bottlenecks under unsorted insertions alongside contiguous array binary search reading speeds.
/// </summary>
[MemoryDiagnoser]
public class SortedListBenchmark {
    private SortedList<int, int> _populatedList = null!;
    private SortedDictionary<int, int> _populatedDict = null!;
    private int[] _shuffledInsertKeys = null!;
    private int[] _searchKeys = null!;
    private uint _searchIndex;

    /// <summary>
    /// The number of entries processed inside the maps.
    /// </summary>
    [Params(1_000, 10_000)]
    public int N;

    /// <summary>
    /// Generates randomized search routes and shuffled unique keys to prevent sequential alignment bias.
    /// </summary>
    [GlobalSetup]
    public void Setup() {
        var random = new Random(42);

        // Generate unique shuffled keys to force realistic mid-array shifting penalties
        _shuffledInsertKeys = Enumerable.Range(0, N).OrderBy(_ => random.Next()).ToArray();

        _populatedList = new SortedList<int, int>();
        _populatedDict = new SortedDictionary<int, int>();

        for (int i = 0; i < N; i++) {
            _populatedList[i] = i;
            _populatedDict[i] = i;
        }

        // Generate randomized keys to guarantee a fair, non-biased lookup evaluation
        _searchKeys = Enumerable.Range(0, 1000).Select(_ => random.Next(0, N)).ToArray();
    }

    /// <summary>
    /// Measures the overhead of inserting unsorted keys into an array-backed structure forcing internal linear vector shifts.
    /// </summary>
    [Benchmark]
    public SortedList<int, int> SortedListRandomAdd() {
        var map = new SortedList<int, int>();
        for (int i = 0; i < _shuffledInsertKeys.Length; i++) {
            int val = _shuffledInsertKeys[i];
            map[val] = val;
        }
        return map;
    }

    /// <summary>
    /// Measures the alternative cost of inserting the exact same unsorted keys into a self-balancing pointer tree structure.
    /// </summary>
    [Benchmark]
    public SortedDictionary<int, int> SortedDictionaryRandomAdd() {
        var map = new SortedDictionary<int, int>();
        for (int i = 0; i < _shuffledInsertKeys.Length; i++) {
            int val = _shuffledInsertKeys[i];
            map[val] = val;
        }
        return map;
    }

    /// <summary>
    /// Measures lookup performance across flat contiguous key-value memory arrays using binary searching techniques.
    /// </summary>
    [Benchmark]
    public bool SortedListLookup() {
        int key = _searchKeys[_searchIndex++ % (uint)_searchKeys.Length];
        return _populatedList.ContainsKey(key);
    }

    /// <summary>
    /// Measures lookup performance navigating multi-layer reference nodes in a balanced tree graph structure.
    /// </summary>
    [Benchmark]
    public bool SortedDictionaryLookup() {
        int key = _searchKeys[_searchIndex++ % (uint)_searchKeys.Length];
        return _populatedDict.ContainsKey(key);
    }
}
