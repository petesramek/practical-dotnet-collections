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

| Method | N | Mean | Gen 0 | Allocated |
| :--- | :--- | :--- | :--- | :--- |
| **ThroughputConcurrentQueue** | 10000 | 47.827 ms (median 29.474 ms) | - | 84.96 KB |
| **ThroughputBlockingCollectionUnbounded** | 10000 | 9.248 ms | - | 79.15 KB |
| **ThroughputBlockingCollectionBounded** | 10000 | 12.412 ms | 218.7500 | 414.29 KB |
| **SlowConsumerConcurrentQueue** | 10000 | 237.267 ms | - | 132.81 KB |
| **SlowConsumerBlockingCollection** | 10000 | 292.963 ms | 500.0000 | 1,622.21 KB |
| | | | | |
| **ThroughputConcurrentQueue** | 50000 | 27.901 ms | 93.7500 | 519.46 KB |
| **ThroughputBlockingCollectionUnbounded** | 50000 | 52.966 ms | - | 306.73 KB |
| **ThroughputBlockingCollectionBounded** | 50000 | 57.737 ms | 714.2857 | 1,690.45 KB |
| **SlowConsumerConcurrentQueue** | 50000 | 1,298.541 ms | - | 517.73 KB |
| **SlowConsumerBlockingCollection** | 50000 | 1,116.140 ms | - | 1,528.11 KB |

Numbers come from a short BenchmarkDotNet job (3 warmup, 8 iterations) on an Arm64 machine with .NET 10. Producers and consumers run on dedicated threads, and outcomes depend on core count and scheduler behavior. Several rows have error margins comparable to the mean, so treat differences below about 2x as inconclusive.

### Interpretation

1. **Throughput is inconclusive**: ConcurrentQueue and BlockingCollection swap places between N = 10,000 and N = 50,000, and the ConcurrentQueue N = 10,000 row has a very large error margin. The benchmark consumers poll TryDequeue and yield on empty, which adds scheduling noise that a blocking consumer does not have. Do not pick one over the other based on raw throughput here.
2. **Slow consumer is consumer-bound**: elapsed time (about 1.1 to 1.3 s at N = 50,000) is determined by the consumer's work, not by the collection. Compare allocation and memory behavior instead.
3. **Bounded capacity**: BlockingCollection with a bound limits how many items are in flight, which is its main reason to exist. ConcurrentQueue has no bound, so a fast producer can grow it without limit. The benchmark measures run time and allocations, not peak queue length.
4. **Allocation**: the bounded variant allocated about 5x more than the unbounded one at N = 10,000 (414 KB vs 79 KB), reflecting the blocking and wake-up bookkeeping.

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
