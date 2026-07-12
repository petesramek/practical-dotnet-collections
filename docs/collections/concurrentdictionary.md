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

- **Read Access**: Near-instant and entirely lock-free across all thread tracks.
- **Write/Update Access**: Near-instant bucket tracking. It incurs minor thread synchronization delays only if two separate threads try to mutate keys that hash into the exact same localized bucket partition.

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

| Method | N | Mean | Gen 0 | Gen 1 | Gen 2 | Allocated |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **ConcurrentDictionaryAdd** | 10000 | 2,643.87 us | 234.3750 | 109.3750 | 39.0625 | 1301.11 KB |
| **DictionaryAddWithLock** | 10000 | 711.74 us | 224.6094 | 91.7969 | 86.9141 | 666.62 KB |
| **GetOrAddWithAllocatingClosure** | 10000 | 170.22 us | 425.0488 | - | - | 861.8 KB |
| **GetOrAddWithStatefulAllocationFree**| 10000 | 23.17 us | 1.1292 | - | - | 2.28 KB |
| | | | | | | |
| **ConcurrentDictionaryAdd** | 50000 | 12,140.33 us | 1062.5000 | 734.3750 | 250.0000 | 5808.46 KB |
| **DictionaryAddWithLock** | 50000 | 3,267.98 us | 636.7188 | 425.7813 | 425.7813 | 2901.61 KB |
| **GetOrAddWithAllocatingClosure** | 50000 | 765.98 us | 2113.2813 | - | - | 4299.43 KB |
| **GetOrAddWithStatefulAllocationFree**| 50000 | 88.91 us | 1.0986 | - | - | 2.3 KB |

### Interpretation

The metrics expose two vital architectural realities regarding shared map tracking:

1. **The Insertion Contention Paradox:** For pure, frantic insertion tasks (`Add`), a standard dictionary behind a global `lock` runs roughly 3.7x faster and uses half the allocation space (~3.2 ms vs ~12.1 ms at N = 50,000). This occurs because initializing a brand new `ConcurrentDictionary` requires allocating multiple localized synchronization primitives. Under massive concurrent write pressure from empty states, the thread synchronization overhead of balancing bucket regions out-costs a fast global loop monitor lock.
2. **The Lambda Closure Extinement:** The true optimization focus of `ConcurrentDictionary` is exposed by the `.GetOrAdd()` tracks. In `GetOrAddWithAllocatingClosure`, passing variables from an outer scope inside the lambda factory forces the compiler to allocate hidden closure objects on the heap on every single loop pass. This creates a severe footprint penalty (4.3 MB at N = 50,000) and drags execution speed down to 765.98 us. 
3. **The State Overload Victory:** Switching to the stateful overload variant (`GetOrAddWithStatefulAllocationFree`) strips the allocation footprint down to an astonishing 2.3 KB and speeds up evaluation to 88.91 us—making it nearly 9x faster. Passing context parameters directly through the state parameter allows the compiler to cache and reuse a single static function pointer handle, preventing heap pollution completely.

## Practical optimizations
- **Always use the stateful overloads for factory lambdas**: When using `.GetOrAdd()` or `.AddOrUpdate()`, pass external scope variables into the state parameter. This keeps your callback delegates completely allocation-free and maximizes performance.
- **Preallocate the concurrency level and capacity defaults if known**: If you know how many unique threads will actively bombard the collection alongside the total expected map count, pass both variables to the constructor (`new ConcurrentDictionary<K, V>(concurrencyLevel, expectedCount)`) to lock down bucket structures immediately.

## Common mistakes
- **Using in single-threaded code**: Instantiating a `ConcurrentDictionary` when only one thread ever executes modifications introduces completely unnecessary lock-stripe setup overhead and pointer safety layers. Use a standard `Dictionary<TKey, TValue>` instead.
- **Assuming factory delegates run exactly once per key evaluation**: The factory lambda inside `.GetOrAdd()` is not execution-atomic. If two separate threads call `.GetOrAdd()` on a missing key at the exact same moment, both factory lambdas may execute simultaneously in a race, with one result thrown away. Ensure factory delegates remain free of operational side effects.

## When I would choose it
- Multiple threads access shared dictionary
- When creating persistent, application-wide configuration caches or session tracking pools read and updated continuously across parallel worker loops.

## When I would avoid it
- Single-threaded -> use Dictionary
- When a collection is built and updated by exactly one single thread before being exposed as a read-only snapshot to other threads. Use a standard `Dictionary<TKey, TValue>` or a `FrozenDictionary<TKey, TValue>`.

## Rule of thumb

Use ConcurrentDictionary<TKey, TValue> only when concurrency is required.
