using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace PracticalDotNetCollections.Benchmarks.Memory;

/// <summary>
/// Benchmarks evaluating <see cref="ImmutableArray{T}"/> performance footprints against a standard <see cref="List{T}"/>.
/// Measures item mutation loops, optimization paths using builders, and hot-path memory read indexing speeds.
/// </summary>
[MemoryDiagnoser]
public class ImmutableArrayBenchmark {
    // Initialized using safe, struct-valid default states instead of reference nulls
    private ImmutableArray<int> _immutableArray = ImmutableArray<int>.Empty;
    private List<int> _list = null!;
    private uint _readIndex;

    /// <summary>
    /// The number of entries processed inside the collections.
    /// </summary>
    [Params(1_000, 10_000)]
    public int N;

    /// <summary>
    /// Sets up the populated data structures for read-only tracking loops.
    /// </summary>
    [GlobalSetup]
    public void Setup() {
        var data = Enumerable.Range(0, 100_000).ToArray();
        _immutableArray = data.ToImmutableArray();
        _list = data.ToList();
    }

    /// <summary>
    /// Demonstrates the catastrophic loop anti-pattern: allocating a brand new array chunk on every single entry modification.
    /// </summary>
    [Benchmark]
    public ImmutableArray<int> ImmutableArrayAdd() {
        var array = ImmutableArray<int>.Empty;
        for (int i = 0; i < N; i++) {
            array = array.Add(i);
        }
        return array;
    }

    /// <summary>
    /// Demonstrates the optimization fix: using an internal builder block to construct an immutable structure with linear performance.
    /// </summary>
    [Benchmark]
    public ImmutableArray<int> ImmutableArrayWithBuilderAdd() {
        var builder = ImmutableArray.CreateBuilder<int>();
        for (int i = 0; i < N; i++) {
            builder.Add(i);
        }
        return builder.ToImmutable();
    }

    /// <summary>
    /// Measures baseline sequential entry mutations into a standard mutable List.
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
    /// Measures lookup performance from an immutable array via direct, raw memory pointer index mappings.
    /// </summary>
    [Benchmark]
    public int ImmutableArrayRead() {
        int targetIndex = (int)(_readIndex++ % 100_000);
        return _immutableArray[targetIndex];
    }

    /// <summary>
    /// Measures lookup performance from a standard list across the exact same reading boundaries.
    /// </summary>
    [Benchmark]
    public int ListRead() {
        int targetIndex = (int)(_readIndex++ % 100_000);
        return _list[targetIndex];
    }
}
