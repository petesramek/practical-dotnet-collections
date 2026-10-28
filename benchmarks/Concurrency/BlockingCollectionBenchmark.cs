using BenchmarkDotNet.Attributes;
using System.Collections.Concurrent;
using static PracticalDotNetCollections.Benchmarks.Concurrency.BenchmarkThreads;

namespace PracticalDotNetCollections.Benchmarks.Concurrency;

/// <summary>
/// Benchmarks comparing <see cref="BlockingCollection{T}"/> and <see cref="ConcurrentQueue{T}"/>.
/// All variants use dedicated threads and the same simulated per-item work, so the differences come from the
/// collection (blocking/bounding behavior) rather than from the thread pool or the work itself.
/// In the slow consumer scenarios elapsed time is dominated by the consumer; the useful signal there is allocation.
/// </summary>
[MemoryDiagnoser]
public class BlockingCollectionBenchmark {
    private const int ProducerCount = 4;
    private const int ConsumerCount = 2;
    private const int BoundedLimit = 500;

    /// <summary>
    /// The total number of items sent through the pipelines.
    /// </summary>
    [Params(10_000, 50_000)]
    public int N;

    /// <summary>
    /// Unbounded ConcurrentQueue with polling consumers (they yield when the queue is empty).
    /// </summary>
    [Benchmark]
    public void ThroughputConcurrentQueue() {
        var queue = new ConcurrentQueue<int>();
        int eventsPerProducer = N / ProducerCount;
        int remaining = eventsPerProducer * ProducerCount;

        var consumers = new Task[ConsumerCount];
        for (int i = 0; i < ConsumerCount; i++) {
            consumers[i] = Start(() => {
                while (Volatile.Read(ref remaining) > 0) {
                    if (queue.TryDequeue(out _)) {
                        Interlocked.Decrement(ref remaining);
                        Thread.SpinWait(FastWork);
                    } else {
                        Thread.Yield();
                    }
                }
            });
        }

        var producers = new Task[ProducerCount];
        for (int i = 0; i < ProducerCount; i++) {
            producers[i] = Start(() => {
                for (int j = 0; j < eventsPerProducer; j++) {
                    queue.Enqueue(j);
                }
            });
        }

        Task.WaitAll(producers);
        Task.WaitAll(consumers);
    }

    /// <summary>
    /// Unbounded BlockingCollection (wraps a ConcurrentQueue) with blocking consumers.
    /// </summary>
    [Benchmark]
    public void ThroughputBlockingCollectionUnbounded() => RunBlockingCollection(new BlockingCollection<int>());

    /// <summary>
    /// Bounded BlockingCollection: producers block when the collection is full.
    /// </summary>
    [Benchmark]
    public void ThroughputBlockingCollectionBounded() => RunBlockingCollection(new BlockingCollection<int>(BoundedLimit));

    private void RunBlockingCollection(BlockingCollection<int> pipeline) {
        using (pipeline) {
            int eventsPerProducer = N / ProducerCount;

            var consumers = new Task[ConsumerCount];
            for (int i = 0; i < ConsumerCount; i++) {
                consumers[i] = Start(() => {
                    foreach (var _ in pipeline.GetConsumingEnumerable()) {
                        Thread.SpinWait(FastWork);
                    }
                });
            }

            var producers = new Task[ProducerCount];
            for (int i = 0; i < ProducerCount; i++) {
                producers[i] = Start(() => {
                    for (int j = 0; j < eventsPerProducer; j++) {
                        pipeline.Add(j);
                    }
                });
            }

            Task.WaitAll(producers);
            pipeline.CompleteAdding();
            Task.WaitAll(consumers);
        }
    }

    /// <summary>
    /// One fast producer and one slow consumer with an unbounded ConcurrentQueue: the queue grows with the backlog.
    /// </summary>
    [Benchmark]
    public void SlowConsumerConcurrentQueue() {
        var queue = new ConcurrentQueue<int>();
        int remaining = N;

        var consumer = Start(() => {
            while (Volatile.Read(ref remaining) > 0) {
                if (queue.TryDequeue(out _)) {
                    Interlocked.Decrement(ref remaining);
                    Thread.SpinWait(SlowWork);
                } else {
                    Thread.Yield();
                }
            }
        });

        var producer = Start(() => {
            for (int i = 0; i < N; i++) {
                queue.Enqueue(i);
            }
        });

        Task.WaitAll(producer, consumer);
    }

    /// <summary>
    /// One fast producer and one slow consumer with a bounded BlockingCollection: the backlog never exceeds the bound.
    /// </summary>
    [Benchmark]
    public void SlowConsumerBlockingCollection() {
        using var pipeline = new BlockingCollection<int>(BoundedLimit);

        var consumer = Start(() => {
            foreach (var _ in pipeline.GetConsumingEnumerable()) {
                Thread.SpinWait(SlowWork);
            }
        });

        var producer = Start(() => {
            for (int i = 0; i < N; i++) {
                pipeline.Add(i);
            }
            pipeline.CompleteAdding();
        });

        Task.WaitAll(producer, consumer);
    }
}
