# PriorityQueue<TElement, TPriority>

## What it is

PriorityQueue<TElement, TPriority> is a heap-based collection that processes elements based on priority rather than insertion order.

## Typical use cases
- Task scheduling
- Job prioritization
- Algorithms (Dijkstra, A*)

## Sample usage

See:
[samples/priorityqueue-basic.cs](../../samples/priorityqueue-basic.cs)

### How to run the sample

`ash
dotnet run samples/priorityqueue-basic.cs
`

## Internal implementation

Binary heap stored in array.

### Lookup flow
- Root element has highest priority
- Heap rebalanced on insert/remove

## Memory characteristics
- Backed by array
- Resizes like List<T>

## Complexity overview

Enqueue: O(log n)  
Dequeue: O(log n)  
Peek: O(1)

## Benchmark results

### Scenario

Compare priority processing:
- PriorityQueue
- List.Sort

### How to run this benchmark only

`ash
cd benchmarks
dotnet run -c Release -- --filter *PriorityQueueBenchmark*
`

### Benchmark code

[benchmarks/Add/PriorityQueueBenchmark.cs](../../benchmarks/Add/PriorityQueueBenchmark.cs)

### Results

(To be filled after running benchmark)

### Interpretation

(To be filled after running benchmark)

## Practical optimizations
- Use when continuous priority processing required

## Common mistakes
- Using List.Sort repeatedly instead of heap

## When I would choose it
- Need priority-based processing

## When I would avoid it
- Simple ordering → use sorting

## Rule of thumb

Use PriorityQueue<TElement, TPriority> when you need incremental priority processing.
