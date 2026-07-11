using BenchmarkDotNet.Attributes;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PracticalDotNetCollections.Benchmarks;

[MemoryDiagnoser]
public class ConcurrentQueueBenchmark
{
    [Params(10, 100, 1_000, 10_000)]
    public int N;

    [Benchmark]
    public void ConcurrentQueueEnqueueDequeue()
    {
        var queue = new ConcurrentQueue<int>();

        Parallel.For(0, N, i =>
        {
            queue.Enqueue(i);
        });

        while (queue.TryDequeue(out _)) { }
    }

    [Benchmark]
    public void QueueWithLock()
    {
        var queue = new Queue<int>();
        var locker = new object();

        Parallel.For(0, N, i =>
        {
            lock (locker)
            {
                queue.Enqueue(i);
            }
        });

        while (true)
        {
            lock (locker)
            {
                if (queue.Count == 0) break;
                queue.Dequeue();
            }
        }
    }
}
