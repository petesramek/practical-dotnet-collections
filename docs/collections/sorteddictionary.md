# SortedDictionary<TKey, TValue>

## What it is

SortedDictionary<TKey, TValue> is a tree-based collection that maintains key/value pairs in sorted order by key.

## Typical use cases
- Ordered key/value data
- Range queries
- Iterating keys in sorted order

## Sample usage

See:
[samples/sorteddictionary-triage.cs](../../samples/sorteddictionary-triage.cs)

### How to run the sample

```bash
dotnet run samples/sorteddictionary-triage.cs
```

## Internal implementation

Implemented as a balanced binary search tree (red-black tree).

### Lookup flow
- Tree traversal (O(log n))

## Memory characteristics
- Node-based structure
- Higher overhead than Dictionary
- Poorer cache locality than array-based collections

## Complexity overview

- **Key Lookups**: Logarithmic node branch step evaluations.
- **Insertions/Deletions**: Logarithmic structural rebalancing adjustments.

## Benchmark results

### Scenario

Compare lookup performance:
- SortedDictionary<TKey, TValue>.ContainsKey
- Dictionary<TKey, TValue>.ContainsKey

### How to run this benchmark only

```bash
cd benchmarks
dotnet run -c Release -- --filter *Lookup.SortedDictionaryBenchmark*
```

### Benchmark code

[benchmarks/Lookup/SortedDictionaryBenchmark.cs](../../benchmarks/Lookup/SortedDictionaryBenchmark.cs)

### Results

Environment: BenchmarkDotNet 0.15.8; Windows 11 (10.0.26300.9711); virtual CPU `virt-10.0` at 1.00 GHz (1 CPU; 4 logical / 4 physical cores); .NET SDK 10.0.403; .NET 10.0.14, Arm64 RyuJIT (`armv8.0-a`); 3 warmup and 8 measurement iterations. [Full report](../../BenchmarkDotNet.Artifacts/results/PracticalDotNetCollections.Benchmarks.Lookup.SortedDictionaryBenchmark-report-github.md).

| Method                 | N     | Mean           | Error         | StdDev        | Gen0    | Allocated |
|----------------------- |------ |---------------:|--------------:|--------------:|--------:|----------:|
| **SortedDictionaryAdd**    | **1000**  | **120,496.587 ns** | **6,328.6848 ns** | **3,310.0245 ns** | **22.8271** |   **47920 B** |
| DictionaryAdd          | 1000  |  15,828.885 ns | 1,994.1902 ns | 1,043.0001 ns | 34.4696 |   73168 B |
| SortedDictionaryLookup | 1000  |      46.389 ns |     2.6487 ns |     1.3853 ns |       - |         - |
| DictionaryLookup       | 1000  |       4.201 ns |     0.1180 ns |     0.0617 ns |       - |         - |
| **SortedDictionaryAdd**    | **10000** | **116,342.231 ns** | **2,199.6544 ns** |   **976.6602 ns** | **22.8271** |   **47920 B** |
| DictionaryAdd          | 10000 |  14,198.847 ns | 1,146.7838 ns |   599.7901 ns | 34.4696 |   73168 B |
| SortedDictionaryLookup | 10000 |      92.689 ns |    14.0015 ns |     6.2168 ns |       - |         - |
| DictionaryLookup       | 10000 |       4.825 ns |     0.6361 ns |     0.3327 ns |       - |         - |

### Interpretation

- **The insertion penalty**: at N = 10,000, `SortedDictionaryAdd` averaged **116 us** versus **14.2 us** for `DictionaryAdd` (about 8x by mean). The sorted map maintains ordering as entries are added.
- **Lookup cost**: at N = 10,000, `SortedDictionaryLookup` averaged **92.7 ns** versus **4.83 ns** for `DictionaryLookup`. Use the reported Error and StdDev values when comparing measurements across sizes.
- **The Memory Allocation Profile**: While `SortedDictionary` tracks elements via pointer chains rather than huge contiguous blocks (resulting in smaller up-front allocation metrics in this static slice check), it generates high numbers of short-lived reference objects on large insert streams due to node allocation overhead.

## Practical optimizations
- Use when ordering is required
- Avoid if only lookup speed matters
- **Avoid querying key existence checks back-to-back**: Never write dual-validation lookups (such as checking `.ContainsKey()` followed immediately by an indexer extraction). This forces your application to execute the exact same logarithmic binary branch traversal twice. Use `.TryGetValue()` to process checks and state extractions in a single node path pass.
- **Isolate insertion tracks from real-time lookup blocks**: If your architecture requires loading large blocks of elements initially before running queries, load data into a standard flat `Dictionary<TKey, TValue>` first. Only use a sorted dictionary if real-time, dynamic collection sort state visibility is a critical runtime constraint.

## Common mistakes
- Using SortedDictionary instead of Dictionary for lookup-heavy workloads
- **Using mutable objects as sorting keys**: The internal graph organizes branches based on key traits. Changing a property on an active key object after it has been added corrupts the binary search path, breaking lookups completely.

## When I would choose it
- Need sorted key/value pairs
- Need ordered iteration
- When your system workflow continuously reads, updates, and deletes records while requiring keys to remain sort-ordered at all times.
- When you require fast range scanning capabilities (such as pulling an isolated array block of adjacent keys).

## When I would avoid it
- Need fastest lookup → use Dictionary<TKey, TValue>
- On high-frequency execution tracks where rapid, near-fast lookup execution speed is your primary performance metric. Use standard `Dictionary<TKey, TValue>` or `FrozenDictionary<TKey, TValue>`.
- In workloads that are highly modification-heavy but rarely require sorted sequence access tracking loops.

## Rule of thumb

Use SortedDictionary<TKey, TValue> when ordering matters, otherwise prefer Dictionary<TKey, TValue>.
