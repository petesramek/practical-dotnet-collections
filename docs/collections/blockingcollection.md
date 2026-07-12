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

- **Add Operations**: Near-instant element tracking when under the capacity limit. Pauses the executing thread if the collection is full.
- **Take Operations**: Near-instant element extraction when items are present. Pauses the executing thread if the collection is empty.

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

| Method | N | Mean | Gen 0 | Gen 1 | Gen 2 | Allocated |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **ThroughputConcurrentQueue** | 10000 | 3.036 ms | 62.5000 | - | - | 132.68 KB |
| **ThroughputBlockingCollection** | 10000 | 4.885 ms | 390.6250 | - | - | 799.88 KB |
| **BackpressureConcurrentQueue** | 10000 | 179.931 ms | - | - | - | 132.17 KB |
| **BackpressureBlockingCollection** | 10000 | 183.028 ms | 666.6667 | - | - | 1729.13 KB |
| | | | | | | |
| **ThroughputConcurrentQueue** | 50000 | 16.030 ms | 93.7500 | 93.7500 | 93.7500 | 517.62 KB |
| **ThroughputBlockingCollection** | 50000 | 23.868 ms | 1562.5000 | - | - | 3187.33 KB |
| **BackpressureConcurrentQueue** | 50000 | 884.783 ms | - | - | - | 517.05 KB |
| **BackpressureBlockingCollection** | 50000 | 904.026 ms | 4000.0000 | - | - | 9950.81 KB |

### Interpretation

The metrics expose a classic software trade-off between raw speed and runtime safety:

1. **The Throughput and Lifetime Allocation Reality:** The raw throughput methods show that `ConcurrentQueue` processes balanced operations significantly faster and allocates less total lifetime memory than `BlockingCollection`. This is expected. `BlockingCollection` uses internal synchronization objects (`SemaphoreSlim` / `ManualResetEventSlim`) to manage thread boundaries, which introduces processing overhead and internal tracking allocations.
2. **The Memory Safety Deficit in ConcurrentQueue:** While `ConcurrentQueue` wins on raw speed, it lacks backpressure control. In the `Backpressure` test, the consumer is intentionally bottlenecked. Because `ConcurrentQueue` has no bounds, the fast producer dumps all 50,000 items into memory instantly. In a live production environment handling severe incoming traffic spikes or massive file streams, this unconstrained growth triggers a severe memory spike that can crash the server process.
3. **The Value of the Blocking Ceil:** `BlockingCollection` addresses this memory risk directly. Although it appears slower and generates higher lifetime garbage collections due to dynamic worker task objects, it enforces a strict upper limit on live data. The moment the collection size reaches its **500-item bounded capacity**, it automatically freezes the producing thread, stopping further allocations until consumers process existing items. 

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
