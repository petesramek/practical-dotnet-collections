# PriorityQueue<TElement, TPriority>

## What it is

PriorityQueue<TElement, TPriority> is a heap-based collection that processes elements based on priority rather than insertion order.

## Typical use cases
- Task scheduling
- Job prioritization
- Algorithms (Dijkstra, A*)

## Sample usage

See:
[samples/priorityqueue-job-triage.cs](../../samples/priorityqueue-job-triage.cs)

### How to run the sample

```bash
dotnet run samples/priorityqueue-job-triage.cs
```

## Internal implementation

Binary heap stored in array.

### Lookup flow
- Root element has highest priority
- Heap rebalanced on insert/remove

## Memory characteristics
- Backed by array
- Resizes like List<T>

## Complexity overview

- **Enqueue Mutations**: Logarithmic node sorting pointer shifts.
- **Dequeue Extractions**: Logarithmic root-node balancing adjustments.
- **Peek Checks**: Constant-time constant-time boundary view.

## Benchmark results

### Scenario

Compare priority processing:
- PriorityQueue
- List.Sort

### How to run this benchmark only

```bash
cd benchmarks
dotnet run -c Release -- --filter *Add.PriorityQueueBenchmark*
```

### Benchmark code

[benchmarks/Add/PriorityQueueBenchmark.cs](../../benchmarks/Add/PriorityQueueBenchmark.cs)

### Results

Environment: BenchmarkDotNet 0.15.8; Windows 11 (10.0.26300.9711); virtual CPU `virt-10.0` at 1.00 GHz (1 CPU; 4 logical / 4 physical cores); .NET SDK 10.0.403; .NET 10.0.14, Arm64 RyuJIT (`armv8.0-a`); 3 warmup and 8 measurement iterations. [Full report](../../BenchmarkDotNet.Artifacts/results/PracticalDotNetCollections.Benchmarks.Add.PriorityQueueBenchmark-report-github.md).

| Method                          | N     | Mean        | Error        | StdDev       | Gen0    | Gen1    | Gen2    | Allocated |
|-------------------------------- |------ |------------:|-------------:|-------------:|--------:|--------:|--------:|----------:|
| **PriorityQueueThroughput**         | **1000**  |    **45.43 μs** |     **4.992 μs** |     **2.216 μs** |  **7.9346** |       **-** |       **-** |  **16.23 KB** |
| PreSizedPriorityQueueThroughput | 1000  |    46.52 μs |     2.997 μs |     1.567 μs |  3.8452 |       - |       - |   7.88 KB |
| ListStreamingSort               | 1000  |    92.54 μs |     9.461 μs |     4.201 μs |  3.9063 |       - |       - |    8.2 KB |
| **PriorityQueueThroughput**         | **10000** | **1,093.68 μs** |   **358.277 μs** |   **187.386 μs** | **82.0313** | **41.0156** | **41.0156** | **256.33 KB** |
| PreSizedPriorityQueueThroughput | 10000 |   757.51 μs |    43.659 μs |    19.385 μs | 36.1328 |       - |       - |   78.2 KB |
| ListStreamingSort               | 10000 | 7,407.75 μs | 2,100.426 μs | 1,098.564 μs | 54.6875 |       - |       - | 128.29 KB |

### Interpretation

- **The list sorting cost**: at N = 10,000, `ListStreamingSort` averaged **7.41 ms**, compared with **1.09 ms** for `PriorityQueueThroughput`. The list repeatedly shifts elements to maintain sorted order.
- **The heap traversal advantage**: `PriorityQueueThroughput` uses a min-heap rather than sorting the whole list after each insertion. Its N = 10,000 mean was lower than the list's, though the Error margins are substantial for both rows.
- **Pre-sized capacity**: `PreSizedPriorityQueueThroughput` allocated **78.2 KB** at N = 10,000 versus **256 KB** for the default queue. Its mean was also lower (758 us vs 1,094 us), but the timing Error ranges overlap; this run does not establish a reliable speedup.

## Practical optimizations
- Use when continuous priority processing required
- **Always preallocate initial capacity when data volume is known**: If you are populating a triage queue using an active database row count or message array size, pass that target count straight to the constructor (`new PriorityQueue<TElement, TPriority>(capacity)`). This locks down the underlying heap array block immediately, preventing runtime resizing copies.
- **Utilize Dequeue-Enqueue combinations atomically**: If your pipeline regularly pops an entry, modifies its weight, and re-submits it to the tracking pool, use `.EnqueueDequeue()` or `.TryDequeue()` parameters safely to run transitions via unified internal index update tracks.

## Common mistakes
- Using List.Sort repeatedly instead of heap
- **Assuming PriorityQueue maintains a stable chronological arrival order**: If two separate elements are enqueued with the exact same priority value, the queue does not guarantee which one exits first. If you need a stable sorting loop that preserves strict FIFO chronological order for equal-weight items, pair the priority weight with an incrementing counter struct as a composite key.

## When I would choose it
- Need priority-based processing
- When implementing automated alert systems, rate limit engines, or background process triages where items arrive out-of-order but must exit strictly by numerical urgency constraints.
- When pathfinding algorithms or scheduling state machines require fast extraction of the lowest or highest weighted node.

## When I would avoid it
- Simple ordering -> use sorting
- When your system design requires traversing or looking up arbitrary elements in the middle of the collection. `PriorityQueue` only exposes the single top root item; it does not support intermediate indexing or sequential key lookups.

## Rule of thumb

Use PriorityQueue<TElement, TPriority> when you need incremental priority processing.
