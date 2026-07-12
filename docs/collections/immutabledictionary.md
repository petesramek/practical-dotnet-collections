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

| Method | N | Mean | Gen 0 | Gen 1 | Gen 2 | Allocated |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **ImmutableDictionaryAdd** | 1000 | 261,056.853 ns | 286.1328 | - | - | 598,712 B |
| **ImmutableDictionaryWithBuilderAdd** | 1000 | 129,912.802 ns | 26.6113 | - | - | 56,040 B |
| **DictionaryAdd** | 1000 | 8,866.314 ns | 34.4696 | - | - | 73,168 B |
| **ImmutableDictionaryLookup** | 1000 | 86.335 ns | - | - | - | - |
| **DictionaryLookup** | 1000 | 4.150 ns | - | - | - | - |
| | | | | | | |
| **ImmutableDictionaryAdd** | 10000 | 3,637,559.914 ns | 1917.9688 | 640.6250 | 3.9063 | 7,882,553 B |
| **ImmutableDictionaryWithBuilderAdd** | 10000 | 1,650,806.896 ns | 123.0469 | 68.3594 | - | 560,040 B |
| **DictionaryAdd** | 10000 | 332,791.280 ns | 150.8789 | 54.1992 | 41.0156 | 673,201 B |
| **ImmutableDictionaryLookup** | 10000 | 78.770 ns | - | - | - | - |
| **DictionaryLookup** | 10000 | 4.209 ns | - | - | - | - |

### Interpretation

- **The Item Loop Allocation Disaster**: Appending 10,000 items individually using `.Add()` (`ImmutableDictionaryAdd`) is an exceptional performance pitfall. It takes **3.63 milliseconds** and pollutes the heap with **7.8 Megabytes** of garbage, forcing frequent Gen 1 and Gen 2 garbage collection cycles. This happens because the framework must rebuild and re-balance an entire binary tree route map on every single entry loop pass.
- **The Builder Salvation Factor**: The `ImmutableDictionaryWithBuilderAdd` method mitigates this initialization penalty. Using the dedicated intermediate `Builder` pattern reduces allocation footprint down to **560 KB** (a **92.8% reduction**) and cuts execution time in half to 1.65 ms. The builder staging area stores items flatly, compiling the binary nodes only once when `.ToImmutable()` is invoked.
- **The Lookup Read Penalty**: On lookup execution hot paths (`ImmutableDictionaryLookup`), the collection is **nearly 20x slower** than a standard `Dictionary<TKey, TValue>` (~78.7 ns vs ~4.2 ns). While a standard dictionary uses flat hash math calculations to hit bucket memory directly, `ImmutableDictionary` must actively step down through layers of pointer branches to find keys within its internal tree structures.

## Practical optimizations
- Use builders (ToBuilder) for bulk updates
- **Always batch map constructions inside the Stateful Builder**: If you must assemble an `ImmutableDictionary` dynamically using configuration cycles, never invoke direct sequential `.Add()` operations. Always construct the collection via `ImmutableDictionary.CreateBuilder<K, V>()` to protect your runtime from massive heap allocation churn.
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
