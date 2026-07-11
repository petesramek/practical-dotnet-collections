# ConcurrentBag<T>

## What it is

ConcurrentBag<T> is a thread-safe, unordered collection optimized for high-throughput concurrent scenarios.

## Typical use cases
- Parallel processing where order does not matter
- Work stealing scenarios
- High-throughput producer-consumer pipelines

## Sample usage

See:
[samples/concurrentbag-processing.cs](../../samples/concurrentbag-processing.cs)

### How to run the sample

`ash
dotnet run samples/concurrentbag-processing.cs
`

## Internal implementation

Uses thread-local storage and work-stealing algorithms.

### Lookup flow
- No ordering guarantees
- Uses TryTake / Add

## Memory characteristics
- Thread-local buffers
- Higher memory overhead
- Optimized for throughput

## Complexity overview

Add: O(1)  
Take: O(1) average  
Thread-safe operations

## Benchmark results

### Scenario

Compare concurrent bag vs locked list:
- ConcurrentBag<T>
- List<T> + lock

### How to run this benchmark only

`ash
cd benchmarks
dotnet run -c Release -- --filter *ConcurrentBagBenchmark*
`

### Benchmark code

[benchmarks/Concurrency/ConcurrentBagBenchmark.cs](../../benchmarks/Concurrency/ConcurrentBagBenchmark.cs)

### Results

(To be filled after running benchmark)

### Interpretation

(To be filled after running benchmark)

## Practical optimizations
- Use when order does not matter

## Common mistakes
- Expecting FIFO or LIFO behavior

## When I would choose it
- High-throughput concurrent workloads
- Unordered processing

## When I would avoid it
- Order matters → use Queue/Stack

## Rule of thumb

Use ConcurrentBag<T> when you need maximum throughput and don’t care about order.
