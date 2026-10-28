# FrozenDictionary<TKey, TValue>

## What it is

FrozenDictionary<TKey, TValue> is a read-only, precomputed hash-based dictionary optimized for very fast lookups.

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

Environment: BenchmarkDotNet 0.15.8; Windows 11 (10.0.26300.9711); virtual CPU `virt-10.0` at 1.00 GHz (1 CPU; 4 logical / 4 physical cores); .NET SDK 10.0.403; .NET 10.0.14, Arm64 RyuJIT (`armv8.0-a`); 3 warmup and 8 measurement iterations. [Full report](../../BenchmarkDotNet.Artifacts/results/PracticalDotNetCollections.Benchmarks.Lookup.FrozenDictionaryBenchmark-report-github.md).

| Method                 | N      | Mean           | Error           | StdDev         | Gen0     | Gen1     | Gen2     | Allocated |
|----------------------- |------- |---------------:|----------------:|---------------:|---------:|---------:|---------:|----------:|
| **CreateFrozenDictionary** | **10000**  |  **31,423.650 ns** |   **3,224.6842 ns** |  **1,431.7798 ns** |  **37.7197** |        **-** |        **-** |   **80088 B** |
| FrozenDictionaryLookup | 10000  |       1.489 ns |       0.1038 ns |      0.0543 ns |        - |        - |        - |         - |
| DictionaryLookup       | 10000  |       5.136 ns |       1.2699 ns |      0.6642 ns |        - |        - |        - |         - |
| **CreateFrozenDictionary** | **100000** | **573,612.289 ns** | **110,460.0047 ns** | **57,772.7183 ns** | **191.4063** | **190.4297** | **190.4297** |  **800572 B** |
| FrozenDictionaryLookup | 100000 |       1.442 ns |       0.0511 ns |      0.0267 ns |        - |        - |        - |         - |
| DictionaryLookup       | 100000 |       5.795 ns |       0.5273 ns |      0.1881 ns |        - |        - |        - |         - |

### Interpretation

- **The startup optimization tax**: at N = 100,000, creating a `FrozenDictionary` takes **573,612 ns** and allocates **800,572 B**. This is a one-time cost; the lookup advantage matters most when a collection is built once and queried repeatedly.
- **Lookup speed**: at N = 100,000, `FrozenDictionaryLookup` averaged **1.442 ns** versus **5.795 ns** for `DictionaryLookup`. Compare the Error columns when considering smaller differences.
- **Scaling**: the FrozenDictionary lookup means were similar at N = 10,000 and N = 100,000 in this run. The report's Error and StdDev columns provide the variability for each measurement.

## Practical optimizations
- Build once, reuse many times
- Use only for read-only scenarios
- **Only create FrozenDictionary instances at initialization boundaries**: Do not create a `FrozenDictionary` inside application hot paths or request-scoped pipelines. The construction overhead will degrade performance. Instead, initialize the collection once during application startup or within a long-lived caching singleton.

## Common mistakes
- Rebuilding frequently (loses benefits)
- **Using FrozenDictionary for volatile, changing datasets**: This collection cannot be mutated. If your workflow requires inserting, updating, or deleting entries during runtime, do not use `FrozenDictionary`.

## When I would choose it
- Very frequent key-based lookups
- Data does not change
- When creating a central configuration store, routing table, or system translation cache initialized exactly once at application startup.

## When I would avoid it
- Data changes -> use Dictionary<TKey, TValue>
- Inside short-lived methods or transient request lifecycles where the collection construction overhead out-costs the lookup benefits.

## Rule of thumb

Use FrozenDictionary<TKey, TValue> when data is static and lookup speed is critical.
