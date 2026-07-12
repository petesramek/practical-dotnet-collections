# Queue<T>

## What it is

Queue<T> is a FIFO (first-in, first-out) collection optimized for sequential processing where order matters.

## Typical use cases
- Processing jobs/tasks in order
- Producer-consumer pipelines
- Buffered processing

## Sample usage

See:
[samples/queue-message-processing.cs](../../samples/queue-message-processing.cs)

### How to run the sample

```bash
dotnet run samples/queue-message-processing.cs
```

## Internal implementation

Queue<T> uses a circular buffer backed by an array.

### Lookup flow
- No direct lookup
- Only Enqueue / Dequeue operations

## Memory characteristics
- Backed by array
- Resizes when capacity exceeded
- Good cache locality

## Complexity overview

- **Enqueue Mutations**: Near-instant pointer index assignment on average.
- **Dequeue Extractions**: Near-instant head index advancement.

## Benchmark results

### Scenario

Compare queue processing vs incorrect List usage:
- Queue<T>.Enqueue/Dequeue
- List<T>.RemoveAt(0)

### How to run this benchmark only

```bash
cd benchmarks
dotnet run -c Release -- --filter *Iteration.QueueBenchmark*
```

### Benchmark code

[benchmarks/Iteration/QueueBenchmark.cs](../../benchmarks/Iteration/QueueBenchmark.cs)

### Results

| Method | N | Mean | Gen 0 | Allocated |
| :--- | :--- | :--- | :--- | :--- |
| **QueueThroughput** | 1000 | 5.360 us | 4.0283 | 8.23 KB |
| **PreSizedQueueThroughput** | 1000 | 4.763 us | 1.9379 | 3.97 KB |
| **ListRemoveAtBeginning** | 1000 | 53.380 us | 3.9673 | 8.23 KB |
| | | | | |
| **QueueThroughput** | 10000 | 52.167 us | 62.4390 | 128.33 KB |
| **PreSizedQueueThroughput** | 10000 | **46.833 us** | 18.8599 | **39.13 KB** |
| **ListRemoveAtBeginning** | 10000 | **4,833.474 us** | 54.6875 | 128.32 KB |

### Interpretation

- **The List Removal Catastrophe**: Using a standard list to manage a streaming FIFO channel by calling `.RemoveAt(0)` (`ListRemoveAtBeginning`) hits a severe performance wall under scale. At N = 10,000, it takes a brutal **4.83 milliseconds**—making it **over 90x slower** than the queue loop. Because lists are straight contiguous vectors, popping an item from index 0 forces the processor to copy and slide all remaining 9,999 records forward in memory by one slot on every single iteration pass.
- **The Circular Pointer Advantage**: The `QueueThroughput` method finishes the exact same 10,000 element streaming workload in a mere **52.1 microseconds**. Because its internal circular buffer replaces expensive element shifting loops with simple pointer arithmetic math, elements are enqueued and dequeued instantly. However, empty initializations force multiple dynamic capacity array expansions, generating 128.3 KB of data overhead.
- **The Pre-Sized Payload Optimization**: The `PreSizedQueueThroughput` track resolves the dynamic resizing tax completely. Specifying the target limit upfront in the constructor (`new Queue<int>(N)`) reduces total allocations down to **39.1 KB** (a **69.5% memory reduction**) and cuts execution time to 46.8 us, eliminating intermediate array copies and heap garbage collection traffic.

## Practical optimizations
- Use Queue for FIFO scenarios
- Avoid List.RemoveAt(0)
- **Always provide a preallocated starting capacity if known**: If you are populating a queue buffer using an explicit database message block or tracking count, supply that number straight to the constructor (`new Queue<T>(capacity)`). This locks down a single circular memory array block immediately, preventing runtime resizing overhead.
- **Consolidate validations into atomic extractions**: Never combine separate availability validations in high-frequency loops (such as checking `if (queue.Count > 0)` followed immediately by `.Dequeue()`). Use `.TryDequeue()` to handle presence testing and item extraction in a single, safe step.

## Common mistakes
- Using List for queue behavior
- Expecting random access
- **Iterating through a queue via foreach loops to process elements**: Running a `foreach` loop over a queue merely enumerates its contents without extracting them. To process a queue correctly, pull items out chronologically using an active `.TryDequeue()` loop.

## When I would choose it
- FIFO processing required
- Sequential consumption of items
- When implementing background thread workflows, messaging channels, or command sequences where items must exit in the exact chronological sequence they arrived (FIFO).

## When I would avoid it
- Need random access
- Need key-based lookup
- In multi-threaded tracks where several background paths mutate data simultaneously without external lock synchronization. Use `ConcurrentQueue<T>` or `Channel<T>` instead.

## Rule of thumb

Use Queue<T> when order of processing matters.
