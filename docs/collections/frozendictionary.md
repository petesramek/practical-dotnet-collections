# FrozenDictionary<TKey, TValue>

## What it is

FrozenDictionary<TKey, TValue> is a read-only, precomputed hash-based dictionary optimized for extremely fast lookups.

## Typical use cases
- Hot-path key/value lookups
- Static configuration data
- High-performance read-only datasets

## Sample usage

See:
[samples/frozendictionary-configuration-lookup.cs](../../samples/frozendictionary-configuration-lookup.cs)

### How to run the sample

```bash
dotnet run samples/frozendictionary-configuration-lookup.cs
```

## Internal implementation

Precomputes optimized hashing structures during creation for fast lookup.

### Lookup flow
- Optimized hash lookup (faster than Dictionary)

## Memory characteristics
- Higher upfront cost to build
- Optimized layout for reads
- No mutation allowed

## Complexity overview

- **Key Lookups**: Fast, flat-scaling performance optimized via closed hash analysis.
- **Insertions/Deletions**: Not supported. The structure is completely immutable.

## Benchmark results

### Scenario

Compare lookup performance:
- FrozenDictionary<TKey, TValue>.ContainsKey
- Dictionary<TKey, TValue>.ContainsKey

### How to run this benchmark only

```bash
cd benchmarks
dotnet run -c Release -- --filter *Lookup.FrozenDictionaryBenchmark*
```

### Benchmark code

[benchmarks/Lookup/FrozenDictionaryBenchmark.cs](../../benchmarks/Lookup/FrozenDictionaryBenchmark.cs)

### Results

| Method | N | Mean | Gen 0 | Gen 1 | Gen 2 | Allocated |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **CreateFrozenDictionary** | 10000 | 22,779.721 ns | 37.7197 | - | - | 80,088 B |
| **FrozenDictionaryLookup** | 10000 | 1.287 ns | - | - | - | - |
| **DictionaryLookup** | 10000 | 3.609 ns | - | - | - | - |
| | | | | | | |
| **CreateFrozenDictionary** | 100000 | 288,477.445 ns | 200.1953 | 198.7305 | 198.7305 | 800,161 B |
| **FrozenDictionaryLookup** | 100000 | 1.265 ns | - | - | - | - |
| **DictionaryLookup** | 100000 | 4.435 ns | - | - | - | - |

### Interpretation

- **The Startup Optimization Tax**: Building a `FrozenDictionary` introduces an explicit processing and memory cost upfront. At N = 100,000, calling `.ToFrozenDictionary()` takes **288,477 ns** and allocates **800 KB** of heap memory, triggering multiple rounds of Gen 0, Gen 1, and Gen 2 garbage collection. This happens because .NET analyzes the input dataset to choose and build a custom lookup architecture.
- **The Read-Only Speed Victory**: Once compiled, the investment delivers significant read performance. `FrozenDictionaryLookup` completes in a flat **1.2 ns** across both N = 10,000 and N = 100,000. It runs **nearly 4x faster** than a standard dictionary, which degrades slightly under scaling. By eliminating internal collision loops and tracking overhead, the CPU evaluates lookups almost instantaneously.
- **Flat O(1) Access Scaling**: Because the lookup structure is pre-optimized for the exact keys it holds, performance does not degrade as the collection size scales from 10,000 to 100,000 items.

## Practical optimizations
- Build once, reuse many times
- Use only for read-only scenarios
- **Only create FrozenDictionary instances at initialization boundaries**: Do not create a `FrozenDictionary` inside application hot paths or request-scoped pipelines. The construction overhead will degrade performance. Instead, initialize the collection once during application startup or within a long-lived caching singleton.

## Common mistakes
- Rebuilding frequently (loses benefits)
- **Using FrozenDictionary for volatile, changing datasets**: This collection cannot be mutated. If your workflow requires inserting, updating, or deleting entries during runtime, do not use `FrozenDictionary`.

## When I would choose it
- Extremely frequent key-based lookups
- Data does not change
- When creating a central configuration store, routing table, or system translation cache initialized exactly once at application startup.

## When I would avoid it
- Data changes -> use Dictionary<TKey, TValue>
- Inside short-lived methods or transient request lifecycles where the collection construction overhead out-costs the lookup benefits.

## Rule of thumb

Use FrozenDictionary<TKey, TValue> when data is static and lookup speed is critical.
