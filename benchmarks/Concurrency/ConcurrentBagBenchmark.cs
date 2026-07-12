using BenchmarkDotNet.Attributes;
using System.Collections.Concurrent;

namespace PracticalDotNetCollections.Benchmarks.Concurrency;

/// <summary>
/// Benchmarks evaluating <see cref="ConcurrentBag{T}"/> against a thread-safe locked <see cref="List{T}"/> and <see cref="ConcurrentQueue{T}"/>.
/// Measures optimal thread-local workflows versus cross-thread work-stealing contention patterns.
/// </summary>
[MemoryDiagnoser]
public class ConcurrentBagBenchmark {
    private const int WorkerCount = 4;

    /// <summary>
    /// The total number of items processed during the benchmark execution.
    /// </summary>
    [Params(10_000, 50_000)]
    public int N;

    /// <summary>
    /// Measures performance when multiple threads exclusively consume the exact same data items they produced.
    /// </summary>
    [Benchmark]
    public void ThreadLocalConcurrentBag() {
        var bag = new ConcurrentBag<int>();
        int itemsPerThread = N / WorkerCount;

        var workers = new Task[WorkerCount];
        for (int i = 0; i < WorkerCount; i++) {
            workers[i] = Task.Run(() => {
                for (int j = 0; j < itemsPerThread; j++) {
                    bag.Add(j);
                }

                // Optimal path: Thread drains its own local queue with zero lock contention
                for (int j = 0; j < itemsPerThread; j++) {
                    bag.TryTake(out _);
                }
            });
        }

        Task.WaitAll(workers);
    }

    /// <summary>
    /// Measures performance of a traditional locked list running the exact same thread-local pattern.
    /// </summary>
    [Benchmark]
    public void ThreadLocalListWithLock() {
        var list = new List<int>();
        var locker = new object();
        int itemsPerThread = N / WorkerCount;

        var workers = new Task[WorkerCount];
        for (int i = 0; i < WorkerCount; i++) {
            workers[i] = Task.Run(() => {
                for (int j = 0; j < itemsPerThread; j++) {
                    lock (locker) {
                        list.Add(j);
                    }
                }

                for (int j = 0; j < itemsPerThread; j++) {
                    lock (locker) {
                        if (list.Count > 0) {
                            list.RemoveAt(list.Count - 1);
                        }
                    }
                }
            });
        }

        Task.WaitAll(workers);
    }

    /// <summary>
    /// Measures performance degradation when distinct consumer threads are forced to steal data produced by different threads.
    /// </summary>
    [Benchmark]
    public void WorkStealingConcurrentBag() {
        var bag = new ConcurrentBag<int>();
        int itemsPerThread = N / WorkerCount;
        long totalConsumed = 0;

        // Separate producers fill the bag up front, localizing data to their own thread IDs
        var producers = new Task[WorkerCount];
        for (int i = 0; i < WorkerCount; i++) {
            producers[i] = Task.Run(() => {
                for (int j = 0; j < itemsPerThread; j++) {
                    bag.Add(j);
                }
            });
        }
        Task.WaitAll(producers);

        // Separate consumer threads are now forced to execute cross-thread work-stealing math
        var consumers = new Task[WorkerCount];
        for (int i = 0; i < WorkerCount; i++) {
            consumers[i] = Task.Run(() => {
                while (Interlocked.Read(ref totalConsumed) < N) {
                    if (bag.TryTake(out _)) {
                        Interlocked.Increment(ref totalConsumed);
                    }
                }
            });
        }
        Task.WaitAll(consumers);
    }

    /// <summary>
    /// Measures performance of a concurrent queue under the exact same cross-thread producer-consumer split workload.
    /// </summary>
    [Benchmark]
    public void WorkStealingConcurrentQueue() {
        var queue = new ConcurrentQueue<int>();
        int itemsPerThread = N / WorkerCount;
        long totalConsumed = 0;

        var producers = new Task[WorkerCount];
        for (int i = 0; i < WorkerCount; i++) {
            producers[i] = Task.Run(() => {
                for (int j = 0; j < itemsPerThread; j++) {
                    queue.Enqueue(j);
                }
            });
        }
        Task.WaitAll(producers);

        var consumers = new Task[WorkerCount];
        for (int i = 0; i < WorkerCount; i++) {
            consumers[i] = Task.Run(() => {
                while (Interlocked.Read(ref totalConsumed) < N) {
                    if (queue.TryDequeue(out _)) {
                        Interlocked.Increment(ref totalConsumed);
                    }
                }
            });
        }
        Task.WaitAll(consumers);
    }
}