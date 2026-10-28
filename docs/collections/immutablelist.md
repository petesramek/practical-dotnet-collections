# ImmutableList<T>

## What it is

ImmutableList<T> is an immutable, tree-based list that supports efficient updates through structural sharing.

## Typical use cases
- Functional programming
- Shared read-heavy data
- Thread-safe collections without locks

## Sample usage

See:
[samples/immutablelist-history-snapshot.cs](../../samples/immutablelist-history-snapshot.cs)

### How to run the sample

```bash
dotnet run samples/immutablelist-history-snapshot.cs
```

## Internal implementation

Tree-based structure (AVL tree). Updates reuse most of the structure (structural sharing).

### Lookup flow
- Tree traversal (O(log n))

## Memory characteristics
- Shares structure between versions
- Less allocation than ImmutableArray for updates
- Higher overhead than List<T>

## Complexity overview

- **Index Access Lookups**: Logarithmic binary tree node traversal steps.
- **Item Mutations**: Logarithmic copy and tree height balance steps.

## Benchmark results

### Scenario

Compare building collections:
- ImmutableList<T>.Add
- List<T>.Add

### How to run this benchmark only

```bash
cd benchmarks
dotnet run -c Release -- --filter *Memory.ImmutableListBenchmark*
```

### Benchmark code

[benchmarks/Memory/ImmutableListBenchmark.cs](../../benchmarks/Memory/ImmutableListBenchmark.cs)

### Results

Environment: BenchmarkDotNet 0.15.8; Windows 11 (10.0.26300.9711); virtual CPU `virt-10.0` at 1.00 GHz (1 CPU; 4 logical / 4 physical cores); .NET SDK 10.0.403; .NET 10.0.14, Arm64 RyuJIT (`armv8.0-a`); 3 warmup and 8 measurement iterations. [Full report](../../BenchmarkDotNet.Artifacts/results/PracticalDotNetCollections.Benchmarks.Memory.ImmutableListBenchmark-report-github.md).

| Method                      | N     | Mean             | Error           | StdDev         | Gen0      | Gen1     | Allocated |
|---------------------------- |------ |-----------------:|----------------:|---------------:|----------:|---------:|----------:|
| **ImmutableListAdd**            | **1000**  |   **210,658.097 ns** |   **8,544.0492 ns** |  **3,793.6108 ns** |  **240.4785** |        **-** |  **502896 B** |
| ImmutableListWithBuilderAdd | 1000  |   121,376.604 ns |   8,364.2537 ns |  4,374.6664 ns |   22.9492 |        - |   48024 B |
| ListAdd                     | 1000  |     3,549.350 ns |     350.1170 ns |    183.1180 ns |    4.0207 |        - |    8424 B |
| ImmutableListRead           | 1000  |       140.030 ns |      27.3203 ns |     14.2890 ns |         - |        - |         - |
| ListRead                    | 1000  |         1.108 ns |       0.1031 ns |      0.0539 ns |         - |        - |         - |
| **ImmutableListAdd**            | **10000** | **3,646,441.298 ns** | **240,180.7835 ns** | **85,650.7810 ns** | **1734.3750** | **648.4375** | **6653616 B** |
| ImmutableListWithBuilderAdd | 10000 | 1,611,571.920 ns |  96,405.3003 ns | 50,421.8362 ns |  103.5156 |  62.5000 |  480024 B |
| ListAdd                     | 10000 |    34,370.388 ns |   1,063.0599 ns |    556.0009 ns |   62.4390 |        - |  131400 B |
| ImmutableListRead           | 10000 |       121.154 ns |      29.1173 ns |     15.2289 ns |         - |        - |         - |
| ListRead                    | 10000 |         1.070 ns |       0.0269 ns |      0.0119 ns |         - |        - |         - |

### Interpretation

- **The item-loop allocation cost**: at N = 10,000, `ImmutableListAdd` averaged **3.65 ms** and allocated **6.65 MB**. The builder averaged **1.61 ms** and allocated **480 KB**, substantially reducing both costs in this run.
- **Indexing cost**: at N = 10,000, `ImmutableListRead` averaged **121 ns** versus **1.07 ns** for `ListRead`. This microbenchmark shows a much higher cost for immutable-list indexing; use the table's Error and StdDev columns when comparing measurements.

## Practical optimizations
- Use builder (ImmutableList.CreateBuilder) for bulk operations
- Prefer ImmutableList over ImmutableArray for frequent updates
- **Always handle sequential initialization loops via a Stateful Builder**: If you must map or parse elements into an `ImmutableList` dynamically, never run direct iterative `.Add()` operations. Always construct datasets via `ImmutableList.CreateBuilder<T>()` to preserve processing speeds and stop heavy garbage collection activity.
- **Prefer ImmutableArray for direct lookup tracks**: If your immutable list structure is read heavily by indexers or simple loops, do not use `ImmutableList`. Choose `ImmutableArray<T>` instead, which maps lookups directly to raw hardware offset blocks with zero execution abstraction layers.

## Common mistakes
- Assuming it behaves like List<T>
- **Modifying elements item-by-item inside active runtime pipelines**: Modifying single lines within high-frequency loops forces tree path duplications, triggering performance drops.

## When I would choose it
- Need immutability with reasonable update performance
- Frequent updates with immutability required
- When implementing thread-safe audit logs, undo-redo stacks, or transactional ledgers shared across parallel background workers without thread locking mechanisms.

## When I would avoid it
- Performance-critical hot paths -> use List<T>
- When index-based lookup execution speed on hot paths is your highest application metric. Use `ImmutableArray<T>` or standard array structures.

## Rule of thumb

Use ImmutableList<T> when you need immutability with reasonable update performance.
