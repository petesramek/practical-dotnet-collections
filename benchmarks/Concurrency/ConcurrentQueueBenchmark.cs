using BenchmarkDotNet.Attributes;
using System.Collections.Concurrent;

namespace PracticalDotNetCollections.Benchmarks;

/// <summary>
/// Benchmarks evaluating <see cref="ConcurrentQueue{T}"/> performance against a locked standard <see cref="Queue{T}"/>.
/// Measures thread contention profiles across decoupled segment pointers versus full global mutual exclusion loops.
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
    /// Measures raw concurrent throughput using specialized lock-free segment chunks.
    /// </summary>
    [Benchmark]
    public void ConcurrentQueueThroughput() {
        var queue = new ConcurrentQueue<int>();
        int itemsPerProducer = N / ProducerCount;
        long totalConsumed = 0;

        var consumers = new Task[ConsumerCount];
        for (int i = 0; i < ConsumerCount; i++) {
            consumers[i] = Task.Run(() => {
                while (Interlocked.Read(ref totalConsumed) < N) {
                    if (queue.TryDequeue(out _)) {
                        Interlocked.Increment(ref totalConsumed);
                    }
                }
            });
        }

        var producers = new Task[ProducerCount];
        for (int i = 0; i < ProducerCount; i++) {
            producers[i] = Task.Run(() => {
                for (int j = 0; j < itemsPerProducer; j++) {
                    queue.Enqueue(j);
                }
            });
        }

        Task.WaitAll(producers);
        Task.WaitAll(consumers);
    }

    /// <summary>
    /// Measures concurrent performance using a standard queue protected by an exclusive monitor lock block.
    /// </summary>
    [Benchmark]
    public void QueueWithLockThroughput() {
        var queue = new Queue<int>();
        var locker = new object();
        int itemsPerProducer = N / ProducerCount;
        long totalConsumed = 0;

        var consumers = new Task[ConsumerCount];
        for (int i = 0; i < ConsumerCount; i++) {
            consumers[i] = Task.Run(() => {
                while (Interlocked.Read(ref totalConsumed) < N) {
                    lock (locker) {
                        if (queue.Count > 0) {
                            queue.Dequeue();
                            Interlocked.Increment(ref totalConsumed);
                        }
                    }
                }
            });
        }

        var producers = new Task[ProducerCount];
        for (int i = 0; i < ProducerCount; i++) {
            producers[i] = Task.Run(() => {
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
