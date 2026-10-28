using BenchmarkDotNet.Attributes;
using System.Collections.Concurrent;
using System.Threading.Channels;
using static PracticalDotNetCollections.Benchmarks.Concurrency.BenchmarkThreads;

namespace PracticalDotNetCollections.Benchmarks.RelatedTypes;

/// <summary>
/// Benchmarks comparing asynchronous <see cref="Channel{T}"/> and synchronous <see cref="BlockingCollection{T}"/>
/// with identical bounds, producer/consumer counts, and simulated per-item work.
/// Channel work runs on the thread pool (it is non-blocking); BlockingCollection work runs on dedicated threads
/// because it blocks. In the slow consumer scenario elapsed time is consumer-bound; compare allocation and thread usage instead.
/// </summary>
[MemoryDiagnoser]
public class ChannelBenchmark {
    private const int ProducerCount = 4;
    private const int ConsumerCount = 2;
    private const int BoundedLimit = 500;

    /// <summary>
    /// The total number of items sent through the pipelines.
    /// </summary>
    [Params(10_000, 50_000)]
    public int N;

    /// <summary>
    /// Bounded Channel with several producers and consumers.
    /// </summary>
    [Benchmark]
    public async Task ThroughputChannel() {
        var channel = Channel.CreateBounded<int>(new BoundedChannelOptions(BoundedLimit) {
            FullMode = BoundedChannelFullMode.Wait
        });

        int eventsPerProducer = N / ProducerCount;
        var writer = channel.Writer;
        var reader = channel.Reader;

        var consumers = new Task[ConsumerCount];
        for (int i = 0; i < ConsumerCount; i++) {
            consumers[i] = Task.Run(async () => {
                await foreach (var _ in reader.ReadAllAsync()) {
                    Thread.SpinWait(FastWork);
                }
            });
        }

        var producers = new Task[ProducerCount];
        for (int i = 0; i < ProducerCount; i++) {
            producers[i] = Task.Run(async () => {
                for (int j = 0; j < eventsPerProducer; j++) {
                    await writer.WriteAsync(j);
                }
            });
        }

        await Task.WhenAll(producers);
        writer.Complete();
        await Task.WhenAll(consumers);
    }

    /// <summary>
    /// Bounded BlockingCollection with several producers and consumers.
    /// </summary>
    [Benchmark]
    public void ThroughputBlockingCollection() {
        using var pipeline = new BlockingCollection<int>(boundedCapacity: BoundedLimit);
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

    /// <summary>
    /// One producer, one slow consumer, bounded Channel. Elapsed time is consumer-bound.
    /// </summary>
    [Benchmark]
    public async Task SlowConsumerChannel() {
        var channel = Channel.CreateBounded<int>(new BoundedChannelOptions(BoundedLimit) {
            FullMode = BoundedChannelFullMode.Wait
        });

        var writer = channel.Writer;
        var reader = channel.Reader;

        var consumer = Task.Run(async () => {
            await foreach (var _ in reader.ReadAllAsync()) {
                Thread.SpinWait(SlowWork);
            }
        });

        var producer = Task.Run(async () => {
            for (int i = 0; i < N; i++) {
                await writer.WriteAsync(i);
            }
            writer.Complete();
        });

        await Task.WhenAll(producer, consumer);
    }

    /// <summary>
    /// One producer, one slow consumer, bounded BlockingCollection. Elapsed time is consumer-bound.
    /// </summary>
    [Benchmark]
    public void SlowConsumerBlockingCollection() {
        using var pipeline = new BlockingCollection<int>(boundedCapacity: BoundedLimit);

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
