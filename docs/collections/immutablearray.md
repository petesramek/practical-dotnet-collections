# ImmutableArray<T>

## What it is

ImmutableArray<T> is a fixed-size immutable collection. Any modification produces a new instance.

## Typical use cases
- Read-only shared data
- Thread-safe data without locks
- Functional-style programming

## Sample usage

See:
[samples/immutablearray-configuration-snapshot.cs](../../samples/immutablearray-configuration-snapshot.cs)

### How to run the sample

```bash
dotnet run samples/immutablearray-configuration-snapshot.cs
```

## Internal implementation

Backed by an array. Mutations create new arrays (copy-on-write behavior).

### Lookup flow
- Direct index access O(1)

## Memory characteristics
- Each mutation creates a new array
- Higher allocation cost than List<T>
- No internal resizing

## Complexity overview

- **Index Lookups**: Constant-time direct hardware array offset mapping.
- **Item Mutations**: Linear array copy replication, unless processed via an optimized staging container.

## Benchmark results

### Scenario

Compare building collections:
- ImmutableArray<T>.Add
- List<T>.Add

### How to run this benchmark only

```bash
cd benchmarks
dotnet run -c Release -- --filter *Memory.ImmutableArrayBenchmark*
```

### Benchmark code

[benchmarks/Memory/ImmutableArrayBenchmark.cs](../../benchmarks/Memory/ImmutableArrayBenchmark.cs)

### Results

Environment: BenchmarkDotNet 0.15.8; Windows 11 (10.0.26300.9711); virtual CPU `virt-10.0` at 1.00 GHz (1 CPU; 4 logical / 4 physical cores); .NET SDK 10.0.403; .NET 10.0.14, Arm64 RyuJIT (`armv8.0-a`); 3 warmup and 8 measurement iterations. [Full report](../../BenchmarkDotNet.Artifacts/results/PracticalDotNetCollections.Benchmarks.Memory.ImmutableArrayBenchmark-report-github.md).

| Method                       | N     | Mean               | Error             | StdDev          | Gen0       | Allocated   |
|----------------------------- |------ |-------------------:|------------------:|----------------:|-----------:|------------:|
| **ImmutableArrayAdd**            | **1000**  |    **180,367.8962 ns** |    **32,525.6728 ns** |  **11,598.9682 ns** |   **968.9941** |   **2028000 B** |
| ImmutableArrayWithBuilderAdd | 1000  |      4,052.0361 ns |       380.1993 ns |     198.8516 ns |     5.9128 |     12376 B |
| ListAdd                      | 1000  |      3,718.8740 ns |     1,093.2092 ns |     485.3916 ns |     4.0207 |      8424 B |
| ImmutableArrayRead           | 1000  |          0.8288 ns |         0.0287 ns |       0.0102 ns |          - |           - |
| ListRead                     | 1000  |          1.0646 ns |         0.3124 ns |       0.1387 ns |          - |           - |
| **ImmutableArrayAdd**            | **10000** | **17,142,003.9992 ns** | **1,652,866.3749 ns** | **733,882.9147 ns** | **95000.0000** | **200280000 B** |
| ImmutableArrayWithBuilderAdd | 10000 |     41,501.7023 ns |     3,681.4864 ns |   1,925.4886 ns |    81.0547 |    171352 B |
| ListAdd                      | 10000 |     40,664.1170 ns |     4,870.9400 ns |   2,162.7275 ns |    62.4390 |    131400 B |
| ImmutableArrayRead           | 10000 |          0.8722 ns |         0.1292 ns |       0.0573 ns |          - |           - |
| ListRead                     | 10000 |          0.8254 ns |         0.1120 ns |       0.0497 ns |          - |           - |

### Interpretation

- **The item-by-item loop cost**: adding 10,000 items individually with `ImmutableArrayAdd` averaged **17.1 ms** and allocated **200.3 MB**, because each update copies the array.
- **The builder optimization**: `ImmutableArrayWithBuilderAdd` averaged **41.5 us** and allocated **171 KB** for the same build. Its mean was close to `ListAdd` (40.7 us, 131 KB) in this run, so the results do not show a speed advantage over the list.
- **Indexing**: at N = 10,000, `ImmutableArrayRead` and `ListRead` had similar means. Compare their Error margins in the table rather than relying on a small point-estimate difference.

## Practical optimizations
- Use builder (ImmutableArray.CreateBuilder) for bulk operations
- **Preallocate your Builder capacity bounds if scale limits are known**: Just like a standard list or array track, initializing your staging container with an explicit starting boundary size (`ImmutableArray.CreateBuilder<T>(capacity)`) prevents internal resizing array allocations during construction.

## Common mistakes
- Using for frequently changing data
- **Attempting to define a Null default value inside data fields**: As a non-nullable value-type structure, initializing fields using reference assignment styles triggers compiler issues. Always rely on `ImmutableArray<T>.Empty` to define clean, default empty collection spaces.

## When I would choose it
- Data is mostly read-only
- Thread safety required without locks
- When broadcasting frozen system configuration maps, routing rules, or validation tables across dozens of background threads without thread locks.

## When I would avoid it
- Frequent mutations -> use List<T>
- Inside short-lived algorithms where datasets are modified immediately before being immediately discarded.

## Rule of thumb

Use ImmutableArray<T> for read-heavy, write-rare scenarios.
