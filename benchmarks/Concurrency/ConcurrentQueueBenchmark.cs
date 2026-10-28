using BenchmarkDotNet.Attributes;
using System.Collections.Concurrent;
using static PracticalDotNetCollections.Benchmarks.Concurrency.BenchmarkThreads;

namespace PracticalDotNetCollections.Benchmarks.Concurrency;

/// <summary>
/// Benchmarks comparing <see cref="ConcurrentQueue{T}"/> against a standard <see cref="Queue{T}"/> protected by a lock.
/// Both variants use dedicated threads, the same producer/consumer counts, and yield when the queue is empty.
/// Results depend heavily on core count and contention; treat them as one data point, not a general ranking.
/// </summary>
[MemoryDiagnoser]
public class ConcurrentQueueBenchmark {
    private const int ProducerCount = 4;
    private const int ConsumerCount = 4;

    /// <summary>
    /// The total number of elements queued and drained during the execution pass.
    /// </summary>
    [Params(10_000, 50_000)]
    public int N;

    /// <summary>
    /// Lock-free ConcurrentQueue with polling consumers.
    /// </summary>
    [Benchmark]
    public void ConcurrentQueueThroughput() {
        var queue = new ConcurrentQueue<int>();
        int itemsPerProducer = N / ProducerCount;
        int remaining = itemsPerProducer * ProducerCount;

        var consumers = new Task[ConsumerCount];
        for (int i = 0; i < ConsumerCount; i++) {
            consumers[i] = Start(() => {
                while (Volatile.Read(ref remaining) > 0) {
                    if (queue.TryDequeue(out _)) {
                        Interlocked.Decrement(ref remaining);
                    } else {
                        Thread.Yield();
                    }
                }
            });
        }

        var producers = new Task[ProducerCount];
        for (int i = 0; i < ProducerCount; i++) {
            producers[i] = Start(() => {
                for (int j = 0; j < itemsPerProducer; j++) {
                    queue.Enqueue(j);
                }
            });
        }

        Task.WaitAll(producers);
        Task.WaitAll(consumers);
    }

    /// <summary>
    /// Standard Queue guarded by a monitor lock with polling consumers.
    /// </summary>
    [Benchmark]
    public void QueueWithLockThroughput() {
        var queue = new Queue<int>();
        var locker = new object();
        int itemsPerProducer = N / ProducerCount;
        int remaining = itemsPerProducer * ProducerCount;

        var consumers = new Task[ConsumerCount];
        for (int i = 0; i < ConsumerCount; i++) {
            consumers[i] = Start(() => {
                while (Volatile.Read(ref remaining) > 0) {
                    bool dequeued;
                    lock (locker) {
                        dequeued = queue.TryDequeue(out _);
                    }

                    if (dequeued) {
                        Interlocked.Decrement(ref remaining);
                    } else {
                        Thread.Yield();
                    }
                }
            });
        }

        var producers = new Task[ProducerCount];
        for (int i = 0; i < ProducerCount; i++) {
            producers[i] = Start(() => {
                for (int j = 0; j < itemsPerProducer; j++) {
                    lock (locker) {
                        queue.Enqueue(j);
                    }
                }
            });
        }

        Task.WaitAll(producers);
        Task.WaitAll(consumers);
    }
}
