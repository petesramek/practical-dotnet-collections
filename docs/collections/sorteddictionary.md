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

| Method | N | Mean | Gen 0 | Allocated |
| :--- | :--- | :--- | :--- | :--- |
| **SortedDictionaryAdd** | 1000 | 106,391.226 ns | 22.7051 | 47,920 B |
| **DictionaryAdd** | 1000 | 9,668.114 ns | 34.4696 | 73,168 B |
| **SortedDictionaryLookup** | 1000 | 34.193 ns | - | - |
| **DictionaryLookup** | 1000 | 3.467 ns | - | - |
| | | | | |
| **SortedDictionaryAdd** | 10000 | 79,689.567 ns | 22.8271 | 47,920 B |
| **DictionaryAdd** | 10000 | 9,413.240 ns | 34.4696 | 73,168 B |
| **SortedDictionaryLookup** | 10000 | **64.021 ns** | - | - |
| **DictionaryLookup** | 10000 | **3.433 ns** | - | - |

### Interpretation

- **The Insertion Penalty**: For random mutations (`SortedDictionaryAdd`), building the tree layout takes **106,391 ns** compared to the hash table's **9,668 ns** at small scales. This makes the sorted dictionary **over 10x slower** for insertions. The framework is forced to run pointer comparison evaluations and handle complex node rotations continuously on every single addition.
- **The Logarithmic Lookup Cost**: On search execution hot paths (`SortedDictionaryLookup`), performance degrades logarithmically as elements scale, jumping from 34.1 ns to **64.0 ns** as elements grow. It runs **nearly 18x slower** than standard hash tables. While a standard dictionary uses flat hash math calculations to hit bucket memory addresses directly (O(1)), a sorted dictionary must actively navigate node-by-node down pointer chains.
- **The Memory Allocation Profile**: While `SortedDictionary` tracks elements via pointer chains rather than huge contiguous blocks (resulting in smaller up-front allocation metrics in this static slice check), it generates high numbers of short-lived reference objects on massive insert streams due to node allocation overhead.

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
- On high-frequency execution tracks where rapid, near-instant lookup execution speed is your primary performance metric. Use standard `Dictionary<TKey, TValue>` or `FrozenDictionary<TKey, TValue>`.
- In workloads that are highly modification-heavy but rarely require sorted sequence access tracking loops.

## Rule of thumb

Use SortedDictionary<TKey, TValue> when ordering matters, otherwise prefer Dictionary<TKey, TValue>.
