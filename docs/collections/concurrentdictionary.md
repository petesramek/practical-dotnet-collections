# ConcurrentDictionary<TKey, TValue>

## What it is

ConcurrentDictionary<TKey, TValue> is a thread-safe dictionary designed for concurrent access without external locking.

## Typical use cases
- Shared state across threads
- Caching in multi-threaded applications
- Parallel processing pipelines

## Sample usage

See:
[samples/concurrentdictionary-basic.cs](../../samples/concurrentdictionary-basic.cs)

### How to run the sample

`ash
dotnet run samples/concurrentdictionary-basic.cs
`

## Internal implementation

Uses fine-grained locking and lock-free techniques.

### Lookup flow
- Lock-free reads where possible

## Memory characteristics
- Higher overhead than Dictionary
- Designed for concurrency, not minimal memory

## Complexity overview

Lookup: O(1) average  
Add: O(1) average  
Thread-safe operations

## Benchmark results

### Scenario

Compare concurrent writes:
- ConcurrentDictionary
- Dictionary + lock

### How to run this benchmark only

`ash
cd benchmarks
dotnet run -c Release -- --filter *ConcurrentDictionaryBenchmark*
`

### Benchmark code

[benchmarks/Concurrency/ConcurrentDictionaryBenchmark.cs](../../benchmarks/Concurrency/ConcurrentDictionaryBenchmark.cs)

### Results

(To be filled after running benchmark)

### Interpretation

(To be filled after running benchmark)

## Practical optimizations
- Use for multi-threaded scenarios only

## Common mistakes
- Using in single-threaded code (unnecessary overhead)

## When I would choose it
- Multiple threads access shared dictionary

## When I would avoid it
- Single-threaded → use Dictionary

## Rule of thumb

Use ConcurrentDictionary<TKey, TValue> only when concurrency is required.
