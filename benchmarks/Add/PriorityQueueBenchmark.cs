using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;

namespace PracticalDotNetCollections.Benchmarks.Add;

/// <summary>
/// Benchmarks evaluating <see cref="PriorityQueue{TElement, TPriority}"/> performance characteristics.
/// Measures dynamic heap tree expansions, pre-sized allocations, and continuous sorting stream bottlenecks against lists.
/// </summary>
[MemoryDiagnoser]
public class PriorityQueueBenchmark {
    /// <summary>
    /// The number of entries processed inside the tracking queues.
    /// </summary>
    [Params(1_000, 10_000)]
    public int N;

    /// <summary>
    /// Measures baseline sequential insertions and extractions forcing dynamic internal array resizing.
    /// </summary>
    [Benchmark]
    public void PriorityQueueThroughput() {
        var pq = new PriorityQueue<int, int>();
        for (int i = 0; i < N; i++) {
            pq.Enqueue(i, i);
        }

        while (pq.Count > 0) {
            pq.Dequeue();
        }
    }

    /// <summary>
    /// Demonstrates the optimization fix: specifying the expected capacity upfront to eliminate internal array expansions.
    /// </summary>
    [Benchmark]
    public void PreSizedPriorityQueueThroughput() {
        var pq = new PriorityQueue<int, int>(N);
        for (int i = 0; i < N; i++) {
            pq.Enqueue(i, i);
        }

        while (pq.Count > 0) {
            pq.Dequeue();
        }
    }

    /// <summary>
    /// Demonstrates the catastrophic sorting anti-pattern: using a list to maintain an actively sorted streaming state.
    /// </summary>
    [Benchmark]
    public void ListStreamingSort() {
        var list = new List<int>();
        for (int i = 0; i < N; i++) {
            // Simulates searching and inserting an element into its exact sorted placement position
            int index = list.BinarySearch(i);
            if (index < 0)
                index = ~index;
            list.Insert(index, i);
        }

        while (list.Count > 0) {
            // Pops from the front to simulate reading the highest-priority element
            list.RemoveAt(0);
        }
    }
}
