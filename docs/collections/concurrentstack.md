# ConcurrentStack<T>

## What it is

ConcurrentStack<T> is a thread-safe LIFO collection designed for concurrent access without external locking.

## Typical use cases
- Parallel work processing (LIFO)
- Task scheduling systems
- Backtracking with concurrency

## Sample usage

See:
[samples/concurrentstack-undo-actions.cs](../../samples/concurrentstack-undo-actions.cs)

### How to run the sample

```bash
dotnet run samples/concurrentstack-undo-actions.cs
```

## Internal implementation

Lock-free stack using compare-and-swap operations.

### Lookup flow
- No lookup support
- Only Push / TryPop

## Memory characteristics
- Node-based structure
- Higher overhead than Stack<T>
- Designed for concurrency

## Complexity overview

- **Push Operations**: Near-instant pointer pointer index assignment. 
- **TryPop Operations**: Near-instant node subtraction if items exist on the head.

## Benchmark results

### Scenario

Compare concurrent stack vs locked stack:
- ConcurrentStack<T>
- Stack<T> + lock

### How to run this benchmark only

```bash
cd benchmarks
dotnet run -c Release -- --filter *ConcurrentStackBenchmark*
```

### Benchmark code

[benchmarks/Concurrency/ConcurrentStackBenchmark.cs](../../benchmarks/Concurrency/ConcurrentStackBenchmark.cs)

### Results

| Method | N | Mean | Gen 0 | Gen 1 | Gen 2 | Allocated |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **ConcurrentStackThroughput** | 10000 | 3.310 ms | 93.7500 | 31.2500 | - | 313.61 KB |
| **StackWithLockThroughput** | 10000 | 2.445 ms | 19.5313 | - | - | 42.14 KB |
| **ConcurrentStackBulkRange** | 10000 | 2.247 ms | 191.4063 | 3.9063 | - | 391.85 KB |
| | | | | | | |
| **ConcurrentStackThroughput** | 50000 | 4.827 ms | 281.2500 | 234.3750 | - | 1563.62 KB |
| **StackWithLockThroughput** | 50000 | 6.266 ms | 85.9375 | 39.0625 | 39.0625 | 248.48 KB |
| **ConcurrentStackBulkRange** | 50000 | **2.775 ms** | 398.4375 | 257.8125 | - | 1954.32 KB |

### Interpretation

The metrics expose two vital architectural facts regarding single-pointer contention tracks:

1. **The Small-Scale Contention Trap:** At small scales (N = 10,000), individual operations inside `ConcurrentStackThroughput` are slower than a plain stack protected by a global monitor lock (3.31 ms vs 2.44 ms). This occurs because `ConcurrentStack` manages additions and subtractions by executing continuous `Interlocked.CompareExchange` loops on a single global Head pointer. When threads hammer the exact same pointer slot concurrently, repeated validation failures force the hardware into expensive retry cycles. 
2. **The High-Contention Tipping Point:** As load scales to N = 50,000, the lock-free structure begins to outpace the lock. `ConcurrentStackThroughput` completes in 4.82 ms, whereas the locked stack degrades to 6.26 ms and triggers heavy Gen 1/Gen 2 garbage collection cycles. While lock retries burn CPU cycles, they prevent operating system threads from physically putting themselves to sleep (context switching), which keeps high-volume execution fluid.
3. **The Bulk Range Victory:** The ultimate optimization edge of `ConcurrentStack` is validated by `ConcurrentStackBulkRange`. At N = 50,000, using `.PushRange()` and `.TryPopRange()` cuts execution time down to **2.77 ms**—making it over **2.2x faster than a locked stack** and roughly **1.7x faster than the item-by-item lock-free loop**. Threads build or drain their linked list node branches entirely in private local arrays, touching the volatile global Head pointer exactly *once* per batch to swap states. This design removes thread cache collisions, delivering maximum execution speed under load.

## Practical optimizations
- **Always prefer bulk methods when working with datasets**: If you have multiple elements to insert or remove from a concurrent stack, do not use iterative loops calling `.Push()` or `.TryPop()`. Use `.PushRange()` and `.TryPopRange()` to execute the operations in a single, high-speed atomic operation.
- **Avoid querying counts**: Checking `.Count` on a lock-free linked collection forces the thread to iterate through the entire linked chain of items node-by-item. Rely entirely on `.TryPop()` or `.TryPopRange()` to evaluate availability safely.

## Common mistakes
- **Using in single-threaded code**: Instantiating a `ConcurrentStack` when only one execution path handles updates incurs completely useless pointer tracking overhead. Use a standard `Stack<T>` instead.
- **Using single-element pushes under extreme multi-threaded stress**: If dozens of background threads are non-stop slamming a singular `ConcurrentStack` with individual items, they will encounter heavy CAS collision stalls. Switch to `ConcurrentQueue` (which decouples head and tail pointers) or use bulk range arrays.

## When I would choose it
- Multiple threads push/pop data
- When your system architecture strictly requires last-in, first-out execution constraints (LIFO) across multiple threads.
- When processing pipelines handle data objects in native pre-allocated blocks or batches, letting you fully maximize `.PushRange()` performance.

## When I would avoid it
- Single-threaded -> use Stack<T>
- When items must exit in the exact sequential order they arrived. Use `ConcurrentQueue<T>` or `Channel<T>`.

## Rule of thumb

Use ConcurrentStack<T> when LIFO behavior and concurrency are required.
