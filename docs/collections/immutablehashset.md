# ImmutableHashSet<T>

## What it is

ImmutableHashSet<T> is an immutable hash-based set that uses structural sharing to allow efficient updates.

## Typical use cases
- Thread-safe sets without locks
- Functional programming
- Shared read-heavy data

## Sample usage

See:
[samples/immutablehashset-security-claims.cs](../../samples/immutablehashset-security-claims.cs)

### How to run the sample

```bash
dotnet run samples/immutablehashset-security-claims.cs
```

## Internal implementation

Hash-based structure with structural sharing (persistent data structure).

### Lookup flow
- Hash lookup with tree traversals

## Memory characteristics
- Shares structure between versions
- Higher overhead than HashSet<T>
- Avoids full copies

## Complexity overview

- **Contains Operations**: Logarithmic binary tree search steps.
- **Item Mutations**: Logarithmic copy and tree height balance steps.

## Benchmark results

### Scenario

Compare building sets:
- ImmutableHashSet<T>.Add
- HashSet<T>.Add

### How to run this benchmark only

```bash
cd benchmarks
dotnet run -c Release -- --filter *Memory.ImmutableHashSetBenchmark*
```

### Benchmark code

[benchmarks/Memory/ImmutableHashSetBenchmark.cs](../../benchmarks/Memory/ImmutableHashSetBenchmark.cs)

### Results

Environment: BenchmarkDotNet 0.15.8; Windows 11 (10.0.26300.9711); virtual CPU `virt-10.0` at 1.00 GHz (1 CPU; 4 logical / 4 physical cores); .NET SDK 10.0.403; .NET 10.0.14, Arm64 RyuJIT (`armv8.0-a`); 3 warmup and 8 measurement iterations. [Full report](../../BenchmarkDotNet.Artifacts/results/PracticalDotNetCollections.Benchmarks.Memory.ImmutableHashSetBenchmark-report-github.md).

| Method                         | N     | Mean             | Error           | StdDev         | Gen0      | Gen1     | Gen2    | Allocated |
|------------------------------- |------ |-----------------:|----------------:|---------------:|----------:|---------:|--------:|----------:|
| **ImmutableHashSetAdd**            | **1000**  |   **284,688.814 ns** |  **11,290.6322 ns** |  **5,013.1107 ns** |  **290.0391** |        **-** |       **-** |  **606712 B** |
| ImmutableHashSetWithBuilderAdd | 1000  |   150,838.743 ns |     984.8929 ns |    515.1181 ns |   26.6113 |        - |       - |   56048 B |
| HashSetAdd                     | 1000  |    10,979.132 ns |   1,215.8938 ns |    635.9360 ns |   27.7710 |        - |       - |   58664 B |
| ImmutableHashSetContains       | 1000  |       165.913 ns |      15.6126 ns |      6.9321 ns |         - |        - |       - |         - |
| HashSetContains                | 1000  |         5.110 ns |       0.1366 ns |      0.0607 ns |         - |        - |       - |         - |
| **ImmutableHashSetAdd**            | **10000** | **4,395,268.107 ns** | **190,099.5616 ns** | **99,425.7465 ns** | **1890.6250** | **640.6250** |       **-** | **7962552 B** |
| ImmutableHashSetWithBuilderAdd | 10000 | 1,994,269.999 ns |  34,724.7836 ns | 18,161.7333 ns |  117.1875 |  66.4063 |       - |  560048 B |
| HashSetAdd                     | 10000 |   311,415.255 ns |  34,588.2102 ns | 18,090.3027 ns |  132.8125 |  44.9219 | 34.1797 |  538660 B |
| ImmutableHashSetContains       | 10000 |       159.373 ns |      11.5409 ns |      6.0361 ns |         - |        - |       - |         - |
| HashSetContains                | 10000 |         5.172 ns |       0.1383 ns |      0.0723 ns |         - |        - |       - |         - |

### Interpretation

- **The item loop allocation cost**: at N = 10,000, `ImmutableHashSetAdd` averaged **4.40 ms** and allocated **7.96 MB**. The builder averaged **1.99 ms** and allocated **560 KB**, substantially reducing both costs in this run.
- **Lookup cost**: at N = 10,000, `ImmutableHashSetContains` averaged **159.4 ns** versus **5.17 ns** for `HashSetContains`. The immutable lookup is much slower in this microbenchmark; consider whether immutability and safe sharing justify that cost for the workload.

## Practical optimizations
- Prefer over ImmutableArray when uniqueness required
- **Always batch map constructions inside the Stateful Builder**: If you must assemble an `ImmutableHashSet` dynamically using configuration cycles, never invoke direct sequential `.Add()` operations. Always construct the collection via `ImmutableHashSet.CreateBuilder<T>()` to protect your runtime from large heap allocation churn.
- **Prefer FrozenSet for permanent read-only caches**: If your lookup datasets are created once at startup and are completely read-only, do not choose `ImmutableHashSet`. Use `FrozenSet` instead, which delivers direct flat hash lookup speeds rather than slower binary branch traversals.

## Common mistakes
- Using in high mutation scenarios expecting HashSet performance
- **Using single-element collection updates inside frequent runtime tracks**: Calling individual mutation expressions inside an active looping request creates thousands of short-lived tree node pointers, destabilizing memory structures under load.

## When I would choose it
- Need immutability + fast lookup
- When passing historical state snapshots down multi-threaded processing networks where deep structural copying costs are too high, but data safety must be fully guaranteed without locks.

## When I would avoid it
- High mutation rate -> use HashSet<T>
- When lookups occur on extreme, high-frequency execution tracks and raw memory speed is your highest engineering metric. Use `FrozenSet<T>` or standard `HashSet<T>`.

## Rule of thumb

Use ImmutableHashSet<T> when you need immutability with fast lookup.
