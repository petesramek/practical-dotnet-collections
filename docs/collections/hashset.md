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
- **Insertions/Deletions**: Near-instant bucket updates, unless insertions push the structure past its boundary threshold and trigger an internal resize.

## Benchmark results

### Scenario

Compare lookup performance:
- `HashSet<T>.Contains`
- `List<T>.Contains`

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
| **HashSetLookup** | 10000 | 4.100 ns | - | - | - | - |
| **ListLookup** | 10000 | 458.741 ns | - | - | - | - |
| **ScenarioIntersectionListLoop** | 10000 | 1,058,973.331 ns | 1.9531 | - | - | 4,304 B |
| **ScenarioIntersectionHashSetBulk** | 10000 | 246,922.444 ns | 38.0859 | 38.0859 | 38.0859 | 161,821 B |
| | | | | | | |
| **HashSetLookup** | 100000 | 4.036 ns | - | - | - | - |
| **ListLookup** | 100000 | 4,523.688 ns | - | - | - | - |
| **ScenarioIntersectionListLoop** | 100000 | 91,578,888.021 ns | - | - | - | 65,840 B |
| **ScenarioIntersectionHashSetBulk** | 100000 | 3,432,752.178 ns | 328.1250 | 328.1250 | 328.1250 | 1,740,872 B |

### Interpretation

- **The Sublinear Lookup Victory**: As data scales 10x (from N = 10,000 to 100,000), `ListLookup` search performance degrades linearly from 458.7 ns to a brutal **4,523.6 ns**. In contrast, `HashSetLookup` performance scales flatly from 4.1 ns to **4.0 ns**—making it **over 1,120x faster** than a list. Hash tables eliminate sequential scanning by calculating bucket coordinates instantly via the item's hash value.
- **The Intersection Complexity Catastrophe**: The `ScenarioIntersectionListLoop` method demonstrates a classic performance trap. It filters duplicates by running a nested loop that evaluates `_filterList.Contains(item)`. As elements scale to 100,000, execution time shoots up to a massive **91.5 seconds**. This occurs because the list-on-list approach forces an exponential quadratic execution complexity pattern.
- **The Hash Table Mitigation Benefit**: The `ScenarioIntersectionHashSetBulk` method addresses this performance issue directly. By implementing hash table lookups via `.IntersectWith()`, it completes the exact same 100,000 element filtering pass in just **3.4 milliseconds**—making it **over 26,000x faster**. Although cloning the source data into a `HashSet` creates an upfront memory footprint of 1.7 MB, it protects your application from catastrophic performance drops under load.

## Practical optimizations
- Use `HashSet<T>` for membership checks
- **Leverage the Boolean result of Add operations**: Never write slow double-validation checks (such as calling `.Contains()` before calling `.Add()`). The `.Add()` method evaluates existence and processes the insertion simultaneously, returning `true` if the item is unique or `false` if it is a duplicate.
- **Pre-size initial capacity boundaries if target data scales are known**: If you know how many unique items will reside within the set pool, specify that size upfront in the constructor (`new HashSet<T>(capacity)`). This entirely eliminates internal bucket reallocation sweeps during runtime updates.

## Common mistakes
- Using `List<T>` for frequent `Contains`
- Using `HashSet<T>` when ordering is required
- **Using a custom value type or object without overriding equality**: If you insert a custom class or struct into a `HashSet` without implementing `IEquatable<T>` and overriding `GetHashCode()`, the runtime falls back to reference comparisons or slow reflection. This causes silent insertion bugs and pollutes processing loops.

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
