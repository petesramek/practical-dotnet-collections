using BenchmarkDotNet.Attributes;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PracticalDotNetCollections.Benchmarks;

[MemoryDiagnoser]
public class ConcurrentBagBenchmark
{
    [Params(10, 100, 1_000, 10_000)]
    public int N;

    [Benchmark]
    public void ConcurrentBagAddTake()
    {
        var bag = new ConcurrentBag<int>();

        Parallel.For(0, N, i =>
        {
            bag.Add(i);
        });

        while (bag.TryTake(out _)) { }
    }

    [Benchmark]
    public void ListWithLock()
    {
        var list = new List<int>();
        var locker = new object();

        Parallel.For(0, N, i =>
        {
            lock (locker)
            {
                list.Add(i);
            }
        });

        while (true)
        {
            lock (locker)
            {
                if (list.Count == 0) break;
                list.RemoveAt(list.Count - 1);
            }
        }
    }
}
