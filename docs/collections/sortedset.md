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

| Method | N | Mean | Gen 0 | Allocated |
| :--- | :--- | :--- | :--- | :--- |
| **SortedSetAdd** | 1000 | 43,151.556 ns | 19.0430 | 39,888 B |
| **HashSetAdd** | 1000 | 8,355.961 ns | 27.7710 | 58,664 B |
| **SortedSetContains** | 1000 | 36.677 ns | - | - |
| **HashSetContains** | 1000 | 3.562 ns | - | - |
| | | | | |
| **SortedSetAdd** | 10000 | 36,086.383 ns | 19.0430 | 39,888 B |
| **HashSetAdd** | 10000 | 8,482.595 ns | 27.7710 | 58,664 B |
| **SortedSetContains** | 10000 | **60.140 ns** | - | - |
| **HashSetContains** | 10000 | **3.343 ns** | - | - |

### Interpretation

- **The Insertion Penalty**: For random mutations (`SortedSetAdd`), building the tree layout takes **43,151 ns** compared to the hash pool's **8,355 ns** at small scales. This makes the sorted set **over 5x slower** for insertions. The framework is forced to run pointer comparison evaluations and handle complex node rotations continuously on every single addition.
- **The Logarithmic Search Cost**: On search containment hot paths (`SortedSetContains`), performance degrades logarithmically as elements scale, jumping from 36.6 ns to **60.1 ns** as elements grow. It runs **nearly 18x slower** than standard hash pools. While a standard hash set uses flat hash math calculations to hit bucket memory addresses directly (O(1)), a sorted set must actively navigate node-by-node down pointer chains.
- **The Memory Allocation Profile**: While `SortedSet` tracks elements via pointer chains rather than huge contiguous blocks (resulting in smaller up-front allocation metrics in this static slice check), it generates high numbers of short-lived reference objects on massive insert streams due to node allocation overhead.

## Practical optimizations
- Use when ordering is required
- Avoid if only lookup speed matters
- **Leverage range subset views over loops**: If you only need to process an isolated block of data within set thresholds, utilize `.GetViewBetween()` instead of filtering the collection yourself. This returns a targeted structural subtree pointer view instantly without copying elements.
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
- On high-frequency execution tracks where rapid, near-instant lookup execution speed is your primary performance metric. Use standard `HashSet<T>` or `FrozenSet<T>`.
- In workloads that are highly modification-heavy but rarely require sorted sequence access tracking loops.

## Rule of thumb

Use SortedSet<T> when you need ordering, otherwise prefer HashSet<T>.
