using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace PracticalDotNetCollections.Benchmarks.Memory;

/// <summary>
/// Benchmarks evaluating <see cref="ImmutableList{T}"/> performance footprints against a standard <see cref="List{T}"/>.
/// Measures item mutation loops, optimization paths using builders, and hot-path search lookup indexing speeds.
/// </summary>
[MemoryDiagnoser]
public class ImmutableListBenchmark {
    private ImmutableList<int> _immutableList = ImmutableList<int>.Empty;
    private List<int> _list = null!;
    private int[] _searchIndices = null!;
    private uint _searchIndex;

    /// <summary>
    /// The number of entries processed inside the lists.
    /// </summary>
    [Params(1_000, 10_000)]
    public int N;

    /// <summary>
    /// Sets up pre-populated dataset structures and randomized indexing access tracks.
    /// </summary>
    [GlobalSetup]
    public void Setup() {
        var random = new Random(42);
        var data = Enumerable.Range(0, 100_000).ToList();

        _immutableList = data.ToImmutableList();
        _list = data;

        // Generate randomized indices to guarantee fair, non-biased lookup evaluations
        _searchIndices = Enumerable.Range(0, 1000).Select(_ => random.Next(0, 100_000)).ToArray();
    }

    /// <summary>
    /// Demonstrates the severe loop anti-pattern: allocating a new internal tree path on every entry addition.
    /// </summary>
    [Benchmark]
    public ImmutableList<int> ImmutableListAdd() {
        var list = ImmutableList<int>.Empty;
        for (int i = 0; i < N; i++) {
            list = list.Add(i);
        }
        return list;
    }

    /// <summary>
    /// Demonstrates the optimization fix: using an internal builder block to construct an immutable tree structure with linear performance.
    /// </summary>
    [Benchmark]
    public ImmutableList<int> ImmutableListWithBuilderAdd() {
        var builder = ImmutableList.CreateBuilder<int>();
        for (int i = 0; i < N; i++) {
            builder.Add(i);
        }
        return builder.ToImmutable();
    }

    /// <summary>
    /// Measures baseline sequential entry additions into a standard mutable List.
    /// </summary>
    [Benchmark]
    public List<int> ListAdd() {
        var list = new List<int>();
        for (int i = 0; i < N; i++) {
            list.Add(i);
        }
        return list;
    }

    /// <summary>
    /// Measures index lookup performance across an immutable list structure, exposing tree traversal overhead.
    /// </summary>
    [Benchmark]
    public int ImmutableListRead() {
        int index = _searchIndices[_searchIndex++ % (uint)_searchIndices.Length];
        return _immutableList[index];
    }

    /// <summary>
    /// Measures index lookup performance from a standard array-backed list baseline.
    /// </summary>
    [Benchmark]
    public int ListRead() {
        int index = _searchIndices[_searchIndex++ % (uint)_searchIndices.Length];
        return _list[index];
    }
}
