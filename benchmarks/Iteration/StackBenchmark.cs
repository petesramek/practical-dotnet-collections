using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

namespace PracticalDotNetCollections.Benchmarks.Iteration;

/// <summary>
/// Benchmarks evaluating standard <see cref="Stack{T}"/> performance characteristics.
/// Measures dynamic array tail expansion, up-front capacity optimizations, and list tail deletion alternatives.
/// </summary>
[MemoryDiagnoser]
public class StackBenchmark {
    /// <summary>
    /// The number of entries processed inside the tracking sequences.
    /// </summary>
    [Params(1_000, 10_000)]
    public int N;

    /// <summary>
    /// Measures baseline sequential pushes and pops forcing dynamic internal array resizing loops.
    /// </summary>
    [Benchmark]
    public void StackThroughput() {
        var stack = new Stack<int>();
        for (int i = 0; i < N; i++) {
            stack.Push(i);
        }

        while (stack.Count > 0) {
            stack.Pop();
        }
    }

    /// <summary>
    /// Demonstrates the optimization fix: specifying the expected capacity upfront to eliminate internal array expansions.
    /// </summary>
    [Benchmark]
    public void PreSizedStackThroughput() {
        var stack = new Stack<int>(N);
        for (int i = 0; i < N; i++) {
            stack.Push(i);
        }

        while (stack.Count > 0) {
            stack.Pop();
        }
    }

    /// <summary>
    /// Measures baseline sequential entry mutations into a standard mutable List handling additions and removals strictly from its tail.
    /// </summary>
    [Benchmark]
    public void ListAddRemoveFromEnd() {
        var list = new List<int>();
        for (int i = 0; i < N; i++) {
            list.Add(i);
        }

        while (list.Count > 0) {
            list.RemoveAt(list.Count - 1);
        }
    }
}
