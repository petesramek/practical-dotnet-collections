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

- **Add Operations**: Near-instant element tracking when localizing to the current thread queue.
- **Take Operations**: Near-instant and lock-free if the item exists in the thread's local queue. Incurs synchronization delays if it must steal from another thread.

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
| **ThreadLocalConcurrentBag** | 10000 | 95.82 us | 23.6816 | 23.5596 | 3.4180 | 117.21 KB |
| **ThreadLocalListWithLock** | 10000 | 445.48 us | 34.1797 | - | - | 65.4 KB |
| **WorkStealingConcurrentBag** | 10000 | 689.58 us | 24.4141 | 23.4375 | 3.9063 | 129.7 KB |
| **WorkStealingConcurrentQueue** | 10000 | 2,039.78 us | 58.5938 | 11.7188 | - | 132.82 KB |
| | | | | | | |
| **ThreadLocalConcurrentBag** | 50000 | 357.30 us | 86.9141 | 86.4258 | 39.5508 | 505.11 KB |
| **ThreadLocalListWithLock** | 50000 | 2,274.97 us | 113.2813 | 113.2813 | 113.2813 | 483.52 KB |
| **WorkStealingConcurrentBag** | 50000 | 3,508.61 us | 78.1250 | 74.2188 | 27.3438 | 514.05 KB |
| **WorkStealingConcurrentQueue** | 50000 | 11,227.34 us | 109.3750 | 109.3750 | 109.3750 | 517.77 KB |

### Interpretation

The metrics expose two vital architectural facts regarding thread coordination:

1. **The Thread-Local Dominance:** In pure thread-local patterns where threads consume exactly what they produce, `ConcurrentBag` runs over 6x faster than a traditionally locked list (~357 us vs ~2,274 us at N = 50,000). By utilizing decentralized, thread-specific storage tracks, it eliminates global lock contention entirely.
2. **The Work-Stealing Penalty and Unexpected Speed:** Under a split model where producers populate the collection before separate consumers drain it, `ConcurrentBag` encounters its internal work-stealing logic. This adds a minor execution delay compared to its optimal thread-local state. However, in this specific batch-based scenario, `ConcurrentBag` remains roughly 3x faster than `ConcurrentQueue` (~3.5 ms vs ~11.2 ms). This occurs because `ConcurrentQueue` suffers heavy multi-threaded contention on its global head/tail pointers when multiple consumer threads bombard the exact same memory slots simultaneously. 
3. **The Order Disregard Factor:** `ConcurrentBag` achieves these execution metrics because it makes zero promises regarding data ordering. It is a completely unordered bucket. If your system requires items to exit in a strict arrival sequence (FIFO) or reverse arrival sequence (LIFO), you cannot trade that correctness for speed.

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
