# HashSet<T>

## What it is

`HashSet<T>` is a hash-based collection that stores unique elements and provides fast existence checks.

## Typical use cases
- Deduplication
- Membership checks
- Set operations

## Sample usage

See:
[samples/hashset-deduplication.cs](../../samples/hashset-deduplication.cs)

### How to run the sample

```bash
dotnet run samples/hashset-deduplication.cs
```

Notes:
- Requires .NET 10 or later
- Runs as a standalone script

## Internal implementation

`HashSet<T>` is implemented using:
- buckets array
- entries array

### Lookup flow
- Compute hash code
- Map to bucket
- Traverse collision chain
- Compare keys

## Memory characteristics
- Similar overhead to `Dictionary<TKey, TValue>`
- Stores buckets and entries
- Resizes cause rehashing

## Complexity overview

- **Containment Lookups**: Flat scaling performance driven by direct hash mapping mathematics.
- **Insertions/Deletions**: Fast bucket updates, unless insertions push the structure past its boundary threshold and trigger an internal resize.

## Benchmark results

### Scenario

Compare lookup performance:
- `HashSet<T>.Contains`
- `List<T>.Contains`
- Intersecting two large collections (list loop vs `HashSet` filter vs `IntersectWith`)
### How to run this benchmark only

```bash
cd benchmarks
dotnet run -c Release -- --filter *Lookup.HashSetBenchmark*
```

### Benchmark code

[benchmarks/Lookup/HashSetBenchmark.cs](../../benchmarks/Lookup/HashSetBenchmark.cs)

### Results

Environment: BenchmarkDotNet 0.15.8; Windows 11 (10.0.26300.9711); virtual CPU `virt-10.0` at 1.00 GHz (1 CPU; 4 logical / 4 physical cores); .NET SDK 10.0.403; .NET 10.0.14, Arm64 RyuJIT (`armv8.0-a`); 3 warmup and 8 measurement iterations. [Full report](../../BenchmarkDotNet.Artifacts/results/PracticalDotNetCollections.Benchmarks.Lookup.HashSetBenchmark-report-github.md).

| Method                            | N      | Mean               | Error              | StdDev             | Gen0     | Gen1     | Gen2     | Allocated |
|---------------------------------- |------- |-------------------:|-------------------:|-------------------:|---------:|---------:|---------:|----------:|
| **HashSetLookup**                     | **10000**  |           **4.209 ns** |          **0.1211 ns** |          **0.0538 ns** |        **-** |        **-** |        **-** |         **-** |
| ListLookup                        | 10000  |         611.392 ns |         47.1768 ns |         20.9468 ns |        - |        - |        - |         - |
| ScenarioIntersectionListLoop      | 10000  |   1,347,413.076 ns |     44,182.9656 ns |     23,108.5453 ns |   1.9531 |        - |        - |    4304 B |
| ScenarioIntersectionHashSetFilter | 10000  |      90,045.310 ns |      3,478.8642 ns |      1,819.5132 ns |   1.9531 |        - |        - |    4304 B |
| ScenarioIntersectionHashSetBulk   | 10000  |     564,776.067 ns |     63,781.2607 ns |     33,358.8327 ns |  38.0859 |  38.0859 |  38.0859 |  161821 B |
| **HashSetLookup**                     | **100000** |           **6.931 ns** |          **3.0615 ns** |          **1.6012 ns** |        **-** |        **-** |        **-** |         **-** |
| ListLookup                        | 100000 |       6,837.801 ns |      2,298.2420 ns |      1,202.0250 ns |        - |        - |        - |         - |
| ScenarioIntersectionListLoop      | 100000 | 177,395,053.819 ns | 79,952,184.7570 ns | 41,816,538.5989 ns |        - |        - |        - |   65856 B |
| ScenarioIntersectionHashSetFilter | 100000 |   1,654,388.184 ns |     38,271.3076 ns |     20,016.6339 ns |  29.2969 |        - |        - |   65840 B |
| ScenarioIntersectionHashSetBulk   | 100000 |   5,589,867.350 ns |    349,875.8964 ns |    182,991.8591 ns | 312.5000 | 312.5000 | 312.5000 | 1740547 B |

### Interpretation

- **Lookup scaling**: `ListLookup` scans the collection, so its work grows with N; `HashSetLookup` hashes the item instead. At N = 100,000, their means were **6,837.801 ns** and **6.931 ns** respectively. The Error and StdDev columns show the variability, especially for the list scan.
- **Intersection with a list loop**: `ScenarioIntersectionListLoop` calls `_filterList.Contains(item)` for every source item, which is quadratic. At N = 100,000 it averaged **177 ms**.
- **Intersection with a `HashSet` filter**: `ScenarioIntersectionHashSetFilter` does the same loop and retains duplicates, but checks membership in a `HashSet`. It averaged **1.65 ms** at N = 100,000, with about the same allocation as the list loop in this run. This is the like-for-like comparison.
- **Intersection with `IntersectWith`**: `ScenarioIntersectionHashSetBulk` copies the source into a `HashSet` and calls `IntersectWith`. It averaged **5.59 ms** and allocated **1.74 MB** at N = 100,000. It is not the same work as the loop: the result is de-duplicated, and the copy costs memory. Use it when you want set semantics, and the filter variant when you want to keep duplicates.

## Practical optimizations
- Use `HashSet<T>` for membership checks
- **Leverage the Boolean result of Add operations**: Never write slow double-validation checks (such as calling `.Contains()` before calling `.Add()`). The `.Add()` method evaluates existence and processes the insertion simultaneously, returning `true` if the item is unique or `false` if it is a duplicate.
- **Pre-size initial capacity boundaries if target data scales are known**: If you know how many unique items will reside within the set pool, specify that size upfront in the constructor (`new HashSet<T>(capacity)`). This entirely eliminates internal bucket reallocation sweeps during runtime updates.

## Common mistakes
- Using `List<T>` for frequent `Contains`
- Using `HashSet<T>` when ordering is required
- **Using a custom value type or object without overriding equality**: If you insert a custom class or struct into a `HashSet` without implementing `IEquatable<T>` and overriding `GetHashCode()`, the runtime falls back to reference equality (for classes), which can silently treat equal-looking items as distinct, or to the default boxing-based comparer (for structs), which is slower and allocates.

## When I would choose it
- Need uniqueness
- Frequent existence checks
- Deduplication
- When performing heavy collection intersections, unions, or inclusion validation checks across scaling business records.

## When I would avoid it
- Need ordering
- Sequential access only
- When your collection size is guaranteed to stay small, and minimizing memory overhead is your primary concern.

## Rule of thumb

Use `HashSet<T>` when you need fast existence checks and uniqueness.
