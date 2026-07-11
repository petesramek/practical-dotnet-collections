# ConcurrentQueue<T>

## What it is

ConcurrentQueue<T> is a thread-safe FIFO collection designed for concurrent producer-consumer scenarios.

## Typical use cases
- Producer-consumer pipelines
- Work queues across threads
- Background processing systems

## Sample usage

See:
[samples/concurrentqueue-processing.cs](../../samples/concurrentqueue-processing.cs)

### How to run the sample

`ash
dotnet run samples/concurrentqueue-processing.cs
`

## Internal implementation

Lock-free, segment-based queue optimized for concurrent access.

### Lookup flow
- No direct lookup
- Uses TryDequeue / Enqueue

## Memory characteristics
- Segment-based allocation
- Higher overhead than Queue<T>
- Designed for concurrency

## Complexity overview

Enqueue: O(1)  
Dequeue: O(1)  
Thread-safe operations

## Benchmark results

### Scenario

Compare concurrent queue vs locked queue:
- ConcurrentQueue<T>
- Queue<T> + lock

### How to run this benchmark only

`ash
cd benchmarks
dotnet run -c Release -- --filter *ConcurrentQueueBenchmark*
`

### Benchmark code

[benchmarks/Concurrency/ConcurrentQueueBenchmark.cs](../../benchmarks/Concurrency/ConcurrentQueueBenchmark.cs)

### Results

(To be filled after running benchmark)

### Interpretation

(To be filled after running benchmark)

## Practical optimizations
- Use for multi-threaded pipelines

## Common mistakes
- Using Queue<T> with locks unnecessarily

## When I would choose it
- Multiple producers/consumers
- High concurrency

## When I would avoid it
- Single-threaded → use Queue<T>

## Rule of thumb

Use ConcurrentQueue<T> for thread-safe FIFO scenarios.
