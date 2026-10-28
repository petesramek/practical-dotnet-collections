# HashSet<T> vs List<T> (Lookup)

## What this comparison answers

When should you use a hash-based lookup vs scanning a list?

---

## Key differences

| Aspect | List<T> | HashSet<T> |
|-------|--------|------------|
| Storage | Contiguous array | Hash buckets |
| Lookup | Linear scan | Hash-based lookup |
| Memory | Compact | Higher overhead |
| Ordering | Preserved | Not guaranteed |
| Duplicates | Allowed | Not allowed |

---

## Internal behavior

### List<T>
- Stored in a single array
- Lookup requires scanning elements one by one
- Best case: first item
- Worst case: last item or not found

### HashSet<T>
- Uses hashing to distribute items into buckets
- Lookup jumps directly to bucket
- May handle collisions internally

---

## Benchmark scenarios

### 1. Successful lookup
- List: may scan partially
- HashSet: direct bucket access

### 2. Failed lookup (IMPORTANT)
- List: scans entire array
- HashSet: still fast lookup

---

## Benchmark results: failed lookups

A missing key is the worst case for a list, because the whole array is scanned. Source: [benchmarks/Lookup/FailedLookupBenchmark.cs](../../benchmarks/Lookup/FailedLookupBenchmark.cs). Run with `dotnet run -c Release -- --filter *FailedLookupBenchmark*`.

Environment: BenchmarkDotNet 0.15.8; Windows 11 (10.0.26300.9711); virtual CPU `virt-10.0` at 1.00 GHz (1 CPU; 4 logical / 4 physical cores); .NET SDK 10.0.403; .NET 10.0.14, Arm64 RyuJIT (`armv8.0-a`); 3 warmup and 8 measurement iterations. [Full report](../../BenchmarkDotNet.Artifacts/results/PracticalDotNetCollections.Benchmarks.Lookup.FailedLookupBenchmark-report-github.md).

| Method                  | N      | Mean          | Error       | StdDev      | Allocated |
|------------------------ |------- |--------------:|------------:|------------:|----------:|
| **List_FailedLookup**       | **10**     |      **1.519 ns** |   **0.2498 ns** |   **0.1109 ns** |         **-** |
| HashSet_FailedLookup    | 10     |      2.880 ns |   0.1771 ns |   0.0926 ns |         - |
| Dictionary_FailedLookup | 10     |      2.493 ns |   0.0407 ns |   0.0213 ns |         - |
| **List_FailedLookup**       | **100**    |     **18.989 ns** |   **0.3629 ns** |   **0.1898 ns** |         **-** |
| HashSet_FailedLookup    | 100    |      2.490 ns |   0.0541 ns |   0.0283 ns |         - |
| Dictionary_FailedLookup | 100    |      2.676 ns |   0.1694 ns |   0.0752 ns |         - |
| **List_FailedLookup**       | **1000**   |    **155.684 ns** |   **4.1442 ns** |   **1.4779 ns** |         **-** |
| HashSet_FailedLookup    | 1000   |      2.675 ns |   0.1092 ns |   0.0485 ns |         - |
| Dictionary_FailedLookup | 1000   |      2.666 ns |   0.0983 ns |   0.0436 ns |         - |
| **List_FailedLookup**       | **10000**  |  **1,254.955 ns** | **112.9401 ns** |  **59.0698 ns** |         **-** |
| HashSet_FailedLookup    | 10000  |      2.986 ns |   2.5307 ns |   1.1236 ns |         - |
| Dictionary_FailedLookup | 10000  |      2.662 ns |   0.3221 ns |   0.1430 ns |         - |
| **List_FailedLookup**       | **100000** | **12,010.048 ns** | **676.3112 ns** | **353.7238 ns** |         **-** |
| HashSet_FailedLookup    | 100000 |      2.648 ns |   0.6737 ns |   0.3524 ns |         - |
| Dictionary_FailedLookup | 100000 |      2.940 ns |   1.0557 ns |   0.5521 ns |         - |

No allocations were reported in these failed-lookup benchmarks.

- The list scan grows with N, while the hash-based means remained around 2.5-3 ns across the measured sizes.
- At N = 10, `List_FailedLookup` averaged 1.519 ns versus 2.880 ns for `HashSet_FailedLookup`; the Error ranges do not overlap in this run. At N = 100, the list mean was 18.989 ns versus 2.490 ns for HashSet, so the crossover is between the tested sizes 10 and 100 for these `int` keys.

---

## Practical guidance

### Use List<T> when
- Collection is small
- You iterate more than you search
- Ordering matters

### Use HashSet<T> when
- You perform frequent lookups
- You need uniqueness
- Collection grows large

---

## The trap

Using List<T> for repeated lookups in large collections.

```csharp
if (list.Contains(value))
{
    // executed many times
}
```

This scans the list every time.

Replace with:

```csharp
var set = new HashSet<int>(list);

if (set.Contains(value))
{
    // fast lookup
}
```

---

## Rule of thumb

If you call .Contains() frequently, use HashSet<T>. If you mostly iterate, use List<T>.
