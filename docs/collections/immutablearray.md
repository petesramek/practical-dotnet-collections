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

- **Index Lookups**: Instant direct hardware array offset mapping.
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

| Method | N | Mean | Gen 0 | Allocated |
| :--- | :--- | :--- | :--- | :--- |
| **ImmutableArrayAdd** | 1000 | 121,709.8404 ns | 968.9941 | 2,028,000 B |
| **ImmutableArrayWithBuilderAdd** | 1000 | 2,773.7270 ns | 5.9166 | 12,376 B |
| **ListAdd** | 1000 | 2,570.3695 ns | 4.0207 | 8,424 B |
| **ImmutableArrayRead** | 1000 | 0.6611 ns | - | - |
| **ListRead** | 1000 | 0.6676 ns | - | - |
| | | | | |
| **ImmutableArrayAdd** | 10000 | 9,492,667.6649 ns | 95000.0000 | 200,280,000 B |
| **ImmutableArrayWithBuilderAdd** | 10000 | 27,527.2640 ns | 81.0547 | 171,352 B |
| **ListAdd** | 10000 | 25,606.4429 ns | 62.4695 | 131,400 B |
| **ImmutableArrayRead** | 10000 | 0.6695 ns | - | - |
| **ListRead** | 10000 | 0.6721 ns | - | - |

### Interpretation

- **The Item-by-Item Loop Catastrophe**: The `ImmutableArrayAdd` method illustrates a severe architectural performance pitfall. Appending 10,000 items sequentially via the `.Add()` extension forces the runtime to re-allocate and duplicate 10,000 separate array instances. This drags execution speed down to **9.49 milliseconds** and generates a massive **200.2 Megabytes** of temporary heap trash.
- **The Builder Extinction Optimization**: The `ImmutableArrayWithBuilderAdd` track fixes this memory issue completely. By utilizing the dedicated internal staging object (`ImmutableArray.CreateBuilder<T>`), items are compiled inside a single, resizable array. Calling `.ToImmutable()` then locks down that specific array reference. This brings loop insertion speed back down to **27.5 microseconds**—making it **340x faster** than a direct loop while removing 99.9% of the heap pollution.
- **The Raw Hardware Indexing Victory**: On pure read paths (`ImmutableArrayRead`), the collection runs faster than a standard `List<T>` (~0.66 ns vs ~0.67 ns). Because `ImmutableArray<T>` wraps a raw memory array block with zero object proxy lookups or tracking abstraction logic, it maps index requests to direct memory pointer addresses with absolute maximum hardware speed.

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
- Inside short-lived algorithms where datasets are modified instantly before being immediately discarded.

## Rule of thumb

Use ImmutableArray<T> for read-heavy, write-rare scenarios.
