# ConcurrentDictionary<TKey, TValue>

## What it is

ConcurrentDictionary<TKey, TValue> is a thread-safe dictionary designed for concurrent access without external locking.

## Typical use cases
- Shared state across threads
- Caching in multi-threaded applications
- Parallel processing pipelines

## Sample usage

See:
[samples/concurrentdictionary-state-cache.cs](../../samples/concurrentdictionary-state-cache.cs)

### How to run the sample

```bash
dotnet run samples/concurrentdictionary-state-cache.cs
```

## Internal implementation

Uses fine-grained locking and lock-free techniques.

### Lookup flow
- Lock-free reads where possible

## Memory characteristics
- Higher overhead than Dictionary
- Designed for concurrency, not minimal memory

## Complexity overview

- **Read Access**: Fast and entirely lock-free across all thread tracks.
- **Write/Update Access**: Fast bucket tracking. It incurs minor thread synchronization delays only if two separate threads try to mutate keys that hash into the exact same localized bucket partition.

## Benchmark results

### Scenario

Compare concurrent writes:
- ConcurrentDictionary
- Dictionary + lock

### How to run this benchmark only

```bash
cd benchmarks
dotnet run -c Release -- --filter *ConcurrentDictionaryBenchmark*
```

### Benchmark code

[benchmarks/Concurrency/ConcurrentDictionaryBenchmark.cs](../../benchmarks/Concurrency/ConcurrentDictionaryBenchmark.cs)

### Results

Environment: BenchmarkDotNet 0.15.8; Windows 11 (10.0.26300.9711); virtual CPU `virt-10.0` at 1.00 GHz (1 CPU; 4 logical / 4 physical cores); .NET SDK 10.0.403; .NET 10.0.14, Arm64 RyuJIT (`armv8.0-a`); 3 warmup and 8 measurement iterations. [Full report](../../BenchmarkDotNet.Artifacts/results/PracticalDotNetCollections.Benchmarks.Concurrency.ConcurrentDictionaryBenchmark-report-github.md).

| Method                             | N     | Mean         | Error       | StdDev       | Gen0      | Gen1     | Gen2     | Allocated  |
|----------------------------------- |------ |-------------:|------------:|-------------:|----------:|---------:|---------:|-----------:|
| **ConcurrentDictionaryAdd**            | **10000** |  **2,526.25 μs** |   **587.72 μs** |   **307.389 μs** |  **218.7500** |  **93.7500** |  **31.2500** | **1299.02 KB** |
| DictionaryAddWithLock              | 10000 |    978.41 μs |   249.46 μs |   130.473 μs |  214.8438 |  85.9375 |  82.0313 |  664.96 KB |
| GetOrAddWithAllocatingClosure      | 10000 |    266.57 μs |    31.52 μs |    16.487 μs |  425.2930 |        - |        - |  861.77 KB |
| GetOrAddWithStatefulAllocationFree | 10000 |     46.87 μs |    14.08 μs |     7.364 μs |    1.1597 |        - |        - |    2.33 KB |
| **ConcurrentDictionaryAdd**            | **50000** | **17,689.75 μs** | **3,948.38 μs** | **2,065.078 μs** | **1031.2500** | **687.5000** | **250.0000** | **5796.43 KB** |
| DictionaryAddWithLock              | 50000 |  4,167.43 μs | 1,034.45 μs |   459.302 μs |  570.3125 | 429.6875 | 429.6875 | 2853.34 KB |
| GetOrAddWithAllocatingClosure      | 50000 |  1,500.22 μs |   217.68 μs |   113.852 μs | 2115.2344 |        - |        - |  4299.4 KB |
| GetOrAddWithStatefulAllocationFree | 50000 |  6,618.08 μs | 5,724.10 μs | 2,993.818 μs |         - |        - |        - |    2.35 KB |

### Interpretation

The metrics expose two vital architectural realities regarding shared map tracking:

1. **Insertion contention**: at N = 50,000, `DictionaryAddWithLock` averaged 4.17 ms and allocated 2.85 MB, compared with 17.69 ms and 5.80 MB for `ConcurrentDictionaryAdd`. The Error margins do not overlap in this run. The locked dictionary is a useful single-run comparison, not a general substitute for thread-safe access.
2. **Lambda closure allocations**: `GetOrAddWithAllocatingClosure` allocated 4.30 MB at N = 50,000, while `GetOrAddWithStatefulAllocationFree` allocated 2.35 KB. The stateful overload avoids the per-call closure allocation, but its N = 50,000 timing has a very large Error margin; these results do not establish a consistent speed advantage.
3. **Stateful overload**: pass captured context through the overload's state parameter and use a static factory to avoid closure allocations. Compare the Mean, Error, and StdDev columns for the timing; the allocation difference is clearer than the timing result in this run.

## Practical optimizations
- **Always use the stateful overloads for factory lambdas**: When using `.GetOrAdd()` or `.AddOrUpdate()`, pass external scope variables into the state parameter. This keeps your callback delegates allocation-free.
- **Preallocate the concurrency level and capacity defaults if known**: If you know how many unique threads will actively bombard the collection alongside the total expected map count, pass both variables to the constructor (`new ConcurrentDictionary<K, V>(concurrencyLevel, expectedCount)`) to lock down bucket structures immediately.

## Common mistakes
- **Using in single-threaded code**: Instantiating a `ConcurrentDictionary` when only one thread ever executes modifications introduces unnecessary lock-stripe setup overhead and pointer safety layers. Use a standard `Dictionary<TKey, TValue>` instead.
- **Assuming factory delegates run exactly once per key evaluation**: The factory lambda inside `.GetOrAdd()` is not execution-atomic. If two separate threads call `.GetOrAdd()` on a missing key at the exact same moment, both factory lambdas may execute simultaneously in a race, with one result thrown away. Ensure factory delegates remain free of operational side effects.

## When I would choose it
- Multiple threads access shared dictionary
- When creating persistent, application-wide configuration caches or session tracking pools read and updated continuously across parallel worker loops.

## When I would avoid it
- Single-threaded -> use Dictionary
- When a collection is built and updated by exactly one single thread before being exposed as a read-only snapshot to other threads. Use a standard `Dictionary<TKey, TValue>` or a `FrozenDictionary<TKey, TValue>`.

## Rule of thumb

Use ConcurrentDictionary<TKey, TValue> only when concurrency is required.
