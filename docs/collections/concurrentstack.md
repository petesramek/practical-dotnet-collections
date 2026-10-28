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

- **Push Operations**: Fast pointer index assignment. 
- **TryPop Operations**: Fast node subtraction if items exist on the head.

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

Environment: BenchmarkDotNet 0.15.8; Windows 11 (10.0.26300.9711); virtual CPU `virt-10.0` at 1.00 GHz (1 CPU; 4 logical / 4 physical cores); .NET SDK 10.0.403; .NET 10.0.14, Arm64 RyuJIT (`armv8.0-a`); 3 warmup and 8 measurement iterations. [Full report](../../BenchmarkDotNet.Artifacts/results/PracticalDotNetCollections.Benchmarks.Concurrency.ConcurrentStackBenchmark-report-github.md).

| Method                    | N     | Mean      | Error     | StdDev    | Median    | Gen0     | Gen1     | Allocated  |
|-------------------------- |------ |----------:|----------:|----------:|----------:|---------:|---------:|-----------:|
| **ConcurrentStackThroughput** | **10000** | **10.493 ms** | **21.438 ms** |  **9.519 ms** |  **5.787 ms** | **156.2500** |        **-** |  **316.43 KB** |
| StackWithLockThroughput   | 10000 | 19.315 ms | 21.858 ms |  9.705 ms | 18.023 ms |  46.8750 |        - |   35.51 KB |
| ConcurrentStackBulkRange  | 10000 | 12.899 ms | 12.192 ms |  5.413 ms |  9.054 ms | 203.1250 |        - |  394.42 KB |
| **ConcurrentStackThroughput** | **50000** | **19.561 ms** | **21.304 ms** | **11.142 ms** | **14.815 ms** | **666.6667** |        **-** | **1566.45 KB** |
| StackWithLockThroughput   | 50000 | 13.692 ms |  5.657 ms |  2.017 ms | 13.109 ms |  62.5000 |        - |  219.97 KB |
| ConcurrentStackBulkRange  | 50000 |  7.307 ms |  7.508 ms |  3.334 ms |  6.086 ms | 375.0000 | 312.5000 | 1956.93 KB |

### Interpretation

1. **Timing is inconclusive**: the Error margins are comparable to or larger than the means for several rows, including `ConcurrentStackThroughput` at both sizes. Do not rank the implementations by their means alone. Earlier claims of a lock-free or bulk-range speed advantage are not supported by this run.
2. **Allocation is higher for the concurrent stack**: at N = 50,000, `ConcurrentStackThroughput` allocated 1.57 MB versus 220 KB for `StackWithLockThroughput`, consistent with allocating a linked node for each push.
3. **Bulk range**: `PushRange` and `TryPopRange` are intended to reduce contention on the head pointer, but the table does not show a consistent timing advantage across both sizes. Measure with representative batch sizes before relying on a speedup.
4. **When to use it**: choose ConcurrentStack for LIFO semantics across threads without writing your own locking, not for allocation or raw speed.

## Practical optimizations
- **Always prefer bulk methods when working with datasets**: If you have multiple elements to insert or remove from a concurrent stack, do not use iterative loops calling `.Push()` or `.TryPop()`. Use `.PushRange()` and `.TryPopRange()` to execute the operations in a single, high-speed atomic operation.
- **Avoid querying counts**: Checking `.Count` on a lock-free linked collection forces the thread to iterate through the entire linked chain of items node-by-item. Rely entirely on `.TryPop()` or `.TryPopRange()` to evaluate availability safely.

## Common mistakes
- **Using in single-threaded code**: Instantiating a `ConcurrentStack` when only one execution path handles updates incurs unnecessary pointer tracking overhead. Use a standard `Stack<T>` instead.
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
