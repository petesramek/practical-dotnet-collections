# FrozenSet<T>

## What it is

FrozenSet<T> is a read-only, precomputed hash-based set optimized for very fast lookups.

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

Environment: BenchmarkDotNet 0.15.8; Windows 11 (10.0.26300.9711); virtual CPU `virt-10.0` at 1.00 GHz (1 CPU; 4 logical / 4 physical cores); .NET SDK 10.0.403; .NET 10.0.14, Arm64 RyuJIT (`armv8.0-a`); 3 warmup and 8 measurement iterations. [Full report](../../BenchmarkDotNet.Artifacts/results/PracticalDotNetCollections.Benchmarks.Lookup.FrozenSetBenchmark-report-github.md).

| Method            | N      | Mean             | Error           | StdDev          | Gen0     | Gen1     | Gen2     | Allocated |
|------------------ |------- |-----------------:|----------------:|----------------:|---------:|---------:|---------:|----------:|
| **CreateFrozenSet**   | **10000**  |   **604,234.749 ns** | **305,912.2878 ns** | **159,998.0417 ns** |  **90.8203** |  **90.8203** |  **90.8203** |  **370157 B** |
| FrozenSetContains | 10000  |         2.457 ns |       0.1732 ns |       0.0906 ns |        - |        - |        - |         - |
| HashSetContains   | 10000  |         4.324 ns |       0.1893 ns |       0.0990 ns |        - |        - |        - |         - |
| **CreateFrozenSet**   | **100000** | **4,169,601.510 ns** | **736,993.0344 ns** | **327,229.4750 ns** | **414.0625** | **410.1563** | **410.1563** | **3943393 B** |
| FrozenSetContains | 100000 |         4.444 ns |       1.4209 ns |       0.7431 ns |        - |        - |        - |         - |
| HashSetContains   | 100000 |         5.442 ns |       0.1461 ns |       0.0521 ns |        - |        - |        - |         - |

### Interpretation

- **Construction trade-off**: at N = 100,000, creating a `FrozenSet` took **4,169,602 ns** and allocated **3,943,393 B**. It is intended for collections built once and queried repeatedly; include construction cost when the collection is short-lived.
- **Containment lookup**: at N = 10,000, `FrozenSetContains` averaged **2.457 ns** versus **4.324 ns** for `HashSetContains`. At N = 100,000 the means were closer (4.444 ns and 5.442 ns), and their Error margins overlap, so that row does not establish a reliable advantage.
- **Scaling**: compare matching N rows and their Error and StdDev values; the measurements do not support assuming a fixed speedup at every size.

## Practical optimizations
- Build once, reuse many times
- Use only for read-only scenarios
- **Only initialize FrozenSet fields at startup or via Lazy singletons**: Never execute `.ToFrozenSet()` inside a hot loop, middleware track, or a web request pipeline. The heavy creation overhead will destroy application performance. Precompute the set once at startup or cache it inside a static long-lived context.

## Common mistakes
- Rebuilding frequently (loses benefits)
- **Using FrozenSet for dynamic collections**: This structure is completely immutable. If elements need to be added, cleared, or changed at runtime based on application events, do not use `FrozenSet`.

## When I would choose it
- Very frequent lookups
- Data does not change
- When creating centralized system routing checks, security token allowlists, or immutable hash rules applied to high-frequency processing workflows.

## When I would avoid it
- Data changes -> use HashSet<T>
- Inside short-lived methods or transient request lifecycles where the collection construction overhead out-costs the lookup benefits.

## Rule of thumb

Use FrozenSet<T> when data is static and lookup speed is critical.
