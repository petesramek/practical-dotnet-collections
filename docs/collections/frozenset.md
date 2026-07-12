# FrozenSet<T>

## What it is

FrozenSet<T> is a read-only, precomputed hash-based set optimized for extremely fast lookups.

## Typical use cases
- Hot-path lookups
- Static configuration data
- High-performance read-only datasets

## Sample usage

See:
[samples/frozenset-allowlist-validation.cs](../../samples/frozenset-allowlist-validation.cs)

### How to run the sample

```bash
dotnet run samples/frozenset-allowlist-validation.cs
```

## Internal implementation

Precomputes optimized hashing structures during creation for fast lookup.

### Lookup flow
- Optimized hash lookup (faster than HashSet)

## Memory characteristics
- Higher upfront cost to build
- Optimized layout for reads
- No mutation allowed

## Complexity overview

- **Contains Operations**: Fast, flat-scaling performance optimized via closed hash analysis.
- **Insertions/Deletions**: Not supported. The structure is completely immutable.

## Benchmark results

### Scenario

Compare lookup performance:
- FrozenSet<T>.Contains
- HashSet<T>.Contains

### How to run this benchmark only

```bash
cd benchmarks
dotnet run -c Release -- --filter *Lookup.FrozenSetBenchmark*
```

### Benchmark code

[benchmarks/Lookup/FrozenSetBenchmark.cs](../../benchmarks/Lookup/FrozenSetBenchmark.cs)

### Results

| Method | N | Mean | Gen 0 | Gen 1 | Gen 2 | Allocated |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **CreateFrozenSet** | 10000 | 227,307.695 ns | 94.9707 | 94.9707 | 94.9707 | 386,870 B |
| **FrozenSetContains** | 10000 | 1.767 ns | - | - | - | - |
| **HashSetContains** | 10000 | 3.497 ns | - | - | - | - |
| | | | | | | |
| **CreateFrozenSet** | 100000 | 1,354,242.696 ns | 498.0469 | 494.1406 | 494.1406 | 3,960,877 B |
| **FrozenSetContains** | 100000 | 2.695 ns | - | - | - | - |
| **HashSetContains** | 100000 | 4.099 ns | - | - | - | - |

### Interpretation

- **The Startup Optimization Tax**: Building a `FrozenSet` introduces a notable up-front processing and allocation tax. At N = 100,000, calling `.ToFrozenSet()` takes **1,354,242 ns** and allocates nearly **4 MB** of temporary heap memory, triggering significant Gen 0, Gen 1, and Gen 2 garbage collection sweeps. This occurs because the .NET engine scans and maps the uniqueness of the elements to choose the fastest specialized underlying matching logic.
- **The Containment Check Victory**: Once compiled, the performance payoff on read paths is substantial. `FrozenSetContains` evaluates inclusion in just **1.7 ns to 2.6 ns**. It runs **nearly 2x faster** than a standard `HashSet`, which drops to 4.099 ns as the elements increase. By removing traditional hash collision check structures and dynamic capacity safety boundaries, the CPU executes validations almost instantly.
- **Flat Access Scaling**: Because the internal structure is precomputed and tailored for the exact keys it holds, access scaling remains highly optimized even as the set increases 10x from 10,000 to 100,000 elements.

## Practical optimizations
- Build once, reuse many times
- Use only for read-only scenarios
- **Only initialize FrozenSet fields at startup or via Lazy singletons**: Never execute `.ToFrozenSet()` inside a hot loop, middleware track, or a web request pipeline. The heavy creation overhead will destroy application performance. Precompute the set once at startup or cache it inside a static long-lived context.

## Common mistakes
- Rebuilding frequently (loses benefits)
- **Using FrozenSet for dynamic collections**: This structure is completely immutable. If elements need to be added, cleared, or changed at runtime based on application events, do not use `FrozenSet`.

## When I would choose it
- Extremely frequent lookups
- Data does not change
- When creating centralized system routing checks, security token allowlists, or immutable hash rules applied to high-frequency processing workflows.

## When I would avoid it
- Data changes -> use HashSet<T>
- Inside short-lived methods or transient request lifecycles where the collection construction overhead out-costs the lookup benefits.

## Rule of thumb

Use FrozenSet<T> when data is static and lookup speed is critical.
