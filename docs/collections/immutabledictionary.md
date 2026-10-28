# ImmutableDictionary<TKey, TValue>

## What it is

ImmutableDictionary<TKey, TValue> is an immutable, hash-based key/value collection that uses structural sharing to support efficient updates.

## Typical use cases
- Thread-safe shared state
- Functional programming
- Read-heavy data with occasional updates

## Sample usage

See:
[samples/immutabledictionary-routing-table.cs](../../samples/immutabledictionary-routing-table.cs)

### How to run the sample

```bash
dotnet run samples/immutabledictionary-routing-table.cs
```

## Internal implementation

Hash-based persistent data structure with structural sharing.

### Lookup flow
- Hash lookup with tree traversals

## Memory characteristics
- Shares structure between versions
- Higher overhead than Dictionary
- Avoids full copies on updates

## Complexity overview

- **Key Lookups**: Logarithmic binary tree search steps.
- **Item Mutations**: Logarithmic copy and tree height balance steps.

## Benchmark results

### Scenario

Compare building dictionaries:
- ImmutableDictionary<TKey, TValue>.Add
- Dictionary<TKey, TValue>.Add

### How to run this benchmark only

```bash
cd benchmarks
dotnet run -c Release -- --filter *Memory.ImmutableDictionaryBenchmark*
```

### Benchmark code

[benchmarks/Memory/ImmutableDictionaryBenchmark.cs](../../benchmarks/Memory/ImmutableDictionaryBenchmark.cs)

### Results

Environment: BenchmarkDotNet 0.15.8; Windows 11 (10.0.26300.9711); virtual CPU `virt-10.0` at 1.00 GHz (1 CPU; 4 logical / 4 physical cores); .NET SDK 10.0.403; .NET 10.0.14, Arm64 RyuJIT (`armv8.0-a`); 3 warmup and 8 measurement iterations. [Full report](../../BenchmarkDotNet.Artifacts/results/PracticalDotNetCollections.Benchmarks.Memory.ImmutableDictionaryBenchmark-report-github.md).

| Method                            | N     | Mean             | Error           | StdDev          | Gen0      | Gen1     | Gen2    | Allocated |
|---------------------------------- |------ |-----------------:|----------------:|----------------:|----------:|---------:|--------:|----------:|
| **ImmutableDictionaryAdd**            | **1000**  |   **296,422.238 ns** |   **9,544.4261 ns** |   **4,237.7843 ns** |  **286.1328** |        **-** |       **-** |  **598712 B** |
| ImmutableDictionaryWithBuilderAdd | 1000  |   156,239.530 ns |   1,215.5818 ns |     635.7728 ns |   26.6113 |        - |       - |   56040 B |
| DictionaryAdd                     | 1000  |    11,824.197 ns |     947.1825 ns |     420.5549 ns |   34.4696 |        - |       - |   73168 B |
| ImmutableDictionaryLookup         | 1000  |       128.098 ns |       9.2092 ns |       4.8166 ns |         - |        - |       - |         - |
| DictionaryLookup                  | 1000  |         5.419 ns |       0.4656 ns |       0.2435 ns |         - |        - |       - |         - |
| **ImmutableDictionaryAdd**            | **10000** | **4,897,166.260 ns** | **875,293.4180 ns** | **457,795.3824 ns** | **1851.5625** | **664.0625** |       **-** | **7882552 B** |
| ImmutableDictionaryWithBuilderAdd | 10000 | 2,281,617.900 ns | 305,571.0190 ns | 159,819.5515 ns |  117.1875 |  66.4063 |       - |  560040 B |
| DictionaryAdd                     | 10000 |   392,942.991 ns |  37,012.3263 ns |  19,358.1623 ns |  149.9023 |  53.7109 | 41.0156 |  673218 B |
| ImmutableDictionaryLookup         | 10000 |       110.865 ns |      20.0456 ns |       8.9004 ns |         - |        - |       - |         - |
| DictionaryLookup                  | 10000 |         5.149 ns |       0.0254 ns |       0.0091 ns |         - |        - |       - |         - |

### Interpretation

- **The item loop allocation cost**: at N = 10,000, `ImmutableDictionaryAdd` averaged **4.90 ms** and allocated **7.88 MB**. The builder averaged **2.28 ms** and allocated **560 KB**, substantially reducing both costs in this run.
- **Lookup cost**: at N = 10,000, `ImmutableDictionaryLookup` averaged **110.9 ns** versus **5.15 ns** for `DictionaryLookup`. The immutable lookup is much slower in this microbenchmark; consider whether immutability and safe sharing justify that cost for the workload.

## Practical optimizations
- Use builders (ToBuilder) for bulk updates
- **Always batch map constructions inside the Stateful Builder**: If you must assemble an `ImmutableDictionary` dynamically using configuration cycles, never invoke direct sequential `.Add()` operations. Always construct the collection via `ImmutableDictionary.CreateBuilder<K, V>()` to protect your runtime from large heap allocation churn.
- **Prefer FrozenDictionary for permanent read-only caches**: If your lookup datasets are created once at startup and are completely read-only, do not choose `ImmutableDictionary`. Use `FrozenDictionary` instead, which delivers direct flat hash lookup speeds rather than slower binary branch traversals.

## Common mistakes
- Using in high mutation scenarios expecting Dictionary performance
- **Using single-element collection updates inside frequent runtime tracks**: Calling individual mutation expressions inside an active looping request creates thousands of short-lived tree node pointers, destabilizing memory structures under load.

## When I would choose it
- Need immutability + key/value mapping
- When passing historical state snapshots down multi-threaded processing networks where deep structural copying costs are too high, but data safety must be fully guaranteed without locks.

## When I would avoid it
- Frequent updates -> use Dictionary<TKey, TValue>
- When lookups occur on extreme, high-frequency execution tracks and raw memory speed is your highest engineering metric. Use `FrozenDictionary<TKey, TValue>` or standard `Dictionary<TKey, TValue>`.

## Rule of thumb

Use ImmutableDictionary<TKey, TValue> when you need immutability with efficient lookup.
