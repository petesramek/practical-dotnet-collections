using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PracticalDotNetCollections.Benchmarks.Add;

/// <summary>
/// Benchmarks evaluating <see cref="LinkedList{T}"/> performance profiles against a standard <see cref="List{T}"/>.
/// Measures front-insertion memory shift scaling boundaries alongside sequential pointer traversal lookup speeds.
/// </summary>
[MemoryDiagnoser]
public class LinkedListBenchmark {
    private LinkedList<int> _linkedList = null!;
    private List<int> _list = null!;
    private int[] _searchIndices = null!;
    private uint _searchIndex;

    /// <summary>
    /// The number of elements processed inside the sequences.
    /// </summary>
    [Params(1_000, 10_000)]
    public int N;

    /// <summary>
    /// Sets up the pre-populated collections and randomized index paths for reading tracks.
    /// </summary>
    [GlobalSetup]
    public void Setup() {
        var random = new Random(42);
        var data = Enumerable.Range(0, N).ToList();

        _linkedList = new LinkedList<int>(data);
        _list = data;

        // Generate randomized indices to guarantee fair, non-biased lookup evaluations
        _searchIndices = Enumerable.Range(0, 1000).Select(_ => random.Next(0, N)).ToArray();
    }

    /// <summary>
    /// Demonstrates the severe array shift anti-pattern: shifting every element in memory forward on every entry insertion.
    /// </summary>
    [Benchmark]
    public void ListInsertAtBeginning() {
        var list = new List<int>();
        for (int i = 0; i < N; i++) {
            list.Insert(0, i);
        }
    }

    /// <summary>
    /// Demonstrates the architectural benefit: linking a new node onto the head pointer instantly without altering existing memory.
    /// </summary>
    [Benchmark]
    public void LinkedListAddFirst() {
        var list = new LinkedList<int>();
        for (int i = 0; i < N; i++) {
            list.AddFirst(i);
        }
    }

    /// <summary>
    /// Highlights the indexing read penalty: jumping sequentially through nodes to resolve a target index.
    /// </summary>
    [Benchmark]
    public int LinkedListReadIndex() {
        int targetIndex = _searchIndices[_searchIndex++ % (uint)_searchIndices.Length];

        // ElementAt forces an O(N) node-by-node pointer traversal search
        return _linkedList.ElementAt(targetIndex);
    }

    /// <summary>
    /// Highlights the indexing read advantage: calculating direct memory offset mappings instantly.
    /// </summary>
    [Benchmark]
    public int ListReadIndex() {
        int targetIndex = _searchIndices[_searchIndex++ % (uint)_searchIndices.Length];
        return _list[targetIndex];
    }
}
