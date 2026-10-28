# List<T> (Default) vs List<T> (Preallocated Capacity)

## What this comparison answers

When should you preallocate List capacity instead of relying on automatic resizing?

---

## Key differences

| Aspect | Default List<T> | Preallocated List<T> |
|-------|-----------------|---------------------|
| Initial capacity | Small (0 → 4 → grows) | Fixed upfront |
| Resizing | Multiple reallocations | None (if sized correctly) |
| Memory allocation | Many allocations | Single allocation |
| Performance | More resizing for growing lists | Lower mean through N = 10,000; N = 100,000 timing is inconclusive |

---

## Internal behavior

### Default List<T>
- Starts with small internal array
- When capacity is exceeded:
  - Allocates new larger array (typically doubles)
  - Copies all elements to new array
  - Old array becomes garbage

### Preallocated List<T>
- Allocates required memory once
- No resizing or copying during population

---

## Benchmark scenarios

### 1. Building large collections
- Default List triggers repeated allocations and copies
- Preallocated List performs single allocation

### 2. Memory pressure
- Default List produces multiple temporary arrays
- Preallocated List minimizes garbage collection

---

## Benchmark results

Source: [benchmarks/Memory/ListCapacityBenchmark.cs](../../benchmarks/Memory/ListCapacityBenchmark.cs). Both variants create and fill the list inside the measured method, so the allocation of the list is included. Run with `dotnet run -c Release -- --filter *ListCapacityBenchmark*`.

Environment: BenchmarkDotNet 0.15.8; Windows 11 (10.0.26300.9711); virtual CPU `virt-10.0` at 1.00 GHz (1 CPU; 4 logical / 4 physical cores); .NET SDK 10.0.403; .NET 10.0.14, Arm64 RyuJIT (`armv8.0-a`); 3 warmup and 8 measurement iterations. [Full report](../../BenchmarkDotNet.Artifacts/results/PracticalDotNetCollections.Benchmarks.Memory.ListCapacityBenchmark-report-github.md).

| Method              | N      | Mean            | Error          | StdDev         | Ratio | RatioSD | Gen0     | Gen1     | Gen2     | Allocated | Alloc Ratio |
|-------------------- |------- |----------------:|---------------:|---------------:|------:|--------:|---------:|---------:|---------:|----------:|------------:|
| **DefaultListAdd**      | **10**     |        **62.77 ns** |       **2.849 ns** |       **1.490 ns** |  **1.00** |    **0.03** |   **0.1032** |        **-** |        **-** |     **216 B** |        **1.00** |
| PreallocatedListAdd | 10     |        32.18 ns |       2.159 ns |       0.959 ns |  0.51 |    0.02 |   0.0459 |        - |        - |      96 B |        0.44 |
|                     |        |                 |                |                |       |         |          |          |          |           |             |
| **DefaultListAdd**      | **100**    |       **389.61 ns** |       **6.884 ns** |       **3.056 ns** |  **1.00** |    **0.01** |   **0.5660** |        **-** |        **-** |    **1184 B** |        **1.00** |
| PreallocatedListAdd | 100    |       291.21 ns |       3.213 ns |       1.680 ns |  0.75 |    0.01 |   0.2179 |        - |        - |     456 B |        0.39 |
|                     |        |                 |                |                |       |         |          |          |          |           |             |
| **DefaultListAdd**      | **1000**   |     **3,328.71 ns** |      **25.841 ns** |      **13.515 ns** |  **1.00** |    **0.01** |   **4.0207** |        **-** |        **-** |    **8424 B** |        **1.00** |
| PreallocatedListAdd | 1000   |     2,929.09 ns |      76.592 ns |      34.007 ns |  0.88 |    0.01 |   1.9379 |        - |        - |    4056 B |        0.48 |
|                     |        |                 |                |                |       |         |          |          |          |           |             |
| **DefaultListAdd**      | **10000**  |    **33,267.52 ns** |   **1,131.640 ns** |     **591.870 ns** |  **1.00** |    **0.02** |  **62.4390** |        **-** |        **-** |  **131400 B** |        **1.00** |
| PreallocatedListAdd | 10000  |    27,514.49 ns |     549.916 ns |     196.105 ns |  0.83 |    0.01 |  18.8599 |        - |        - |   40056 B |        0.30 |
|                     |        |                 |                |                |       |         |          |          |          |           |             |
| **DefaultListAdd**      | **100000** | **1,082,017.17 ns** | **428,487.145 ns** | **224,107.062 ns** |  **1.04** |    **0.29** | **285.1563** | **285.1563** | **285.1563** | **1049072 B** |        **1.00** |
| PreallocatedListAdd | 100000 | 1,000,079.35 ns | 677,785.118 ns | 354,494.722 ns |  0.96 |    0.37 | 123.0469 | 123.0469 | 123.0469 |  400097 B |        0.38 |

At N = 100,000, the default list allocated 1.05 MB versus 400 KB for the preallocated list (about 62% less). The timing Error margins are large and overlap at that size; use the smaller-N rows for clearer timing comparisons.

---

## Practical guidance

### Use default List<T> when
- Collection size is small or unknown
- Performance is not critical

### Use preallocated List<T> when
- You know approximate size
- You build large collections
- You want to reduce allocations

---

## The trap

Building large lists without capacity:

```csharp
var list = new List<int>();

for (int i = 0; i < 100000; i++)
{
    list.Add(i);
}
```

This causes multiple reallocations and copies.

Better:

```csharp
var list = new List<int>(100000);

for (int i = 0; i < 100000; i++)
{
    list.Add(i);
}
```

---

## Rule of thumb

If you know the size, set the capacity. It removes the repeated resizing allocations and copies and reduces total allocation.
