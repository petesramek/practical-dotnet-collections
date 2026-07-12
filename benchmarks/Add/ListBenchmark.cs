using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

namespace PracticalDotNetCollections.Benchmarks.Add;

/// <summary>
/// Benchmarks evaluating standard <see cref="List{T}"/> performance characteristics.
/// Measures dynamic array tail expansion, up-front capacity optimizations, and front memory shift overhead.
/// </summary>
[MemoryDiagnoser]
public class ListBenchmark {
    /// <summary>
    /// The number of elements processed inside the list sequences.
    /// </summary>
    [Params(1_000, 10_000)]
    public int N;

    /// <summary>
    /// Measures baseline sequential entries added to the tail of a list, forcing dynamic array resizes.
    /// </summary>
    [Benchmark]
    public void Add() {
        var list = new List<int>();
        for (int i = 0; i < N; i++) {
            list.Add(i);
        }
    }

    /// <summary>
    /// Demonstrates the optimization fix: specifying the expected size upfront to eliminate dynamic resizing copies.
    /// </summary>
    [Benchmark]
    public void PreSizedCapacityAdd() {
        var list = new List<int>(N);
        for (int i = 0; i < N; i++) {
            list.Add(i);
        }
    }

    /// <summary>
    /// Demonstrates the severe array shift anti-pattern: shifting every element in memory forward on every entry insertion.
    /// </summary>
    [Benchmark]
    public void InsertAtBeginning() {
        var list = new List<int>();
        for (int i = 0; i < N; i++) {
            list.Insert(0, i);
        }
    }
}
