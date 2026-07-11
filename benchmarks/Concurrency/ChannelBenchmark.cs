using BenchmarkDotNet.Attributes;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace PracticalDotNetCollections.Benchmarks;

[MemoryDiagnoser]
public class ChannelBenchmark
{
    [Params(10, 100, 1_000, 10_000)]
    public int N;

    [Benchmark]
    public async Task ChannelProducerConsumer()
    {
        var channel = Channel.CreateBounded<int>(new BoundedChannelOptions(N)
        {
            FullMode = BoundedChannelFullMode.Wait
        });

        var writer = channel.Writer;
        var reader = channel.Reader;

        var producer = Task.Run(async () =>
        {
            for (int i = 0; i < N; i++)
            {
                await writer.WriteAsync(i);
            }
            writer.Complete();
        });

        var consumer = Task.Run(async () =>
        {
            await foreach (var item in reader.ReadAllAsync()) { }
        });

        await Task.WhenAll(producer, consumer);
    }
}
