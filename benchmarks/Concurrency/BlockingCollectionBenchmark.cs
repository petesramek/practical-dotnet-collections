using BenchmarkDotNet.Attributes;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace PracticalDotNetCollections.Benchmarks;

/// <summary>
/// Benchmarks comparing <see cref="BlockingCollection{T}"/> and <see cref="ConcurrentQueue{T}"/>.
/// Evaluates raw concurrent throughput alongside behavior under heavy downstream consumption bottlenecks.
/// </summary>
[MemoryDiagnoser]
public class BlockingCollectionBenchmark {
    private const int ProducerCount = 4;
    private const int ConsumerCount = 2;
    private const int BoundedLimit = 500;

    /// <summary>
    /// The total number of items sent through the messaging pipelines.
    /// </summary>
    [Params(10_000, 50_000)]
    public int N;

    /// <summary>
    /// Measures raw concurrent throughput of a standard ConcurrentQueue when threads flow freely.
    /// </summary>
    [Benchmark]
    public void ThroughputConcurrentQueue() {
        var queue = new ConcurrentQueue<int>();
        int eventsPerProducer = N / ProducerCount;
        long totalConsumed = 0;

        var consumers = new Task[ConsumerCount];
        for (int i = 0; i < ConsumerCount; i++) {
            consumers[i] = Task.Run(() => {
                while (Interlocked.Read(ref totalConsumed) < N) {
                    if (queue.TryDequeue(out _)) {
                        Interlocked.Increment(ref totalConsumed);
                        Thread.SpinWait(10);
                    }
                }
            });
        }

        var producers = new Task[ProducerCount];
        for (int i = 0; i < ProducerCount; i++) {
            producers[i] = Task.Run(() => {
                for (int j = 0; j < eventsPerProducer; j++) {
                    queue.Enqueue(j);
                }
            });
        }

        Task.WaitAll(producers);
        Task.WaitAll(consumers);
    }

    /// <summary>
    /// Measures raw concurrent throughput of a bounded BlockingCollection when threads flow freely.
    /// </summary>
    [Benchmark]
    public void ThroughputBlockingCollection() {
        using var pipeline = new BlockingCollection<int>(boundedCapacity: BoundedLimit);
        int eventsPerProducer = N / ProducerCount;

        var consumers = new Task[ConsumerCount];
        for (int i = 0; i < ConsumerCount; i++) {
            consumers[i] = Task.Run(() => {
                foreach (var _ in pipeline.GetConsumingEnumerable()) {
                    Thread.SpinWait(10);
                }
            });
        }

        var producers = new Task[ProducerCount];
        for (int i = 0; i < ProducerCount; i++) {
            producers[i] = Task.Run(() => {
                for (int j = 0; j < eventsPerProducer; j++) {
                    pipeline.Add(j);
                }
            });
        }

        Task.WhenAll(producers).ContinueWith(_ => pipeline.CompleteAdding());
        Task.WaitAll(consumers);
    }

    /// <summary>
    /// Measures execution time for an unconstrained concurrent queue during a severe worker bottleneck.
    /// </summary>
    [Benchmark]
    public void BackpressureConcurrentQueue() {
        var queue = new ConcurrentQueue<int>();
        long totalConsumed = 0;

        var consumer = Task.Run(() => {
            while (Interlocked.Read(ref totalConsumed) < N) {
                if (queue.TryDequeue(out _)) {
                    Interlocked.Increment(ref totalConsumed);
                    Thread.SpinWait(500);
                }
            }
        });

        var producer = Task.Run(() => {
            for (int i = 0; i < N; i++) {
                queue.Enqueue(i);
            }
        });

        Task.WaitAll(producer, consumer);
    }

    /// <summary>
    /// Measures execution time for a bounded blocking collection during a severe worker bottleneck.
    /// </summary>
    [Benchmark]
    public void BackpressureBlockingCollection() {
        using var pipeline = new BlockingCollection<int>(boundedCapacity: BoundedLimit);

        var consumer = Task.Run(() => {
            foreach (var _ in pipeline.GetConsumingEnumerable()) {
                Thread.SpinWait(500);
            }
        });

        var producer = Task.Run(() => {
            for (int i = 0; i < N; i++) {
                pipeline.Add(i);
            }
            pipeline.CompleteAdding();
        });

        Task.WaitAll(producer, consumer);
    }
}