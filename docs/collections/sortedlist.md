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

Environment: BenchmarkDotNet 0.15.8; Windows 11 (10.0.26300.9711); virtual CPU `virt-10.0` at 1.00 GHz (1 CPU; 4 logical / 4 physical cores); .NET SDK 10.0.403; .NET 10.0.14, Arm64 RyuJIT (`armv8.0-a`); 3 warmup and 8 measurement iterations. [Full report](../../BenchmarkDotNet.Artifacts/results/PracticalDotNetCollections.Benchmarks.Add.SortedListBenchmark-report-github.md).

| Method                    | N     | Mean            | Error            | StdDev         | Gen0    | Gen1    | Allocated |
|-------------------------- |------ |----------------:|-----------------:|---------------:|--------:|--------:|----------:|
| **SortedListRandomAdd**       | **1000**  |   **130,762.35 ns** |    **24,016.882 ns** |  **12,561.294 ns** |  **7.8125** |       **-** |   **16848 B** |
| SortedDictionaryRandomAdd | 1000  |   126,424.63 ns |    10,800.755 ns |   4,795.602 ns | 22.9492 |       - |   48112 B |
| SortedListLookup          | 1000  |        77.22 ns |        17.405 ns |       9.103 ns |       - |       - |         - |
| SortedDictionaryLookup    | 1000  |        57.62 ns |         7.425 ns |       3.883 ns |       - |       - |         - |
| **SortedListRandomAdd**       | **10000** | **9,849,729.38 ns** | **1,492,302.591 ns** | **662,591.599 ns** | **93.7500** | **31.2500** |  **262800 B** |
| SortedDictionaryRandomAdd | 10000 | 2,584,087.85 ns |   623,939.769 ns | 326,332.564 ns | 93.7500 | 62.5000 |  480112 B |
| SortedListLookup          | 10000 |       102.12 ns |        16.396 ns |       8.575 ns |       - |       - |         - |
| SortedDictionaryLookup    | 10000 |       104.54 ns |        40.959 ns |      21.422 ns |       - |       - |         - |

### Interpretation

- **The array shifting cost**: at N = 10,000, `SortedListRandomAdd` averaged **9.85 ms** versus **2.58 ms** for `SortedDictionaryRandomAdd`. The Error margins are wide, but the measured means favor the sorted dictionary for this random-insertion workload.
- **Lookup cost**: at N = 10,000, `SortedListLookup` and `SortedDictionaryLookup` averaged **102 ns** and **105 ns**. Their Error margins overlap, so this run does not establish a lookup advantage for either collection.
- **Allocation**: at N = 10,000, `SortedListRandomAdd` allocated **263 KB** versus **480 KB** for `SortedDictionaryRandomAdd`.

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
- On high-frequency execution tracks where rapid, near-fast lookup execution speed is your primary performance metric. Use standard `Dictionary<TKey, TValue>`.

## Rule of thumb

Use SortedList<TKey, TValue> for compact, mostly-read sorted data.
