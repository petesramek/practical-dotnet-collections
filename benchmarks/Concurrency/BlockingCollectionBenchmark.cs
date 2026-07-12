using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace PracticalDotNetCollections.Benchmarks;

[MemoryDiagnoser]
public class BlockingCollectionBenchmark {
    private const int ProducerCount = 4;
    private const int ConsumerCount = 2;
    private const int BoundedLimit = 500;

    public static int LatestConcurrentQueuePeak;
    public static int LatestBlockingCollectionPeak;

    [Params(10_000, 50_000)]
    public int N;

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

    [Benchmark]
    public void BackpressureConcurrentQueue() {
        var queue = new ConcurrentQueue<int>();
        int peakCount = 0;
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
                int currentCount = queue.Count;
                if (currentCount > peakCount)
                    peakCount = currentCount;
            }
        });

        Task.WaitAll(producer, consumer);
        LatestConcurrentQueuePeak = peakCount;
    }

    [Benchmark]
    public void BackpressureBlockingCollection() {
        using var pipeline = new BlockingCollection<int>(boundedCapacity: BoundedLimit);
        int peakCount = 0;

        var consumer = Task.Run(() => {
            foreach (var _ in pipeline.GetConsumingEnumerable()) {
                Thread.SpinWait(500);
            }
        });

        var producer = Task.Run(() => {
            for (int i = 0; i < N; i++) {
                pipeline.Add(i);
                int currentCount = pipeline.Count;
                if (currentCount > peakCount)
                    peakCount = currentCount;
            }
            pipeline.CompleteAdding();
        });

        Task.WaitAll(producer, consumer);
        LatestBlockingCollectionPeak = peakCount;
    }
}