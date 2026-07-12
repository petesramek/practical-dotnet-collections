using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PracticalDotNetCollections.Benchmarks;

/// <summary>
/// Benchmarks evaluating <see cref="ConcurrentStack{T}"/> performance against a locked standard <see cref="Stack{T}"/>.
/// Measures multi-threaded push/pop contention alongside bulk atomic pointer range manipulations.
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
    /// Measures raw multi-threaded throughput using individual lock-free node pushes and pops.
    /// </summary>
    [Benchmark]
    public void ConcurrentStackThroughput() {
        var stack = new ConcurrentStack<int>();
        int itemsPerProducer = N / ProducerCount;
        long totalConsumed = 0;

        var consumers = new Task[ConsumerCount];
        for (int i = 0; i < ConsumerCount; i++) {
            consumers[i] = Task.Run(() => {
                while (Interlocked.Read(ref totalConsumed) < N) {
                    if (stack.TryPop(out _)) {
                        Interlocked.Increment(ref totalConsumed);
                    }
                }
            });
        }

        var producers = new Task[ProducerCount];
        for (int i = 0; i < ProducerCount; i++) {
            producers[i] = Task.Run(() => {
                for (int j = 0; j < itemsPerProducer; j++) {
                    stack.Push(j);
                }
            });
        }

        Task.WaitAll(producers);
        Task.WaitAll(consumers);
    }

    /// <summary>
    /// Measures concurrent performance using a standard stack protected by an exclusive global loop lock.
    /// </summary>
    [Benchmark]
    public void StackWithLockThroughput() {
        var stack = new Stack<int>();
        var locker = new object();
        int itemsPerProducer = N / ProducerCount;
        long totalConsumed = 0;

        var consumers = new Task[ConsumerCount];
        for (int i = 0; i < ConsumerCount; i++) {
            consumers[i] = Task.Run(() => {
                while (Interlocked.Read(ref totalConsumed) < N) {
                    lock (locker) {
                        if (stack.Count > 0) {
                            stack.Pop();
                            Interlocked.Increment(ref totalConsumed);
                        }
                    }
                }
            });
        }

        var producers = new Task[ProducerCount];
        for (int i = 0; i < ProducerCount; i++) {
            producers[i] = Task.Run(() => {
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
    /// Measures the massive performance benefit of using bulk atomic range pointer updates rather than individual loops.
    /// </summary>
    [Benchmark]
    public void ConcurrentStackBulkRange() {
        var stack = new ConcurrentStack<int>();
        int itemsPerProducer = N / ProducerCount;
        long totalConsumed = 0;

        var consumers = new Task[ConsumerCount];
        for (int i = 0; i < ConsumerCount; i++) {
            consumers[i] = Task.Run(() => {
                var popBuffer = new int[itemsPerProducer];
                while (Interlocked.Read(ref totalConsumed) < N) {
                    // Atomically pops multiple elements from the head pointer in a single cycle
                    int popped = stack.TryPopRange(popBuffer);
                    if (popped > 0) {
                        Interlocked.Add(ref totalConsumed, popped);
                    }
                }
            });
        }

        var producers = new Task[ProducerCount];
        for (int i = 0; i < ProducerCount; i++) {
            producers[i] = Task.Run(() => {
                var pushBuffer = new int[itemsPerProducer];
                for (int j = 0; j < itemsPerProducer; j++) {
                    pushBuffer[j] = j;
                }

                // Atomically links the entire array segment onto the stack head in a single operation
                stack.PushRange(pushBuffer);
            });
        }

        Task.WaitAll(producers);
        Task.WaitAll(consumers);
    }
}
