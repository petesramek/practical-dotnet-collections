# ReadOnlyCollection<T>

## What it is

ReadOnlyCollection<T> is a wrapper around an existing collection that prevents modification through its interface.

## Typical use cases
- Exposing data safely from APIs
- Preventing accidental modification

## Sample usage

See:
[samples/readonlycollection-data-shielding.cs](../../samples/readonlycollection-data-shielding.cs)

### How to run the sample

```bash
dotnet run samples/readonlycollection-data-shielding.cs
```

## Internal implementation

Wraps an existing IList<T> without copying data. All operations delegate to the underlying collection.

### Lookup flow
- Direct mapping to the underlying collection tracking indexes.

## Memory characteristics
- No additional storage
- Minimal overhead (wrapper only)

## Complexity overview

- **Index Lookups**: Fast direct reference offset delegation steps.
- **Collection Wrap Initializations**: Constant-time constant-time reference assignment (O(1)).

## Benchmark results

### Scenario

Compare iteration:
- List<T>
- ReadOnlyCollection<T>

### How to run this benchmark only

```bash
cd benchmarks
dotnet run -c Release -- --filter *Iteration.ReadOnlyCollectionBenchmark*
```

### Benchmark code

[benchmarks/Iteration/ReadOnlyCollectionBenchmark.cs](../../benchmarks/Iteration/ReadOnlyCollectionBenchmark.cs)

### Results

Environment: BenchmarkDotNet 0.15.8; Windows 11 (10.0.26300.9711); virtual CPU `virt-10.0` at 1.00 GHz (1 CPU; 4 logical / 4 physical cores); .NET SDK 10.0.403; .NET 10.0.14, Arm64 RyuJIT (`armv8.0-a`); 3 warmup and 8 measurement iterations. [Full report](../../BenchmarkDotNet.Artifacts/results/PracticalDotNetCollections.Benchmarks.Iteration.ReadOnlyCollectionBenchmark-report-github.md).

| Method                            | N     | Mean        | Error     | StdDev    | Gen0   | Allocated |
|---------------------------------- |------ |------------:|----------:|----------:|-------:|----------:|
| **IterateListForeach**                | **1000**  |    **547.8 ns** |   **6.77 ns** |   **3.01 ns** |      **-** |         **-** |
| IterateReadOnlyCollectionForeach  | 1000  |  2,626.1 ns |  43.02 ns |  19.10 ns | 0.0191 |      40 B |
| IterateListForIndex               | 1000  |    650.3 ns |  47.91 ns |  25.06 ns |      - |         - |
| IterateReadOnlyCollectionForIndex | 1000  |  1,622.5 ns |  39.73 ns |  17.64 ns |      - |         - |
| **IterateListForeach**                | **10000** |  **5,425.6 ns** |  **91.30 ns** |  **40.54 ns** |      **-** |         **-** |
| IterateReadOnlyCollectionForeach  | 10000 | 26,266.8 ns | 641.90 ns | 335.73 ns |      - |      40 B |
| IterateListForIndex               | 10000 |  6,340.0 ns | 105.29 ns |  55.07 ns |      - |         - |
| IterateReadOnlyCollectionForIndex | 10000 | 15,840.8 ns | 188.65 ns |  83.76 ns |      - |         - |

### Interpretation

- **The foreach interface cost**: at N = 10,000, `IterateReadOnlyCollectionForeach` averaged **26.3 us** versus **5.43 us** for `IterateListForeach`, and allocated 40 B. The wrapped collection's enumeration path is more expensive in this benchmark.
- **Direct list enumeration**: `IterateListForeach` allocated nothing in this run, consistent with its specialized value-type enumerator.
- **Index-based iteration**: `IterateReadOnlyCollectionForIndex` averaged **15.8 us** at N = 10,000 versus **26.3 us** for the read-only foreach path, with no allocation. The means favor indexing here; use the table's Error columns to assess the size of the difference.

## Practical optimizations
- Use for API boundaries
- **Always loop over a ReadOnlyCollection using an index-based `for` loop**: If your software architecture requires processing a wrapped collection on hot execution paths, do not write a `foreach` loop. Use a standard `for (int i = 0; i < collection.Count; i++)` statement to completely avoid interface lookup overhead and reference type heap allocations.
- **Expose arrays as ReadOnlySpan views for ultimate read-path speeds**: If you are shipping data down internal method arguments and want an un-mutable shield paired with pure O(1) raw hardware array speed, pass the collection as a `ReadOnlySpan<T>` view instead of a wrapper class object.

## Common mistakes
- Assuming it copies data
- Assuming underlying data cannot change
- **Re-instantiating the wrapper object inside continuous data loops**: Calling `.AsReadOnly()` repeatedly on high-frequency paths forces constant heap wrapper object creation cycles. Wrap the collection once at initialization or isolate it behind a private property backing store.

## When I would choose it
- Exposing read-only view
- When implementing encapsulation layers to protect private state lists from being modified or cleared by public application callers.
- When you require a zero-allocation read-only interface wrapper that automatically reflects structural backend adjustments made by a tracking worker system.

## When I would avoid it
- Need true immutability → use Immutable collections
- On extreme high-frequency execution tracks where raw loop iteration speed is your highest engineering priority. Use `ReadOnlySpan<T>` or direct list references.

## Rule of thumb

Use ReadOnlyCollection<T> as a safety wrapper, not as an immutable collection.
