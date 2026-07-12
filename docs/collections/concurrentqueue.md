# ConcurrentQueue<T>

## What it is

ConcurrentQueue<T> is a thread-safe FIFO collection designed for concurrent producer-consumer scenarios.

## Typical use cases
- Producer-consumer pipelines
- Work queues across threads
- Background processing systems

## Sample usage

See:
[samples/concurrentqueue-log-streaming.cs](../../samples/concurrentqueue-log-streaming.cs)

### How to run the sample

```bash
dotnet run samples/concurrentqueue-log-streaming.cs
```

## Internal implementation

Lock-free, segment-based queue optimized for concurrent access.

### Lookup flow
- No direct lookup
- Uses TryDequeue / Enqueue

## Memory characteristics
- Segment-based allocation
- Higher overhead than Queue<T>
- Designed for concurrency

## Complexity overview

- **Enqueue Operations**: Near-instant pointer index assignment. 
- **TryDequeue Operations**: Near-instant extraction if items exist in the active head segment.

## Benchmark results

### Scenario

Compare concurrent queue vs locked queue:
- ConcurrentQueue<T>
- Queue<T> + lock

### How to run this benchmark only

```bash
cd benchmarks
dotnet run -c Release -- --filter *ConcurrentQueueBenchmark*
```

### Benchmark code

[benchmarks/Concurrency/ConcurrentQueueBenchmark.cs](../../benchmarks/Concurrency/ConcurrentQueueBenchmark.cs)

### Results

| Method | N | Mean | Gen 0 | Gen 1 | Gen 2 | Allocated |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **ConcurrentQueueThroughput** | 10000 | 34.379 ms | - | - | - | 126.92 KB |
| **QueueWithLockThroughput** | 10000 | 3.274 ms | 23.4375 | - | - | 50.5 KB |
| | | | | | | |
| **ConcurrentQueueThroughput** | 50000 | 43.087 ms | 76.9231 | 76.9231 | 76.9231 | 468.34 KB |
| **QueueWithLockThroughput** | 50000 | 6.472 ms | 78.1250 | 46.8750 | 46.8750 | 291.49 KB |

### Interpretation

The metrics expose an essential architectural reality regarding lock-free collection structures:

1. **The Lock-Free Throughput Penalty:** The numbers reveal that a standard queue wrapped in an exclusive global `lock` runs **nearly 7x faster** than `ConcurrentQueue` (~6.4 ms vs ~43.0 ms at N = 50,000). This occurs because `ConcurrentQueue` utilizes complex atomic memory synchronization checks (`Interlocked` loops and state comparisons) to manage its segmented tracking architecture across multiple threads. Under tight, non-stop thread loop collisions from an empty state, this logic generates significantly higher CPU spinning cycles than a plain hardware mutex loop lock.
2. **The Allocation Overhead:** `ConcurrentQueue` allocates more lifetime memory over the benchmark execution. This is expected due to the structural overhead of creating tracking references for each internal multi-element array chunk segment.
3. **The Lock-Free Use Case Reality:** Lock-free data structures are built to protect applications from thread blocking and lock saturation—not to speed up tight insertion loop benchmarks. In real-world enterprise architectures where processing logic or I/O waits exist between insertions, the global lock on a standard queue creates massive thread blocking stalls. `ConcurrentQueue` prevents this by ensuring threads can always make execution progress independently.

## Practical optimizations
- **Avoid manually tracking counts**: Do not write loops that check `.Count` before pulling items. Querying the count forces .NET to traverse the segment chain, hurting performance. Rely entirely on `.TryDequeue()` to handle presence testing and item extraction in a single, safe step.
- **Prefer modern Channels for async workflows**: If your producer-consumer architecture is built around asynchronous async/await pipelines, do not try to adapt `ConcurrentQueue`. Use `System.Threading.Channels` directly instead.

## Common mistakes
- **Using Queue<T> with locks unnecessarily**: While a locked `Queue<T>` performs exceptionally well in highly localized, empty-state benchmarks, it introduces significant risk in real applications. If multiple threads must perform heavy processing, holding a global lock creates thread execution stalls across the entire application pool.

## When I would choose it
- Multiple producers/consumers
- High concurrency
- When you want to eliminate global thread-blocking lock saturation in non-async, thread-heavy background tracking daemons.

## When I would avoid it
- Single-threaded -> use Queue<T>
- When working within single-threaded tracking loops.

## Rule of thumb

Use ConcurrentQueue<T> for thread-safe FIFO scenarios.
