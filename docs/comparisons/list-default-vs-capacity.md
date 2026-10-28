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
| Performance | Slower for large builds | Faster (about 0.75x to 0.84x of the time at N >= 100) |

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

| Method | N | Mean | Ratio | Allocated |
| :--- | :--- | :--- | :--- | :--- |
| DefaultListAdd | 10 | 106.46 ns | 1.00 | 216 B |
| PreallocatedListAdd | 10 | 55.96 ns | 0.53 | 96 B |
| DefaultListAdd | 1000 | 5,771.81 ns | 1.00 | 8,424 B |
| PreallocatedListAdd | 1000 | 4,147.45 ns | 0.75 | 4,056 B |
| DefaultListAdd | 100000 | 1,422,181.93 ns | 1.00 | 1,049,072 B |
| PreallocatedListAdd | 100000 | 1,176,335.29 ns | 0.84 | 400,097 B |

Short BenchmarkDotNet job (3 warmup, 8 iterations), Arm64, .NET 10. The time ratio is modest (roughly 15% to 25% less time); the allocation reduction is larger (about 60% less at N = 100,000).

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
