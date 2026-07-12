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
- **Peek Checks**: Instant constant-time boundary view.

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

| Method | N | Mean | Gen 0 | Gen 1 | Gen 2 | Allocated |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **PriorityQueueThroughput** | 1000 | 30.24 us | 7.9346 | - | - | 16.23 KB |
| **PreSizedPriorityQueueThroughput** | 1000 | 24.96 us | 3.8452 | - | - | 7.88 KB |
| **ListStreamingSort** | 1000 | 58.60 us | 3.9673 | - | - | 8.20 KB |
| | | | | | | |
| **PriorityQueueThroughput** | 10000 | 567.33 us | 82.0313 | 41.0156 | 41.0156 | 256.33 KB |
| **PreSizedPriorityQueueThroughput** | 10000 | **509.22 us** | 36.1328 | - | - | **78.20 KB** |
| **ListStreamingSort** | 10000 | **4,788.42 us** | 54.6875 | - | - | 128.29 KB |

### Interpretation

- **The List Sorting Catastrophe**: Maintaining an actively sorted tracking map inside a standard list using binary search lookups and mid-array insertions (`ListStreamingSort`) hits a severe performance wall under scale. At N = 10,000, it takes a brutal **4.78 milliseconds**—making it **nearly 10x slower** than the priority queue. While searching is fast, forcing mid-array element shifting loops on every single insertion destroys application throughput.
- **The Heap Traversal Advantage**: The `PriorityQueueThroughput` method breezes through the exact same 10,000 element triage workload in just **567 microseconds**. Because it manages relationships using a compact min-heap tree structure rather than sorting the full array on every pass, it moves elements to their relative sorted slots with minimal work. However, the empty initialization forces multiple array doublings, triggering Gen 1 and Gen 2 garbage collection cycles.
- **The Pre-Sized Payload Optimization**: The `PreSizedPriorityQueueThroughput` track resolves the array expansion tax completely. Specifying the target limit upfront in the constructor (`new PriorityQueue<T, P>(N)`) reduces total allocations down to **78.2 KB** (a **69.5% memory reduction**) and cuts execution time down to 509 us while completely bypassing Gen 1/Gen 2 multi-generation garbage collection pressure.

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
- When pathfinding algorithms or scheduling state machines require instant extraction of the lowest or highest weighted node.

## When I would avoid it
- Simple ordering -> use sorting
- When your system design requires traversing or looking up arbitrary elements in the middle of the collection. `PriorityQueue` only exposes the single top root item; it does not support intermediate indexing or sequential key lookups.

## Rule of thumb

Use PriorityQueue<TElement, TPriority> when you need incremental priority processing.
