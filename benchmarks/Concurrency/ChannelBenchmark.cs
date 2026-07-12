using BenchmarkDotNet.Attributes;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace PracticalDotNetCollections.Benchmarks;

/// <summary>
/// Benchmarks comparing asynchronous <see cref="Channel{T}"/> and synchronous <see cref="BlockingCollection{T}"/>.
/// Evaluates raw asynchronous throughput alongside behavioral mechanics under severe downstream consumption bottlenecks.
/// </summary>
[MemoryDiagnoser]
public class ChannelBenchmark {
    private const int ProducerCount = 4;
    private const int ConsumerCount = 2;
    private const int BoundedLimit = 500;

    /// <summary>
    /// The total number of items sent through the messaging pipelines.
    /// </summary>
    [Params(10_000, 50_000)]
    public int N;

    /// <summary>
    /// Measures concurrent throughput of an asynchronous bounded Channel when threads flow freely.
    /// </summary>
    [Benchmark]
    public async Task ThroughputChannel() {
        var channel = Channel.CreateBounded<int>(new BoundedChannelOptions(BoundedLimit) {
            FullMode = BoundedChannelFullMode.Wait,
            SingleWriter = false,
            SingleReader = false
        });

        int eventsPerProducer = N / ProducerCount;
        var writer = channel.Writer;
        var reader = channel.Reader;

        var consumers = new Task[ConsumerCount];
        for (int i = 0; i < ConsumerCount; i++) {
            consumers[i] = Task.Run(async () => {
                await foreach (var _ in reader.ReadAllAsync()) {
                    Thread.SpinWait(10);
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
    /// Measures concurrent throughput of a synchronous bounded BlockingCollection when threads flow freely.
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
    /// Measures execution metrics for an asynchronous channel during a severe downstream consumer bottleneck.
    /// </summary>
    [Benchmark]
    public async Task BackpressureChannel() {
        var channel = Channel.CreateBounded<int>(new BoundedChannelOptions(BoundedLimit) {
            FullMode = BoundedChannelFullMode.Wait
        });

        var writer = channel.Writer;
        var reader = channel.Reader;

        var consumer = Task.Run(async () => {
            await foreach (var _ in reader.ReadAllAsync()) {
                Thread.SpinWait(500);
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
    /// Measures execution metrics for a synchronous blocking collection during a severe downstream consumer bottleneck.
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