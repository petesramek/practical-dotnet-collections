using BenchmarkDotNet.Attributes;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace PracticalDotNetCollections.Benchmarks;

[MemoryDiagnoser]
public class BlockingCollectionBenchmark
{
    [Params(10, 100, 1_000, 10_000)]
    public int N;

    [Benchmark]
    public void BlockingCollectionProducerConsumer()
    {
        var collection = new BlockingCollection<int>(boundedCapacity: N);

        var producer = Task.Run(() =>
        {
            for (int i = 0; i < N; i++)
                collection.Add(i);

            collection.CompleteAdding();
        });

        var consumer = Task.Run(() =>
        {
            foreach (var item in collection.GetConsumingEnumerable()) { }
        });

        Task.WaitAll(producer, consumer);
    }

    [Benchmark]
    public void ConcurrentQueueNoBlocking()
    {
        var queue = new ConcurrentQueue<int>();

        Parallel.For(0, N, i => queue.Enqueue(i));

        while (queue.TryDequeue(out _)) { }
    }
}
