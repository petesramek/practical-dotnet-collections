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

| Method | N = 10 | N = 100 | N = 1,000 | N = 10,000 | N = 100,000 |
| :--- | :--- | :--- | :--- | :--- | :--- |
| List.Contains (missing) | 1.9 ns | 14.1 ns | 148.8 ns | 1,604 ns | 12,544 ns |
| HashSet.Contains (missing) | 4.7 ns | 3.0 ns | 3.3 ns | 3.0 ns | 3.1 ns |
| Dictionary.ContainsKey (missing) | 2.6 ns | 2.5 ns | 3.8 ns | 3.0 ns | 3.4 ns |

No allocations in any case. Short BenchmarkDotNet job (3 warmup, 8 iterations), Arm64, .NET 10.

- The list scan grows roughly linearly with N; the hash-based lookups stay around 3 ns.
- At N = 10 the list was faster (1.9 ns vs 4.7 ns for `HashSet`), and the crossover is somewhere between 10 and 100 items for `int` keys. The error margin at N = 10 is large relative to the values, so treat that gap as approximate.

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
