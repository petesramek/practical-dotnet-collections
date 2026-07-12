# LinkedList<T>

## What it is

LinkedList<T> is a node-based collection optimized for fast insertions and removals.

## Typical use cases
- Frequent inserts/removes at beginning or middle
- When shifting elements is too expensive

## Sample usage

See:
[samples/linkedlist-prepend.cs](../../samples/linkedlist-prepend.cs)

### How to run the sample

```bash
dotnet run samples/linkedlist-prepend.cs
```

## Internal implementation

Doubly-linked list of nodes.

### Lookup flow
- Sequential traversal required

## Memory characteristics
- Each element is a separate node
- Higher memory overhead
- Poor cache locality

## Complexity overview

- **Front/Back Mutations (`AddFirst`/`AddLast`)**: Instant constant-time pointer assignments.
- **Index Lookups**: Linear node-by-node traversal loops.

## Benchmark results

### Scenario

Compare insert at beginning:
- List<T>.Insert(0, item)
- LinkedList<T>.AddFirst

### How to run this benchmark only

```bash
cd benchmarks
dotnet run -c Release -- --filter *Add.LinkedListBenchmark*
```

### Benchmark code

[benchmarks/Add/LinkedListBenchmark.cs](../../benchmarks/Add/LinkedListBenchmark.cs)

### Results

| Method | N | Mean | Gen 0 | Gen 1 | Allocated |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **ListInsertAtBeginning** | 1000 | 61,844.2193 ns | 3.9673 | - | 8,392 B |
| **LinkedListAddFirst** | 1000 | 12,793.8849 ns | 22.9645 | - | 48,040 B |
| **LinkedListReadIndex** | 1000 | 501.1954 ns | - | - | - |
| **ListReadIndex** | 1000 | 0.8606 ns | - | - | - |
| | | | | | |
| **ListInsertAtBeginning** | 10000 | 4,676,077.7375 ns | 54.6875 | - | 131,368 B |
| **LinkedListAddFirst** | 10000 | **152,201.8592 ns** | 97.6563 | 61.2793 | 480,040 B |
| **LinkedListReadIndex** | 10000 | **5,567.5224 ns** | - | - | - |
| **ListReadIndex** | 10000 | **0.8598 ns** | - | - | - |

### Interpretation

- **The Array Shifting Catastrophe**: Appending items to the absolute front of a standard list (`ListInsertAtBeginning`) is a severe performance pitfall. At N = 10,000, it takes a brutal **4.67 milliseconds**. Because arrays are contiguous, adding an item at index 0 forces the CPU to copy and shift all existing 9,999 items forward by one memory slot on every single loop pass.
- **The Head Insertion Victory**: The `LinkedListAddFirst` method bypasses this shifting cost entirely, completing the exact same 10,000 element front-insertion pass in just **152 microseconds**—making it **over 30x faster** than a standard list. It allocates a node and updates the head references instantly without touching any existing memory slots. However, notice the allocation tax: it generates **480 KB** of heap memory trash (nearly 4x more than the list) and forces heavy garbage collection cleanups due to creating 10,000 independent wrapper objects.
- **The Index Reading Penalty**: On index-based lookup execution paths, the structural comparison flips dramatically. `ListReadIndex` performs constant-time lookups in a flat **0.85 ns** regardless of data scale because it calculates memory hardware offsets directly. In contrast, `LinkedListReadIndex` takes **5,567.5 ns**—making it **nearly 6,500x slower**. Because a linked list lacks contiguous memory offsets, it must traverse from node to node to resolve an index.

## Practical optimizations
- Use when insert/remove dominates
- **Always preserve node references during iterations if splicing**: If your algorithm loops through a linked list and removes or adds elements based on complex logic, do not use values directly. Leverage the explicit `LinkedListNode<T>` object reference properties (`.First`, `.Next`, `.Previous`). Passing the target node object straight to `.Remove()` or `.AddAfter()` executes in instant constant time, removing secondary scan loops.
- **Rely strictly on linear foreach or stream cursors**: Never write index-bound loops (such as using a `for` loop accessing `list.ElementAt(i)`) over a `LinkedList<T>`. This creates an exponential performance degradation because every single iteration restarts a node pointer scan from the head.

## Common mistakes
- Using for lookup-heavy workloads
- **Using LinkedList assuming it automatically runs faster than a List**: Contiguous memory arrays are highly optimized for CPU cache lines. Unless your application requires continuous insertions or deletions at the collection boundaries, a standard `List<T>` easily out-allocates and out-performs a linked list across typical business tracking paths.

## When I would choose it
- Frequent insert/remove operations
- When implementing logging streams, real-time activity dashboards, or undo histories where items are pre-pended or appended continuously to the sequence boundaries.

## When I would avoid it
- Frequent lookups
- Memory-sensitive scenarios
- In memory-constrained scenarios where object overhead per element must be restricted to prevent garbage collection pressure.

## Rule of thumb

Use LinkedList<T> when insertion cost matters more than lookup performance.
