# ReadOnlyDictionary<TKey, TValue>

## What it is

ReadOnlyDictionary<TKey, TValue> is a wrapper around an existing dictionary that prevents modification through its interface.

## Typical use cases
- Exposing dictionary data safely from APIs
- Preventing accidental modification

## Internal implementation

Wraps an existing IDictionary<TKey, TValue> without copying data. All operations delegate to the underlying dictionary.

## Memory characteristics
- No additional storage
- Minimal overhead (wrapper only)

## Complexity overview

Same as underlying dictionary

## Benchmark results

### Scenario

Compare lookup:
- Dictionary<TKey, TValue>
- ReadOnlyDictionary<TKey, TValue>

### How to run this benchmark only

`ash
cd benchmarks
dotnet run -c Release -- --filter *ReadOnlyDictionaryBenchmark*
`

### Benchmark code

[benchmarks/Lookup/ReadOnlyDictionaryBenchmark.cs](../../benchmarks/Lookup/ReadOnlyDictionaryBenchmark.cs)

### Results

(To be filled after running benchmark)

### Interpretation

(To be filled after running benchmark)

## Practical optimizations
- Use at API boundaries

## Common mistakes
- Assuming it copies data
- Assuming underlying data cannot change

## When I would choose it
- Need read-only view of dictionary

## When I would avoid it
- Need true immutability → use ImmutableDictionary

## Rule of thumb

Use ReadOnlyDictionary<TKey, TValue> as a wrapper, not as an immutable collection.
