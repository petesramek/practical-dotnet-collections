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

| Method | N | Mean | Gen 0 | Gen 1 | Gen 2 | Allocated |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **ImmutableHashSetAdd** | 1000 | 230,404.545 ns | 290.0391 | - | - | 606,712 B |
| **ImmutableHashSetWithBuilderAdd** | 1000 | 124,495.481 ns | 26.6113 | - | - | 56,048 B |
| **HashSetAdd** | 1000 | 8,081.024 ns | 27.7710 | - | - | 58,664 B |
| **ImmutableHashSetContains** | 1000 | 114.888 ns | - | - | - | - |
| **HashSetContains** | 1000 | 4.494 ns | - | - | - | - |
| | | | | | | |
| **ImmutableHashSetAdd** | 10000 | 3,270,639.258 ns | 1953.1250 | 582.0313 | 7.8125 | 7,962,555 B |
| **ImmutableHashSetWithBuilderAdd** | 10000 | 1,605,048.327 ns | 123.0469 | 68.3594 | - | 560,048 B |
| **HashSetAdd** | 10000 | 302,168.659 ns | 132.8125 | 44.9219 | 34.1797 | 538,683 B |
| **ImmutableHashSetContains** | 10000 | 109.239 ns | - | - | - | - |
| **HashSetContains** | 10000 | 4.154 ns | - | - | - | - |

### Interpretation

- **The Item Loop Allocation Disaster**: Appending 10,000 items individually using `.Add()` (`ImmutableHashSetAdd`) is an exceptional performance pitfall. It takes **3.27 milliseconds** and pollutes the heap with **7.9 Megabytes** of garbage, forcing frequent Gen 1 and Gen 2 garbage collection cycles. This happens because the framework must rebuild and re-balance an entire binary tree route map on every single entry loop pass.
- **The Builder Salvation Factor**: The `ImmutableHashSetWithBuilderAdd` method mitigates this initialization penalty. Using the dedicated intermediate `Builder` pattern reduces allocation footprint down to **560 KB** (a **92.8% reduction**) and cuts execution time in half to 1.60 ms. The builder staging area stores items flatly, compiling the binary nodes only once when `.ToImmutable()` is invoked.
- **The Containment Check Read Penalty**: On lookup execution hot paths (`ImmutableHashSetContains`), the collection is **nearly 25x slower** than a standard `HashSet<int>` (~109.2 ns vs ~4.1 ns). While a standard unique hash set uses flat hash math calculations to hit bucket memory directly, `ImmutableHashSet` must actively step down through layers of pointer branches to find items within its internal tree structures.

## Practical optimizations
- Prefer over ImmutableArray when uniqueness required
- **Always batch map constructions inside the Stateful Builder**: If you must assemble an `ImmutableHashSet` dynamically using configuration cycles, never invoke direct sequential `.Add()` operations. Always construct the collection via `ImmutableHashSet.CreateBuilder<T>()` to protect your runtime from massive heap allocation churn.
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
