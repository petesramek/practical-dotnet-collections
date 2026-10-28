# SortedSet<T>

## What it is

SortedSet<T> is a tree-based collection that maintains unique elements in sorted order.

## Typical use cases
- Maintaining ordered unique data
- Range queries
- Ordered iteration without sorting step

## Sample usage

See:
[samples/sortedset-analytics-tracker.cs](../../samples/sortedset-analytics-tracker.cs)

### How to run the sample

```bash
dotnet run samples/sortedset-analytics-tracker.cs
```

## Internal implementation

SortedSet<T> is implemented as a balanced binary search tree (red-black tree).

### Lookup flow
- Tree traversal (O(log n))

## Memory characteristics
- Node-based structure
- Higher overhead than array/hash-based collections
- Poorer cache locality than List/HashSet

## Complexity overview

- **Containment Checks (`Contains`)**: Logarithmic node branch step evaluations.
- **Insertions/Deletions**: Logarithmic structural rebalancing adjustments.

## Benchmark results

### Scenario

Compare lookup performance:
- SortedSet<T>.Contains
- HashSet<T>.Contains

### How to run this benchmark only

```bash
cd benchmarks
dotnet run -c Release -- --filter *Lookup.SortedSetBenchmark*
```

### Benchmark code

[benchmarks/Lookup/SortedSetBenchmark.cs](../../benchmarks/Lookup/SortedSetBenchmark.cs)

### Results

Environment: BenchmarkDotNet 0.15.8; Windows 11 (10.0.26300.9711); virtual CPU `virt-10.0` at 1.00 GHz (1 CPU; 4 logical / 4 physical cores); .NET SDK 10.0.403; .NET 10.0.14, Arm64 RyuJIT (`armv8.0-a`); 3 warmup and 8 measurement iterations. [Full report](../../BenchmarkDotNet.Artifacts/results/PracticalDotNetCollections.Benchmarks.Lookup.SortedSetBenchmark-report-github.md).

| Method            | N     | Mean          | Error          | StdDev         | Gen0    | Allocated |
|------------------ |------ |--------------:|---------------:|---------------:|--------:|----------:|
| **SortedSetAdd**      | **1000**  | **70,559.745 ns** |  **7,091.4642 ns** |  **3,708.9729 ns** | **19.0430** |   **39888 B** |
| HashSetAdd        | 1000  | 21,627.633 ns |  4,993.2547 ns |  2,611.5688 ns | 27.7710 |   58664 B |
| SortedSetContains | 1000  |     45.231 ns |      0.8191 ns |      0.4284 ns |       - |         - |
| HashSetContains   | 1000  |      4.649 ns |      0.1696 ns |      0.0605 ns |       - |         - |
| **SortedSetAdd**      | **10000** | **66,618.298 ns** | **29,551.4230 ns** | **15,455.9656 ns** | **19.0430** |   **39888 B** |
| HashSetAdd        | 10000 | 12,378.080 ns |    490.7115 ns |    256.6516 ns | 27.7710 |   58664 B |
| SortedSetContains | 10000 |     84.225 ns |      5.1229 ns |      2.6794 ns |       - |         - |
| HashSetContains   | 10000 |      4.790 ns |      0.3367 ns |      0.1761 ns |       - |         - |

### Interpretation

- **The insertion penalty**: at N = 10,000, `SortedSetAdd` averaged **66.6 us** versus **12.4 us** for `HashSetAdd`. The SortedSet Error margin is large relative to its mean, so treat the ratio as approximate.
- **Containment lookup**: at N = 10,000, `SortedSetContains` averaged **84.2 ns** versus **4.79 ns** for `HashSetContains`. The Error margins are much smaller than the gap in this run.
- **The Memory Allocation Profile**: While `SortedSet` tracks elements via pointer chains rather than huge contiguous blocks (resulting in smaller up-front allocation metrics in this static slice check), it generates high numbers of short-lived reference objects on large insert streams due to node allocation overhead.

## Practical optimizations
- Use when ordering is required
- Avoid if only lookup speed matters
- **Leverage range subset views over loops**: If you only need to process an isolated block of data within set thresholds, utilize `.GetViewBetween()` instead of filtering the collection yourself. This returns a targeted structural subtree pointer view immediately without copying elements.
- **Isolate insertion tracks from real-time lookup blocks**: If your architecture requires loading large blocks of elements initially before running queries, load data into a standard flat `HashSet<T>` first. Only use a sorted set if real-time, dynamic collection sort state visibility is a critical runtime constraint.

## Common mistakes
- Using SortedSet instead of HashSet for lookup-heavy workloads
- **Using mutable objects inside sorting rules**: The internal tree organizes branches based on element comparison states. Mutating properties on an element after it has been added corrupts the binary search path, breaking containment lookups completely.

## When I would choose it
- Need sorted unique data
- Need ordered iteration
- When your system workflow continuously reads, updates, and deletes records while requiring elements to remain sort-ordered at all times.
- When you require fast range scanning capabilities (such as pulling an isolated array block of adjacent values).

## When I would avoid it
- Need fastest lookup → use HashSet<T>
- On high-frequency execution tracks where rapid, near-fast lookup execution speed is your primary performance metric. Use standard `HashSet<T>` or `FrozenSet<T>`.
- In workloads that are highly modification-heavy but rarely require sorted sequence access tracking loops.

## Rule of thumb

Use SortedSet<T> when you need ordering, otherwise prefer HashSet<T>.
