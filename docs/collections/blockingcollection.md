# BlockingCollection<T>

## What it is

BlockingCollection<T> is a thread-safe wrapper that adds blocking and bounding capabilities on top of concurrent collections, enabling backpressure control in multi-threaded workflows.

## Typical use cases
- Multi-threaded producer-consumer pipelines that require backpressure to stop producers from overloading memory.
- Bounded processing queues with a strict maximum limit on item counts.
- Controlled throughput worker systems where consumer threads sleep automatically when the queue is empty.

## Sample usage

See:
[samples/blockingcollection-log-processing.cs](../../samples/blockingcollection-log-processing.cs)

### How to run the sample

```bash
dotnet run samples/blockingcollection-log-processing.cs
```

## Internal implementation

Wraps any collection implementing `IProducerConsumerCollection<T>` (defaulting to `ConcurrentQueue<T>` if none is provided). 

To handle blocking semantics without burning CPU cycles in spinning loops, it coordinates producers and consumers using low-level synchronization primitives (`SemaphoreSlim` and `ManualResetEventSlim`). When the collection is empty, consumer threads are put to sleep by the operating system until a producer adds an item. When a bounded capacity is reached, producing threads are put to sleep until a consumer removes an item.

## Memory characteristics
- **Dependent on backing store**: Memory footprint mirrors the underlying concurrent collection used (like `ConcurrentQueue<T>`).
- **Prevents Out-Of-Memory exceptions**: By enforcing a strict bounded capacity limit, it caps the maximum possible memory allocation, preventing uncontrolled data spikes from crashing the application pool.

## Complexity overview

- **Add Operations**: Fast element tracking when under the capacity limit. Pauses the executing thread if the collection is full.
- **Take Operations**: Fast element extraction when items are present. Pauses the executing thread if the collection is empty.

## Benchmark results

### Scenario

Compare thread-safe execution speeds between blocking synchronization pipelines and open non-blocking concurrent queues under producer-consumer workloads:
- BlockingCollection<T>
- ConcurrentQueue<T>

### How to run this benchmark only

```bash
cd benchmarks
dotnet run -c Release -- --filter *BlockingCollectionBenchmark*
```

### Benchmark code

[benchmarks/Concurrency/BlockingCollectionBenchmark.cs](../../benchmarks/Concurrency/BlockingCollectionBenchmark.cs)

### Results

Environment: BenchmarkDotNet 0.15.8; Windows 11 (10.0.26300.9711); virtual CPU `virt-10.0` at 1.00 GHz (1 CPU; 4 logical / 4 physical cores); .NET SDK 10.0.403; .NET 10.0.14, Arm64 RyuJIT (`armv8.0-a`); 3 warmup and 8 measurement iterations. [Full report](../../BenchmarkDotNet.Artifacts/results/PracticalDotNetCollections.Benchmarks.Concurrency.BlockingCollectionBenchmark-report-github.md).

| Method                                | N     | Mean        | Error      | StdDev    | Median       | Gen0      | Allocated  |
|-------------------------------------- |------ |------------:|-----------:|----------:|-------------:|----------:|-----------:|
| **ThroughputConcurrentQueue**             | **10000** |    **13.57 ms** |  **37.975 ms** | **13.542 ms** |     **8.408 ms** |   **31.2500** |   **78.14 KB** |
| ThroughputBlockingCollectionUnbounded | 10000 |    11.35 ms |  10.905 ms |  5.704 ms |     9.945 ms |   46.8750 |   64.07 KB |
| ThroughputBlockingCollectionBounded   | 10000 |    11.61 ms |   4.343 ms |  1.549 ms |    11.710 ms |  171.8750 |  329.87 KB |
| SlowConsumerConcurrentQueue           | 10000 |   293.32 ms |  95.139 ms | 49.760 ms |   283.103 ms |         - |  132.81 KB |
| SlowConsumerBlockingCollection        | 10000 |   210.69 ms |  17.827 ms |  7.915 ms |   211.919 ms |         - | 1511.81 KB |
| **ThroughputConcurrentQueue**             | **50000** |    **40.71 ms** |  **31.343 ms** | **13.917 ms** |    **37.965 ms** |         **-** |  **263.01 KB** |
| ThroughputBlockingCollectionUnbounded | 50000 |    49.52 ms |  16.076 ms |  8.408 ms |    50.141 ms |         - |  264.12 KB |
| ThroughputBlockingCollectionBounded   | 50000 |    33.55 ms |   7.760 ms |  4.059 ms |    34.249 ms | 1000.0000 | 2324.12 KB |
| SlowConsumerConcurrentQueue           | 50000 | 1,127.55 ms |  92.508 ms | 41.074 ms | 1,149.757 ms |         - |  517.73 KB |
| SlowConsumerBlockingCollection        | 50000 | 1,065.13 ms | 143.553 ms | 75.081 ms | 1,029.394 ms | 2000.0000 | 4080.78 KB |

### Interpretation

1. **Throughput is inconclusive**: the mean ordering changes with N, and several Error margins are large relative to their means. Consumers poll `TryDequeue` and yield when empty, adding scheduling noise that a blocking consumer does not have; do not choose a collection based on these timings alone.
2. **Slow consumer is consumer-bound**: at N = 50,000, both slow-consumer means are around one second and their Error margins overlap. Compare allocation and memory behavior as well.
3. **Bounded capacity**: BlockingCollection with a bound limits how many items are in flight, which is its main reason to exist. ConcurrentQueue has no bound, so a fast producer can grow it without limit. The benchmark measures run time and allocations, not peak queue length.
4. **Allocation**: the bounded variant allocated more than the unbounded variant in both tested sizes (330 KB vs 64 KB at N = 10,000; 2.3 MB vs 264 KB at N = 50,000). The benchmark measures allocated bytes, not peak queue length.

## Practical optimizations
- **Always provide a bounded capacity**: Initializing without an upper bound disables backpressure entirely, making it behave like a standard concurrent queue while still carrying the overhead of internal synchronization primitives.
- **Pass a CancellationToken**: Always pass a cancellation token to `.Take()` or `.GetConsumingEnumerable()` calls. If a background worker thread goes to sleep waiting for data that never arrives, a cancellation token is your only way to safely tear down the thread during an application shutdown.

## Common mistakes
- **Forgetting to call `CompleteAdding()`**: If your producers finish sending work but never call `.CompleteAdding()`, consumer loops using `GetConsumingEnumerable()` will hang indefinitely, freezing the worker thread and leaking system resources.
- **Using `BlockingCollection<T>` inside async-await tracks**: The blocking mechanisms in this collection explicitly block the active operating system thread. Calling `.Take()` inside an asynchronous method will tie up a thread pool worker. For modern `async/await` code bases, use `System.Threading.Channels` instead.
- **Reading item counts using `.Count` inside consumer loops**: Checking the item count before pulling data introduces a race condition where another consumer could steal the item first. Rely completely on thread-safe consumer extraction via `.Take()` or `.TryTake()`.

## When I would choose it
- When building synchronous, thread-bound background worker pipelines that require thread coordination.
- When you must restrict memory expansion by throttling producers using hardware-level backpressure.

## When I would avoid it
- When your architecture is built around asynchronous code workflows utilizing `Task`, `async`, and `await`. Use `Channel<T>` instead.
- When you do not need threads to sleep or pause based on capacity limits. If you just need a thread-safe list to throw data into, use `ConcurrentQueue<T>` or `ConcurrentDictionary<TKey, TValue>` directly.

## Rule of thumb

Use BlockingCollection<T> when you need synchronous, thread-safe backpressure and blocking capabilities to balance production and consumption workloads.
