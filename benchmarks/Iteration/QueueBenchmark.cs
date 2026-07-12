using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

namespace PracticalDotNetCollections.Benchmarks.Iteration;

/// <summary>
/// Benchmarks evaluating standard <see cref="Queue{T}"/> performance characteristics.
/// Measures dynamic circular array expansions, up-front capacity optimizations, and list-shifting dequeue penalties.
/// </summary>
[MemoryDiagnoser]
public class QueueBenchmark {
    /// <summary>
    /// The number of entries processed inside the sequences.
    /// </summary>
    [Params(1_000, 10_000)]
    public int N;

    /// <summary>
    /// Measures baseline sequential enqueues and dequeues forcing dynamic internal array resizing.
    /// </summary>
    [Benchmark]
    public void QueueThroughput() {
        var queue = new Queue<int>();
        for (int i = 0; i < N; i++) {
            queue.Enqueue(i);
        }

        while (queue.Count > 0) {
            queue.Dequeue();
        }
    }

    /// <summary>
    /// Demonstrates the optimization fix: specifying the expected size upfront to eliminate dynamic resizing copies.
    /// </summary>
    [Benchmark]
    public void PreSizedQueueThroughput() {
        var queue = new Queue<int>(N);
        for (int i = 0; i < N; i++) {
            queue.Enqueue(i);
        }

        while (queue.Count > 0) {
            queue.Dequeue();
        }
    }

    /// <summary>
    /// Demonstrates the catastrophic queue anti-pattern: removing items from the front of a list, forcing linear O(N) array shifts.
    /// </summary>
    [Benchmark]
    public void ListRemoveAtBeginning() {
        var list = new List<int>();
        for (int i = 0; i < N; i++) {
            list.Add(i);
        }

        while (list.Count > 0) {
            list.RemoveAt(0);
        }
    }
}
