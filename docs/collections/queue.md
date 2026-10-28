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

- **Enqueue Mutations**: Fast pointer index assignment on average.
- **Dequeue Extractions**: Fast head index advancement.

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

Environment: BenchmarkDotNet 0.15.8; Windows 11 (10.0.26300.9711); virtual CPU `virt-10.0` at 1.00 GHz (1 CPU; 4 logical / 4 physical cores); .NET SDK 10.0.403; .NET 10.0.14, Arm64 RyuJIT (`armv8.0-a`); 3 warmup and 8 measurement iterations. [Full report](../../BenchmarkDotNet.Artifacts/results/PracticalDotNetCollections.Benchmarks.Iteration.QueueBenchmark-report-github.md).

| Method                  | N     | Mean         | Error      | StdDev     | Gen0    | Allocated |
|------------------------ |------ |-------------:|-----------:|-----------:|--------:|----------:|
| **QueueThroughput**         | **1000**  |     **7.410 μs** |  **1.1776 μs** |  **0.5229 μs** |  **4.0283** |   **8.23 KB** |
| PreSizedQueueThroughput | 1000  |     6.686 μs |  0.5207 μs |  0.1857 μs |  1.9379 |   3.97 KB |
| ListRemoveAtBeginning   | 1000  |    84.214 μs | 26.7387 μs | 13.9848 μs |  3.9063 |   8.23 KB |
| **QueueThroughput**         | **10000** |    **61.822 μs** |  **3.4371 μs** |  **1.7976 μs** | **62.3779** | **128.33 KB** |
| PreSizedQueueThroughput | 10000 |    58.729 μs |  8.7286 μs |  3.8755 μs | 18.8599 |  39.13 KB |
| ListRemoveAtBeginning   | 10000 | 5,727.860 μs | 45.5733 μs | 23.8357 μs | 54.6875 | 128.32 KB |

### Interpretation

- **The list removal cost**: at N = 10,000, `ListRemoveAtBeginning` averaged **5.73 ms**, compared with **61.8 us** for `QueueThroughput`. Removing index zero from a list shifts the remaining elements; a queue advances its circular-buffer head instead.
- **The pre-sized capacity trade-off**: at N = 10,000, `PreSizedQueueThroughput` allocated **39.1 KB** versus **128 KB** for the default queue. The means were 58.7 us and 61.8 us respectively, but their Error margins overlap, so the timing difference is inconclusive in this run.

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
