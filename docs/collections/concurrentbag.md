# ConcurrentBag<T>

## What it is

ConcurrentBag<T> is a thread-safe, unordered collection optimized for high-throughput concurrent scenarios.

## Typical use cases
- Parallel processing where order does not matter
- Work stealing scenarios
- High-throughput producer-consumer pipelines

## Sample usage

See:
[samples/concurrentbag-task-distribution.cs](../../samples/concurrentbag-task-distribution.cs)

### How to run the sample

```bash
dotnet run samples/concurrentbag-task-distribution.cs
```

## Internal implementation

Uses thread-local storage and work-stealing algorithms.

### Lookup flow
- No ordering guarantees
- Uses TryTake / Add

## Memory characteristics
- Thread-local buffers
- Higher memory overhead
- Optimized for throughput

## Complexity overview

- **Add Operations**: Fast element tracking when localizing to the current thread queue.
- **Take Operations**: Fast and lock-free if the item exists in the thread's local queue. Incurs synchronization delays if it must steal from another thread.

## Benchmark results

### Scenario

Compare concurrent bag vs locked list:
- ConcurrentBag<T>
- List<T> + lock

### How to run this benchmark only

```bash
cd benchmarks
dotnet run -c Release -- --filter *ConcurrentBagBenchmark*
```

### Benchmark code

[benchmarks/Concurrency/ConcurrentBagBenchmark.cs](../../benchmarks/Concurrency/ConcurrentBagBenchmark.cs)

### Results

Environment: BenchmarkDotNet 0.15.8; Windows 11 (10.0.26300.9711); virtual CPU `virt-10.0` at 1.00 GHz (1 CPU; 4 logical / 4 physical cores); .NET SDK 10.0.403; .NET 10.0.14, Arm64 RyuJIT (`armv8.0-a`); 3 warmup and 8 measurement iterations. [Full report](../../BenchmarkDotNet.Artifacts/results/PracticalDotNetCollections.Benchmarks.Concurrency.ConcurrentBagBenchmark-report-github.md).

| Method                      | N     | Mean        | Error     | StdDev    | Gen0     | Gen1     | Gen2     | Allocated |
|---------------------------- |------ |------------:|----------:|----------:|---------:|---------:|---------:|----------:|
| **ThreadLocalConcurrentBag**    | **10000** |    **120.1 μs** |  **16.01 μs** |   **7.11 μs** |  **25.5127** |  **25.3906** |   **3.7842** | **126.07 KB** |
| ThreadLocalListWithLock     | 10000 |    579.4 μs |  81.80 μs |  42.78 μs |  33.2031 |        - |        - |  63.19 KB |
| WorkStealingConcurrentBag   | 10000 |    774.3 μs | 179.01 μs |  93.62 μs |  25.3906 |  24.4141 |   2.9297 | 129.56 KB |
| WorkStealingConcurrentQueue | 10000 |  2,245.0 μs | 167.33 μs |  87.51 μs |  58.5938 |  11.7188 |        - | 132.81 KB |
| **ThreadLocalConcurrentBag**    | **50000** |    **454.1 μs** |  **58.49 μs** |  **30.59 μs** |  **85.9375** |  **83.9844** |  **31.2500** | **508.04 KB** |
| ThreadLocalListWithLock     | 50000 |  3,470.5 μs | 219.05 μs |  97.26 μs | 105.4688 | 105.4688 | 105.4688 | 470.51 KB |
| WorkStealingConcurrentBag   | 50000 |  4,238.2 μs | 666.07 μs | 348.37 μs |  78.1250 |  70.3125 |  15.6250 | 515.01 KB |
| WorkStealingConcurrentQueue | 50000 | 12,437.6 μs | 625.99 μs | 327.40 μs | 109.3750 | 109.3750 | 109.3750 | 517.76 KB |

### Interpretation

1. **Thread-local pattern**: when each thread consumes what it produced, the N = 50,000 mean for `ThreadLocalConcurrentBag` was 454 μs versus 3,471 μs for the locked list. Each thread uses its own local storage, so there is little shared contention; this is the scenario the type is designed for.
2. **Work-stealing pattern**: when producers fill the bag and different consumers drain it, the bag must steal from other threads' lists. At N = 50,000, `WorkStealingConcurrentBag` averaged 4.24 ms versus 12.44 ms for `WorkStealingConcurrentQueue`; both timing rows include Error margins, and consumers yield on empty, so scheduling can affect the comparison.
3. **No ordering**: ConcurrentBag makes no ordering promises. If you need FIFO or LIFO, use ConcurrentQueue<T> or ConcurrentStack<T> instead of trading correctness for speed.

## Practical optimizations
- **Keep work items localized to the originating thread**: Structure your background worker logic so that threads push metadata objects into the bag and pull them back out inside their own execution cycles, keeping the code on the optimal lock-free path.
- **Avoid when ordering matters**: Never use `ConcurrentBag` if elements must be evaluated sequentially. It is strictly built to serve as an unordered repository.

## Common mistakes
- **Expecting FIFO or LIFO behavior**: `ConcurrentBag` gives no ordering guarantees. Items are retrieved in an unpredictable sequence based on thread access patterns.
- **Relying on `.Count` or `.IsEmpty` inside performance loops**: Calling `.Count` on a `ConcurrentBag` forces .NET to freeze all thread interaction and acquire locks across every internal thread queue to tally up the elements safely. Rely instead on thread-safe `.TryTake()` loops.

## When I would choose it
- When implementing thread-local resource caching pools (like reusing large string arrays or memory segments within specific worker loops).
- When distributing broad, unordered parallel chunks of tasks where workers process their own items and only occasionally steal work from neighbors when finished.

## When I would avoid it
- When elements must maintain a strict arrival order (FIFO/LIFO sequence checks). Use `ConcurrentQueue<T>` or `ConcurrentStack<T>`.
- When a single thread handles all item generation while completely different threads handle all item consumption. Use `ConcurrentQueue<T>` or `Channel<T>`.

## Rule of thumb

Use ConcurrentBag<T> when you need maximum throughput and don’t care about order.
