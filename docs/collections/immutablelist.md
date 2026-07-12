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

| Method | N | Mean | Gen 0 | Gen 1 | Allocated |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **ImmutableListAdd** | 1000 | 183,936.2602 ns | 240.4785 | - | 502,896 B |
| **ImmutableListWithBuilderAdd** | 1000 | 95,319.7534 ns | 22.9492 | - | 48,024 B |
| **ListAdd** | 1000 | 2,606.9691 ns | 4.0207 | - | 8,424 B |
| **ImmutableListRead** | 1000 | 83.0917 ns | - | - | - |
| **ListRead** | 1000 | 0.8572 ns | - | - | - |
| | | | | | |
| **ImmutableListAdd** | 10000 | 2,656,093.8779 ns | 1687.5000 | 781.2500 | 6,653,616 B |
| **ImmutableListWithBuilderAdd** | 10000 | 1,260,724.5687 ns | 105.4688 | 58.5938 | 480,024 B |
| **ListAdd** | 10000 | 26,252.5542 ns | 62.4695 | - | 131,400 B |
| **ImmutableListRead** | 10000 | 80.7988 ns | - | - | - |
| **ListRead** | 10000 | 0.8093 ns | - | - | - |

### Interpretation

- **The Item-Loop Node Avalanche**: Appending 10,000 items item-by-item using `.Add()` (`ImmutableListAdd`) is an exceptional performance pitfall. It takes **2.65 milliseconds** and pollutes the heap with **6.6 Megabytes** of garbage data, generating severe Gen 0 and Gen 1 garbage collection loops. This happens because the internal engine must instantiate and re-balance deep binary tree branches on every element added.
- **The Builder Stabilization Effect**: The `ImmutableListWithBuilderAdd` method balances this construction overhead. Utilizing the companion staging container (`ImmutableList.CreateBuilder<T>`) cuts memory generation down to **480 KB** (a **92.7% memory reduction**) and speeds up processing time by over 2x. Items are held flatly, compiling tree node structures once when `. ToImmutable()` is executed.
- **The Index Reading Penalty**: On index lookup execution paths (`ImmutableListRead`), the collection is **nearly 100x slower** than a standard `List<T>` (~80.7 ns vs ~0.8 ns). A standard list maps index references to direct hardware array offsets instantly (O(1)). In contrast, `ImmutableList` must step sequentially down through layers of branch nodes to find indices within its internal binary tree layout.

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
