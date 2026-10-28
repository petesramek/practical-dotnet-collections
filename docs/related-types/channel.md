# Channel<T>

## What it is

Channel<T> is an asynchronous, thread-safe producer-consumer primitive designed for high-throughput pipelines.

> `Channel<T>` is not a collection (it does not implement `ICollection<T>` or `IEnumerable<T>`); it is a related type kept here because it is the usual async alternative to `BlockingCollection<T>`.

## Typical use cases
- Async pipelines
- Background processing
- High-throughput messaging systems

## Sample usage

See:
[samples/related-types/channel-log-processing.cs](../../samples/related-types/channel-log-processing.cs)

### How to run the sample

```bash
dotnet run samples/related-types/channel-log-processing.cs
```

## Internal implementation

Uses lock-free techniques and async coordination between producers and consumers.

### Lookup flow
- No lookup
- Uses WriteAsync / ReadAllAsync

## Memory characteristics
- Backed by buffers (bounded/unbounded)
- Can apply backpressure

## Complexity overview

- **WriteAsync Operations**: Fast element tracking when under the capacity limit. Pauses the producer task asynchronously if full.
- **ReadAsync Operations**: Fast element extraction when items are present. Pauses the consumer task asynchronously if empty.

## Benchmark results

### Scenario

Measure async producer-consumer throughput using Channel<T>.

### How to run this benchmark only

```bash
cd benchmarks
dotnet run -c Release -- --filter *RelatedTypes.ChannelBenchmark*
```

### Benchmark code

[benchmarks/RelatedTypes/ChannelBenchmark.cs](../../benchmarks/RelatedTypes/ChannelBenchmark.cs)

### Results

Environment: BenchmarkDotNet 0.15.8; Windows 11 (10.0.26300.9711); virtual CPU `virt-10.0` at 1.00 GHz (1 CPU; 4 logical / 4 physical cores); .NET SDK 10.0.403; .NET 10.0.14, Arm64 RyuJIT (`armv8.0-a`); 3 warmup and 8 measurement iterations. [Full report](../../BenchmarkDotNet.Artifacts/results/PracticalDotNetCollections.Benchmarks.RelatedTypes.ChannelBenchmark-report-github.md).

| Method                         | N     | Mean         | Error       | StdDev     | Gen0      | Allocated  |
|------------------------------- |------ |-------------:|------------:|-----------:|----------:|-----------:|
| **ThroughputChannel**              | **10000** |     **5.648 ms** |   **0.4680 ms** |  **0.2448 ms** |  **156.2500** |   **316.9 KB** |
| ThroughputBlockingCollection   | 10000 |     7.765 ms |   0.9525 ms |  0.4982 ms |  156.2500 |  294.05 KB |
| SlowConsumerChannel            | 10000 |   261.602 ms | 109.1317 ms | 57.0780 ms |         - |    6.26 KB |
| SlowConsumerBlockingCollection | 10000 |   198.916 ms |  12.7234 ms |  5.6493 ms |  666.6667 | 1711.96 KB |
| **ThroughputChannel**              | **50000** |    **30.885 ms** |   **2.7146 ms** |  **1.4198 ms** |  **750.0000** | **1542.63 KB** |
| ThroughputBlockingCollection   | 50000 |    37.731 ms |   3.0132 ms |  1.3379 ms |  692.3077 | 1448.37 KB |
| SlowConsumerChannel            | 50000 | 1,145.841 ms |  47.3133 ms | 21.0074 ms |         - |    6.32 KB |
| SlowConsumerBlockingCollection | 50000 | 1,032.069 ms | 126.3392 ms | 66.0778 ms | 2000.0000 | 5202.84 KB |

### Interpretation

1. **Throughput**: with a fast consumer, `Channel` had the lower mean in both runs (5.65 ms vs 7.77 ms at N = 10,000; 30.9 ms vs 37.7 ms at N = 50,000). The Error ranges do not overlap in this run, though the benchmark environment and scheduling affect these results.
2. **Slow consumer**: at N = 50,000, the mean times were 1.15 s for `Channel` and 1.03 s for `BlockingCollection`; their Error ranges overlap. Allocation differed substantially: 6.32 KB for `Channel` versus 5.20 MB for `BlockingCollection`. The benchmark does not isolate the cause of the allocation difference.
3. **Threads**: `Channel` writers and readers await instead of blocking, so a waiting consumer does not hold a thread. In this benchmark `Channel` runs on the thread pool with `async`/`await` and `BlockingCollection` runs on dedicated threads (because it blocks), so the two rows are not identical setups; the benchmark does not measure thread usage directly.

## Practical optimizations
- **Enforce SingleWriter or SingleReader rules whenever possible**: Setting `SingleWriter = true` or `SingleReader = true` in your configuration options allows .NET to bypass thread-safe multi-contention locks, switching internally to a much faster, allocation-free execution model.
- **Prefer TryWrite over WriteAsync for non-blocking tracks**: If you are inside a critical loop and want to append an item without awaiting a task handle, use `.TryWrite()`. It attempts an instant entry-point assignment and returns a boolean status without any task allocations.

## Common mistakes
- **Using Channel in synchronous, thread-bound loops**: Forcing an asynchronous channel to run inside a synchronous thread workflow using `.Result` or `.Wait()` completely destroys the task scheduling safety and can easily trigger thread pool starvation.
- **Forgetting to call Complete() on the writer**: If your production loops finish processing data but never explicitly call `writer.Complete()`, consumers reading the pipeline via `reader.ReadAllAsync()` will hang indefinitely waiting for more entries, creating a permanent memory leak.

## When I would choose it
- In any modern application utilizing `async` and `await` task patterns for background queue processing.
- When memory usage and garbage collection pressure are critical constraints under severe processing loads.

## When I would avoid it
- When working inside legacy synchronous or thread-bound architectures that do not support the task async workflow. Use `BlockingCollection<T>` instead.
- When you do not require backpressure bounding or queue sleep coordination. If threads just need an unconstrained state cache map, use `ConcurrentQueue<T>` or `ConcurrentDictionary<TKey, TValue>`.

## Rule of thumb

Use Channel<T> for async producer-consumer pipelines.
