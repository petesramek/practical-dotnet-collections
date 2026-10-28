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

| Method | N | Mean | Gen 0 | Gen 1 | Gen 2 | Allocated |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **ThreadLocalConcurrentBag** | 10000 | 128.0 us | 22.9492 | 22.7051 | 6.8359 | 114.41 KB |
| **ThreadLocalListWithLock** | 10000 | 589.1 us | 34.1797 | - | - | 66.69 KB |
| **WorkStealingConcurrentBag** | 10000 | 885.9 us | 25.3906 | 24.4141 | 7.8125 | 129.61 KB |
| **WorkStealingConcurrentQueue** | 10000 | 2,475.0 us | 58.5938 | 11.7188 | - | 132.81 KB |
| | | | | | | |
| **ThreadLocalConcurrentBag** | 50000 | 547.0 us | 83.0078 | 82.0313 | 29.2969 | 482.52 KB |
| **ThreadLocalListWithLock** | 50000 | 3,108.2 us | 113.2813 | 113.2813 | 113.2813 | 492.52 KB |
| **WorkStealingConcurrentBag** | 50000 | 4,185.1 us | 78.1250 | 70.3125 | 31.2500 | 515.01 KB |
| **WorkStealingConcurrentQueue** | 50000 | 13,227.4 us | 109.3750 | 109.3750 | 109.3750 | 517.78 KB |

Numbers come from a short BenchmarkDotNet job (3 warmup, 8 iterations) on an Arm64 machine with .NET 10. Results depend on core count and scheduler behavior.

### Interpretation

1. **Thread-local pattern**: when each thread consumes what it produced, ConcurrentBag was about 5.7x faster than a locked List<T> (547 us vs 3,108 us at N = 50,000). Each thread uses its own local storage, so there is little shared contention. This is the scenario the type is designed for.
2. **Work-stealing pattern**: when producers fill the bag and different consumers drain it, ConcurrentBag has to steal from other threads' lists and is slower than in the thread-local case. In this benchmark it was still about 3x faster than ConcurrentQueue (4.2 ms vs 13.2 ms at N = 50,000). Consumers in both variants yield on empty, so part of that gap may reflect scheduling rather than the collections themselves.
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
