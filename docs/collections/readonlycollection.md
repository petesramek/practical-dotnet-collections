# ReadOnlyCollection<T>

## What it is

ReadOnlyCollection<T> is a wrapper around an existing collection that prevents modification through its interface.

## Typical use cases
- Exposing data safely from APIs
- Preventing accidental modification

## Internal implementation

Wraps an existing IList<T> without copying data. All operations delegate to the underlying collection.

## Memory characteristics
- No additional storage
- Minimal overhead (wrapper only)

## Complexity overview

Same as underlying collection

## Benchmark results

### Scenario

Compare iteration:
- List<T>
- ReadOnlyCollection<T>

### How to run this benchmark only

`ash
cd benchmarks
dotnet run -c Release -- --filter *ReadOnlyCollectionBenchmark*
`

### Benchmark code

[benchmarks/Iteration/ReadOnlyCollectionBenchmark.cs](../../benchmarks/Iteration/ReadOnlyCollectionBenchmark.cs)

### Results

(To be filled after running benchmark)

### Interpretation

(To be filled after running benchmark)

## Practical optimizations
- Use for API boundaries

## Common mistakes
- Assuming it copies data
- Assuming underlying data cannot change

## When I would choose it
- Exposing read-only view

## When I would avoid it
- Need true immutability → use Immutable collections

## Rule of thumb

Use ReadOnlyCollection<T> as a safety wrapper, not as an immutable collection.
