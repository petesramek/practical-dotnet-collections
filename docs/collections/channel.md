# Channel<T>

## What it is

Channel<T> is an asynchronous, thread-safe producer-consumer primitive designed for high-throughput pipelines.

## Typical use cases
- Async pipelines
- Background processing
- High-throughput messaging systems

## Sample usage

See:
[samples/channel-log-processing.cs](../../samples/channel-log-processing.cs)

### How to run the sample

```bash
dotnet run samples/channel-log-processing.cs
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

- **WriteAsync Operations**: Near-instant element tracking when under the capacity limit. Pauses the producer task asynchronously if full.
- **ReadAsync Operations**: Near-instant element extraction when items are present. Pauses the consumer task asynchronously if empty.

## Benchmark results

### Scenario

Measure async producer-consumer throughput using Channel<T>.

### How to run this benchmark only

```bash
cd benchmarks
dotnet run -c Release -- --filter *ChannelBenchmark*
```

### Benchmark code

[benchmarks/Concurrency/ChannelBenchmark.cs](../../benchmarks/Concurrency/ChannelBenchmark.cs)

### Results

| Method | N | Mean | Gen 0 | Gen 1 | Gen 2 | Allocated |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **ThroughputChannel** | 10000 | 5.078 ms | 187.5000 | - | - | 376.61 KB |
| **ThroughputBlockingCollection** | 10000 | 4.948 ms | 390.6250 | - | - | 792.87 KB |
| **BackpressureChannel** | 10000 | 186.953 ms | - | - | - | 6.24 KB |
| **BackpressureBlockingCollection** | 10000 | 181.945 ms | 666.6667 | - | - | 1919.05 KB |
| | | | | | | |
| **ThroughputChannel** | 50000 | 25.484 ms | 1187.5000 | - | - | 2,389.09 KB |
| **ThroughputBlockingCollection** | 50000 | 25.249 ms | 1437.5000 | - | - | 2,938.83 KB |
| **BackpressureChannel** | 50000 | 933.176 ms | - | - | - | 6.27 KB |
| **BackpressureBlockingCollection** | 50000 | 901.283 ms | 4000.0000 | - | - | 9,972.95 KB |

### Interpretation

The metrics expose a striking memory optimization milestone for asynchronous systems:

1. **The Raw Throughput Parity:** In unhindered tracks where threads flow freely, `Channel` and `BlockingCollection` finish at nearly identical speeds (~25 ms at N = 50,000). While `Channel` introduces a minor async execution layer, it reduces total lifetime data overhead by roughly 20%, requiring less total garbage collection attention.
2. **The Backpressure Allocation Elimination:** The true value of `Channel` is exposed under heavy consumer bottlenecks. At N = 50,000 with a slow worker, `BlockingCollection` generates a massive 9.9 MB of lifetime garbage allocations and triggers 4,000 Gen 0 collection cleanups because it relies on standard thread-synchronization objects. Under the exact same bottleneck, `Channel` drops allocations down to an astonishing 6.27 KB with zero garbage collection overhead.
3. **The Non-Blocking Edge:** Because `Channel` leverages asynchronous task coordination, threads are never forcefully put to sleep or context-switched by the operating system. Instead, the CPU cycles are safely yielded back to the application thread pool, maximizing runtime density and eliminating resource waste.

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
