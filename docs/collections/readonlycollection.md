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

- **Index Lookups**: Near-instant direct reference offset delegation steps.
- **Collection Wrap Initializations**: Instant constant-time reference assignment (O(1)).

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

| Method | N | Mean | Gen 0 | Allocated |
| :--- | :--- | :--- | :--- | :--- |
| **IterateListForeach** | 1000 | 472.9 ns | - | - |
| **IterateReadOnlyCollectionForeach** | 1000 | 2,234.1 ns | 0.0191 | 40 B |
| **IterateListForIndex** | 1000 | 542.0 ns | - | - |
| **IterateReadOnlyCollectionForIndex** | 1000 | 1,367.6 ns | - | - |
| | | | | |
| **IterateListForeach** | 10000 | 4,548.3 ns | - | - |
| **IterateReadOnlyCollectionForeach** | 10000 | **21,969.1 ns** | - | **40 B** |
| **IterateListForIndex** | 10000 | **5,256.8 ns** | - | - |
| **IterateReadOnlyCollectionForIndex** | 10000 | **13,043.0 ns** | - | - |

### Interpretation

- **The Foreach Interface Trap**: Iterating a `ReadOnlyCollection` using an item loop (`IterateReadOnlyCollectionForeach`) is an exceptional execution performance pitfall. At N = 10,000, it takes a brutal **21.9 microseconds**—making it **nearly 5x slower** than the native list loop. Because the wrapper masks the underlying types, the JIT cannot optimize loop paths. It forces .NET to fallback to a generic `IEnumerable<T>` virtual interface method lookup and leaves a **40-Byte** generic enumerator allocation on the heap on every entry run.
- **The Struct Enumerator Dominance**: The `IterateListForeach` track runs in a mere **4.5 microseconds** with zero heap allocations. When a loop hits a direct list type, the C# compiler uses a specialized, pre-compiled value-type struct enumerator (`List<T>.Enumerator`), which runs inline directly inside CPU registers with absolute maximum hardware speed.
- **The Index Traversal Optimization**: The `IterateReadOnlyCollectionForIndex` method highlights the proper optimization path for wrapped collections. Changing your loop check from a `foreach` statement to an index-bound `for` loop cuts processing delays down to **13.0 microseconds**—speeding up execution by **over 40%** while removing the heap garbage allocation footprint completely. Index access bypasses enumerator state machines, calling internal array offsets through direct reference delegation.

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
