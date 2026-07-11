# ConcurrentStack<T>

## What it is

ConcurrentStack<T> is a thread-safe LIFO collection designed for concurrent access without external locking.

## Typical use cases
- Parallel work processing (LIFO)
- Task scheduling systems
- Backtracking with concurrency

## Sample usage

See:
[samples/concurrentstack-processing.cs](../../samples/concurrentstack-processing.cs)

### How to run the sample

`ash
dotnet run samples/concurrentstack-processing.cs
`

## Internal implementation

Lock-free stack using compare-and-swap operations.

### Lookup flow
- No lookup support
- Only Push / TryPop

## Memory characteristics
- Node-based structure
- Higher overhead than Stack<T>
- Designed for concurrency

## Complexity overview

Push: O(1)  
Pop: O(1)  
Thread-safe operations

## Benchmark results

### Scenario

Compare concurrent stack vs locked stack:
- ConcurrentStack<T>
- Stack<T> + lock

### How to run this benchmark only

`ash
cd benchmarks
dotnet run -c Release -- --filter *ConcurrentStackBenchmark*
`

### Benchmark code

[benchmarks/Concurrency/ConcurrentStackBenchmark.cs](../../benchmarks/Concurrency/ConcurrentStackBenchmark.cs)

### Results

(To be filled after running benchmark)

### Interpretation

(To be filled after running benchmark)

## Practical optimizations
- Use for multi-threaded LIFO scenarios

## Common mistakes
- Using in single-threaded code

## When I would choose it
- Multiple threads push/pop data

## When I would avoid it
- Single-threaded → use Stack<T>

## Rule of thumb

Use ConcurrentStack<T> when LIFO behavior and concurrency are required.
