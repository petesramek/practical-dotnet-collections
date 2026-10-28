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

Environment: BenchmarkDotNet 0.15.8; Windows 11 (10.0.26300.9711); virtual CPU `virt-10.0` at 1.00 GHz (1 CPU; 4 logical / 4 physical cores); .NET SDK 10.0.403; .NET 10.0.14, Arm64 RyuJIT (`armv8.0-a`); 3 warmup and 8 measurement iterations. [Full report](../../BenchmarkDotNet.Artifacts/results/PracticalDotNetCollections.Benchmarks.Concurrency.ConcurrentQueueBenchmark-report-github.md).

| Method                    | N     | Mean      | Error      | StdDev     | Median    | Gen0    | Gen1    | Gen2    | Allocated |
|-------------------------- |------ |----------:|-----------:|-----------:|----------:|--------:|--------:|--------:|----------:|
| **ConcurrentQueueThroughput** | **10000** |  **3.831 ms** |  **0.8015 ms** |  **0.4192 ms** |  **3.876 ms** |       **-** |       **-** |       **-** |  **70.88 KB** |
| QueueWithLockThroughput   | 10000 | 14.480 ms | 29.3278 ms | 13.0217 ms |  9.678 ms | 46.8750 |       - |       - |  35.14 KB |
| **ConcurrentQueueThroughput** | **50000** | **34.101 ms** | **42.4219 ms** | **22.1875 ms** | **25.491 ms** | **93.7500** | **31.2500** | **31.2500** | **255.74 KB** |
| QueueWithLockThroughput   | 50000 | 13.068 ms |  6.0249 ms |  2.6751 ms | 12.008 ms | 62.5000 |       - |       - | 227.99 KB |

### Interpretation

1. **Timing is inconclusive**: the mean ordering changes between N = 10,000 and N = 50,000, and the Error margins are large for the locked-queue rows and the N = 50,000 `ConcurrentQueue` row. The consumers yield when the queue is empty, so scheduling noise is part of this workload; do not infer a general winner from these means.
2. **Allocation**: `ConcurrentQueue` allocated more in both runs (71 KB vs 35 KB at N = 10,000; 256 KB vs 228 KB at N = 50,000), consistent with allocating queue segments as it grows.
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
