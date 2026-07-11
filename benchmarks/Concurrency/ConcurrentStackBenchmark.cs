using BenchmarkDotNet.Attributes;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PracticalDotNetCollections.Benchmarks;

[MemoryDiagnoser]
public class ConcurrentStackBenchmark
{
    [Params(10, 100, 1_000, 10_000)]
    public int N;

    [Benchmark]
    public void ConcurrentStackPushPop()
    {
        var stack = new ConcurrentStack<int>();

        Parallel.For(0, N, i =>
        {
            stack.Push(i);
        });

        while (stack.TryPop(out _)) { }
    }

    [Benchmark]
    public void StackWithLock()
    {
        var stack = new Stack<int>();
        var locker = new object();

        Parallel.For(0, N, i =>
        {
            lock (locker)
            {
                stack.Push(i);
            }
        });

        while (true)
        {
            lock (locker)
            {
                if (stack.Count == 0) break;
                stack.Pop();
            }
        }
    }
}
