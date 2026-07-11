# Queue<T>

## What it is

Queue<T> is a FIFO (first-in, first-out) collection optimized for sequential processing where order matters.

## Typical use cases
- Processing jobs/tasks in order
- Producer-consumer pipelines
- Buffered processing

## Sample usage

See:
[samples/queue-processing.cs](../../samples/queue-processing.cs)

### How to run the sample

`ash
dotnet run samples/queue-processing.cs
`

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

Enqueue: O(1) amortized  
Dequeue: O(1)  
Lookup: not supported

## Benchmark results

### Scenario

Compare queue processing vs incorrect List usage:
- Queue<T>.Enqueue/Dequeue
- List<T>.RemoveAt(0)

### How to run this benchmark only

`ash
cd benchmarks
dotnet run -c Release -- --filter *QueueDequeueBenchmark*
`

### Benchmark code

[benchmarks/Iteration/QueueDequeueBenchmark.cs](../../benchmarks/Iteration/QueueDequeueBenchmark.cs)

### Results

(To be filled after running benchmark)

### Interpretation

(To be filled after running benchmark)

## Practical optimizations
- Use Queue for FIFO scenarios
- Avoid List.RemoveAt(0)

## Common mistakes
- Using List for queue behavior
- Expecting random access

## When I would choose it
- FIFO processing required
- Sequential consumption of items

## When I would avoid it
- Need random access
- Need key-based lookup

## Rule of thumb

Use Queue<T> when order of processing matters.
