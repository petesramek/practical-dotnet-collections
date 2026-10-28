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

| Method | N | Mean | Gen 0 | Allocated |
| :--- | :--- | :--- | :--- | :--- |
| **ThroughputChannel** | 10000 | 6.223 ms | 156.2500 | 316.55 KB |
| **ThroughputBlockingCollection** | 10000 | 17.295 ms | 140.6250 | 262.4 KB |
| **SlowConsumerChannel** | 10000 | 255.256 ms | - | 6.16 KB |
| **SlowConsumerBlockingCollection** | 10000 | 268.188 ms | - | 365.88 KB |
| | | | | |
| **ThroughputChannel** | 50000 | 29.101 ms | 593.7500 | 1,232.01 KB |
| **ThroughputBlockingCollection** | 50000 | 34.824 ms | 666.6667 | 1,385.54 KB |
| **SlowConsumerChannel** | 50000 | 1,111.970 ms | - | 6.32 KB |
| **SlowConsumerBlockingCollection** | 50000 | 1,092.624 ms | 4000.0000 | 8,649.47 KB |

Numbers come from a short BenchmarkDotNet job (3 warmup, 8 iterations) on an Arm64 machine with .NET 10. Producers and consumers run on the thread pool for `Channel` and on dedicated threads for `BlockingCollection`, but results still depend on core count and scheduler behavior, and the error margins of the `BlockingCollection` rows are large.

### Interpretation

1. **Throughput**: with a fast consumer, `Channel` was faster than `BlockingCollection` in both runs (6.2 ms vs 17.3 ms at N = 10,000 and 29.1 ms vs 34.8 ms at N = 50,000), but the `BlockingCollection` error margins overlap the gap at N = 50,000. Treat the two as comparable in raw throughput.
2. **Slow consumer**: the run time is dominated by the consumer's work (about 1.1 s at N = 50,000 for both), so speed does not differ. Allocation does: `Channel` allocated about 6 KB while `BlockingCollection` allocated about 8.6 MB at N = 50,000, presumably from the blocking and wake-up bookkeeping while the bounded buffer is full. The benchmark does not isolate the cause.
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
