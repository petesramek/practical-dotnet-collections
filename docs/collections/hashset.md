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

| Method | N | Mean | Gen 0 | Gen 1 | Gen 2 | Allocated |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **HashSetLookup** | 10000 | 5.522 ns | - | - | - | - |
| **ListLookup** | 10000 | 827.851 ns | - | - | - | - |
| **ScenarioIntersectionListLoop** | 10000 | 1,791,909 ns | 1.9531 | - | - | 4,304 B |
| **ScenarioIntersectionHashSetFilter** | 10000 | 65,229 ns | 2.0142 | - | - | 4,304 B |
| **ScenarioIntersectionHashSetBulk** | 10000 | 635,030 ns | 38.0859 | 38.0859 | 38.0859 | 161,821 B |
| | | | | | | |
| **HashSetLookup** | 100000 | 7.304 ns | - | - | - | - |
| **ListLookup** | 100000 | 8,217.004 ns | - | - | - | - |
| **ScenarioIntersectionListLoop** | 100000 | 247,090,719 ns | - | - | - | 65,840 B |
| **ScenarioIntersectionHashSetFilter** | 100000 | 2,666,999 ns | 27.3438 | - | - | 65,840 B |
| **ScenarioIntersectionHashSetBulk** | 100000 | 7,758,623 ns | 312.5000 | 312.5000 | 312.5000 | 1,740,428 B |

Numbers come from a short BenchmarkDotNet job (3 warmup, 8 iterations) on an Arm64 machine with .NET 10; compare ratios rather than absolute values.

### Interpretation

- **Lookup scaling**: when N grows 10x (10,000 to 100,000), `ListLookup` grows about 10x (828 ns to 8,217 ns), as expected for a linear scan, while `HashSetLookup` stays in the same order of magnitude (5.5 ns to 7.3 ns) because it hashes the item instead of scanning.
- **Intersection with a list loop**: `ScenarioIntersectionListLoop` calls `_filterList.Contains(item)` for every source item, which is quadratic. At N = 100,000 it took about 247 ms.
- **Intersection with a `HashSet` filter**: `ScenarioIntersectionHashSetFilter` does the same work (same loop, same output including duplicates) but checks membership in a `HashSet`. It took about 2.7 ms at N = 100,000, roughly 90x faster than the list loop, with identical allocation. This is the like-for-like comparison.
- **Intersection with `IntersectWith`**: `ScenarioIntersectionHashSetBulk` copies the source into a `HashSet` and calls `IntersectWith`. It took about 7.8 ms and allocated about 1.7 MB at N = 100,000. It is not the same work as the loop: the result is de-duplicated, and the copy costs memory. Use it when you want set semantics, and the filter variant when you want to keep duplicates.

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
