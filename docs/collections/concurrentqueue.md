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

- **Enqueue Operations**: Fast pointer index assignment. 
- **TryDequeue Operations**: Fast extraction if items exist in the active head segment.

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
| **ConcurrentQueueThroughput** | 10000 | 33.219 ms (median 22.295 ms) | 31.2500 | - | - | 54.68 KB |
| **QueueWithLockThroughput** | 10000 | 9.611 ms | 46.8750 | - | - | 34.64 KB |
| | | | | | | |
| **ConcurrentQueueThroughput** | 50000 | 24.281 ms | 109.3750 | 31.2500 | 31.2500 | 259.76 KB |
| **QueueWithLockThroughput** | 50000 | 24.880 ms | 85.9375 | 7.8125 | 7.8125 | 171.47 KB |

Numbers come from a short BenchmarkDotNet job (3 warmup, 8 iterations) on an Arm64 machine with .NET 10. Producers and consumers run on dedicated threads, and outcomes depend on core count and scheduler behavior. Several rows have error margins comparable to the mean, so treat differences below about 2x as inconclusive.

### Interpretation

1. **No clear winner**: at N = 50,000 both implementations take about 24 ms. At N = 10,000 the locked queue was faster, but the ConcurrentQueue row has a very large error margin (about 56 ms), so that gap is not reliable. An earlier version of this benchmark showed the locked queue about 7x faster; that came from consumers busy-spinning on an empty queue, and the benchmark has been changed so consumers yield instead.
2. **Allocation**: ConcurrentQueue allocated more in both runs (260 KB vs 171 KB at N = 50,000), because it links fixed-size segments as it grows.
3. **When to use it**: a short, tight lock around a plain Queue<T> is competitive when contention is low and critical sections are tiny. ConcurrentQueue removes the need for you to write and maintain the lock, and it avoids holding a lock while a consumer is descheduled. Choose it for those properties, not for raw benchmark speed.

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
