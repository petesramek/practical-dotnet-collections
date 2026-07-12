# SortedList<TKey, TValue>

## What it is

SortedList<TKey, TValue> stores keys and values in two tightly packed arrays that are always kept sorted.

## Typical use cases
- Small, read-heavy sorted datasets
- Configuration or lookup tables loaded once

## Sample usage

See:
[samples/sortedlist-lookup-registry.cs](../../samples/sortedlist-lookup-registry.cs)

### How to run the sample

```bash
dotnet run samples/sortedlist-lookup-registry.cs
```

## Internal implementation

Uses two arrays (keys + values). Inserts require shifting elements to keep order.

### Lookup flow
- Binary search over contiguous key array

## Memory characteristics
- Very compact (no per-node allocations)
- Better cache locality than tree-based collections

## Complexity overview

- **Key Lookups**: Logarithmic binary search array slicing.
- **Unsorted Insertions/Deletions**: Linear array element vector shifting overhead.

## Benchmark results

### Scenario

Compare insertion:
- SortedList<TKey, TValue>
- SortedDictionary<TKey, TValue>

### How to run this benchmark only

```bash
cd benchmarks
dotnet run -c Release -- --filter *Add.SortedListBenchmark*
```

### Benchmark code

[benchmarks/Add/SortedListBenchmark.cs](../../benchmarks/Add/SortedListBenchmark.cs)

### Results

| Method | N | Mean | Gen 0 | Gen 1 | Allocated |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **SortedListRandomAdd** | 1000 | 95,933.94 ns | 7.9346 | - | 16,848 B |
| **SortedDictionaryRandomAdd** | 1000 | 79,349.38 ns | 22.9492 | - | 48,112 B |
| **SortedListLookup** | 1000 | 35.89 ns | - | - | - |
| **SortedDictionaryLookup** | 1000 | 34.54 ns | - | - | - |
| | | | | | |
| **SortedListRandomAdd** | 10000 | **5,230,231.12 ns** | 93.7500 | 23.4375 | **262,800 B** |
| **SortedDictionaryRandomAdd** | 10000 | **1,621,457.84 ns** | 93.7500 | 58.5938 | 480,112 B |
| **SortedListLookup** | 10000 | **53.11 ns** | - | - | - |
| **SortedDictionaryLookup** | 10000 | **63.27 ns** | - | - | - |

### Interpretation

- **The Array Shifting Wall**: For random insertions (`SortedListRandomAdd`), scaling to N = 10,000 exposes a massive performance barrier. The method takes **5.23 milliseconds**, making it **over 3.2x slower** than a sorted dictionary. Because `SortedList` uses primitive internal arrays, inserting non-sequential keys forces the CPU to copy and shift elements down the array block on every single loop pass.
- **The Read-Path Binary Victory**: On lookup execution hot paths (`SortedListLookup`), the array layout demonstrates its structural advantage at scale. At N = 10,000, it processes queries in **53.1 ns** compared to the tree's **63.2 ns**. Bypassing deep tree pointer dereferences allows the processor to isolate data inside flat cache lines using direct binary searches.
- **The Allocation Compaction Profile**: At N = 10,000, `SortedList` utilizes nearly **half the allocation footprint** of a sorted dictionary (~262 KB vs ~480 KB). It packs parameters tightly into flat primitive buffers instead of populating individual heap nodes.

## Practical optimizations
- Use when data is loaded once and rarely modified
- **Always initialize SortedList with a Capacity limit if loading in bulk**: If you are populating the registry from a database row count or fixed config array, specify that size upfront (`new SortedList<K, V>(capacity)`). This prevents dynamic internal array expansions.
- **Populate the collection using pre-sorted data sequences whenever possible**: If keys arrive pre-sorted in ascending numerical sequence, the collection skips internal element shifting entirely, turning O(N) insertion costs into high-speed constant-time appends.

## Common mistakes
- Using for frequently changing data
- **Relying on double validation checks**: Avoid pattern blocks like checking `.ContainsKey()` followed by indexer extractions, which forces the collection to execute its array binary search logic twice. Use `.TryGetValue()` to process the check and extract data in a single path.

## When I would choose it
- Memory efficiency matters
- Mostly read operations
- When implementing fixed metadata lookups, translation registries, or static lookup codes loaded once during application startup and read continuously.

## When I would avoid it
- Frequent inserts/removes -> use SortedDictionary
- On high-frequency execution tracks where rapid, near-instant lookup execution speed is your primary performance metric. Use standard `Dictionary<TKey, TValue>`.

## Rule of thumb

Use SortedList<TKey, TValue> for compact, mostly-read sorted data.
