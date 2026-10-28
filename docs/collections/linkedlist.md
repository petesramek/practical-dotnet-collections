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

- **Front/Back Mutations (`AddFirst`/`AddLast`)**: Constant-time constant-time pointer assignments.
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

Environment: BenchmarkDotNet 0.15.8; Windows 11 (10.0.26300.9711); virtual CPU `virt-10.0` at 1.00 GHz (1 CPU; 4 logical / 4 physical cores); .NET SDK 10.0.403; .NET 10.0.14, Arm64 RyuJIT (`armv8.0-a`); 3 warmup and 8 measurement iterations. [Full report](../../BenchmarkDotNet.Artifacts/results/PracticalDotNetCollections.Benchmarks.Add.LinkedListBenchmark-report-github.md).

| Method                | N     | Mean             | Error             | StdDev          | Gen0    | Gen1    | Allocated |
|---------------------- |------ |-----------------:|------------------:|----------------:|--------:|--------:|----------:|
| **ListInsertAtBeginning** | **1000**  |    **71,920.901 ns** |    **19,398.8317 ns** |   **8,613.2015 ns** |  **3.9063** |       **-** |    **8392 B** |
| LinkedListAddFirst    | 1000  |    16,643.207 ns |       755.1503 ns |     335.2914 ns | 22.9492 |       - |   48040 B |
| LinkedListReadIndex   | 1000  |       760.756 ns |        54.8993 ns |      28.7134 ns |       - |       - |         - |
| ListReadIndex         | 1000  |         1.549 ns |         0.2242 ns |       0.0995 ns |       - |       - |         - |
| **ListInsertAtBeginning** | **10000** | **7,564,462.484 ns** | **1,411,572.0659 ns** | **738,279.4849 ns** | **46.8750** |       **-** |  **131368 B** |
| LinkedListAddFirst    | 10000 |   264,712.200 ns |    21,673.7230 ns |  11,335.7762 ns | 98.3887 | 61.0352 |  480040 B |
| LinkedListReadIndex   | 10000 |     9,248.812 ns |     1,517.6221 ns |     673.8336 ns |       - |       - |         - |
| ListReadIndex         | 10000 |         1.346 ns |         0.3520 ns |       0.1841 ns |       - |       - |         - |

### Interpretation

- **The array shifting cost**: inserting 10,000 items at the beginning with `ListInsertAtBeginning` averaged **7.56 ms**. An array-backed list shifts existing elements on each front insertion.
- **The head insertion result**: `LinkedListAddFirst` averaged **265 us** for the same workload, about 29x faster by mean in this run. It allocated **480 KB** versus **131 KB** for the list, because each linked-list entry requires a node.
- **The index reading penalty**: at N = 10,000, `LinkedListReadIndex` averaged **9.25 us** versus **1.35 ns** for `ListReadIndex`. A linked list must traverse nodes to reach an index, while a list indexes its backing array directly.

## Practical optimizations
- Use when insert/remove dominates
- **Always preserve node references during iterations if splicing**: If your algorithm loops through a linked list and removes or adds elements based on complex logic, do not use values directly. Leverage the explicit `LinkedListNode<T>` object reference properties (`.First`, `.Next`, `.Previous`). Passing the target node object straight to `.Remove()` or `.AddAfter()` executes in fast constant time, removing secondary scan loops.
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
