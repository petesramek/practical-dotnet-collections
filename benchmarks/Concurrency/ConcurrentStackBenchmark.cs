using BenchmarkDotNet.Attributes;
using System.Collections.Concurrent;
using static PracticalDotNetCollections.Benchmarks.Concurrency.BenchmarkThreads;

namespace PracticalDotNetCollections.Benchmarks.Concurrency;

/// <summary>
/// Benchmarks comparing <see cref="ConcurrentStack{T}"/> against a standard <see cref="Stack{T}"/> protected by a lock,
/// plus the bulk PushRange/TryPopRange APIs. All variants use dedicated threads and yield when the stack is empty.
/// Results depend heavily on core count and contention.
/// </summary>
[MemoryDiagnoser]
public class ConcurrentStackBenchmark {
    private const int ProducerCount = 4;
    private const int ConsumerCount = 4;

    /// <summary>
    /// The total number of elements pushed and popped during the execution pass.
    /// </summary>
    [Params(10_000, 50_000)]
    public int N;

    /// <summary>
    /// Lock-free ConcurrentStack with single-element Push/TryPop.
    /// </summary>
    [Benchmark]
    public void ConcurrentStackThroughput() {
        var stack = new ConcurrentStack<int>();
        int itemsPerProducer = N / ProducerCount;
        int remaining = itemsPerProducer * ProducerCount;

        var consumers = new Task[ConsumerCount];
        for (int i = 0; i < ConsumerCount; i++) {
            consumers[i] = Start(() => {
                while (Volatile.Read(ref remaining) > 0) {
                    if (stack.TryPop(out _)) {
                        Interlocked.Decrement(ref remaining);
                    } else {
                        Thread.Yield();
                    }
                }
            });
        }

        var producers = new Task[ProducerCount];
        for (int i = 0; i < ProducerCount; i++) {
            producers[i] = Start(() => {
                for (int j = 0; j < itemsPerProducer; j++) {
                    stack.Push(j);
                }
            });
        }

        Task.WaitAll(producers);
        Task.WaitAll(consumers);
    }

    /// <summary>
    /// Standard Stack guarded by a monitor lock.
    /// </summary>
    [Benchmark]
    public void StackWithLockThroughput() {
        var stack = new Stack<int>();
        var locker = new object();
        int itemsPerProducer = N / ProducerCount;
        int remaining = itemsPerProducer * ProducerCount;

        var consumers = new Task[ConsumerCount];
        for (int i = 0; i < ConsumerCount; i++) {
            consumers[i] = Start(() => {
                while (Volatile.Read(ref remaining) > 0) {
                    bool popped;
                    lock (locker) {
                        popped = stack.TryPop(out _);
                    }

                    if (popped) {
                        Interlocked.Decrement(ref remaining);
                    } else {
                        Thread.Yield();
                    }
                }
            });
        }

        var producers = new Task[ProducerCount];
        for (int i = 0; i < ProducerCount; i++) {
            producers[i] = Start(() => {
                for (int j = 0; j < itemsPerProducer; j++) {
                    lock (locker) {
                        stack.Push(j);
                    }
                }
            });
        }

        Task.WaitAll(producers);
        Task.WaitAll(consumers);
    }

    /// <summary>
    /// ConcurrentStack using PushRange/TryPopRange to move many items per atomic operation.
    /// </summary>
    [Benchmark]
    public void ConcurrentStackBulkRange() {
        var stack = new ConcurrentStack<int>();
        int itemsPerProducer = N / ProducerCount;
        int remaining = itemsPerProducer * ProducerCount;

        var consumers = new Task[ConsumerCount];
        for (int i = 0; i < ConsumerCount; i++) {
            consumers[i] = Start(() => {
                var popBuffer = new int[itemsPerProducer];
                while (Volatile.Read(ref remaining) > 0) {
                    int popped = stack.TryPopRange(popBuffer);
                    if (popped > 0) {
                        Interlocked.Add(ref remaining, -popped);
                    } else {
                        Thread.Yield();
                    }
                }
            });
        }

        var producers = new Task[ProducerCount];
        for (int i = 0; i < ProducerCount; i++) {
            producers[i] = Start(() => {
                var pushBuffer = new int[itemsPerProducer];
                for (int j = 0; j < itemsPerProducer; j++) {
                    pushBuffer[j] = j;
                }

                stack.PushRange(pushBuffer);
            });
        }

        Task.WaitAll(producers);
        Task.WaitAll(consumers);
    }
}
