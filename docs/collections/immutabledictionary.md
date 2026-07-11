# ImmutableDictionary<TKey, TValue>

## What it is

ImmutableDictionary<TKey, TValue> is an immutable, hash-based key/value collection that uses structural sharing to support efficient updates.

## Typical use cases
- Thread-safe shared state
- Functional programming
- Read-heavy data with occasional updates

## Sample usage

See:
[samples/immutabledictionary-basic.cs](../../samples/immutabledictionary-basic.cs)

### How to run the sample

`ash
dotnet run samples/immutabledictionary-basic.cs
`

## Internal implementation

Hash-based persistent data structure with structural sharing.

### Lookup flow
- Hash lookup O(1) average

## Memory characteristics
- Shares structure between versions
- Higher overhead than Dictionary
- Avoids full copies on updates

## Complexity overview

Lookup: O(1) average  
Add: O(log n) amortized  
Remove: O(log n)

## Benchmark results

### Scenario

Compare building dictionaries:
- ImmutableDictionary<TKey, TValue>.Add
- Dictionary<TKey, TValue>.Add

### How to run this benchmark only

`ash
cd benchmarks
dotnet run -c Release -- --filter *ImmutableDictionaryBenchmark*
`

### Benchmark code

[benchmarks/Memory/ImmutableDictionaryBenchmark.cs](../../benchmarks/Memory/ImmutableDictionaryBenchmark.cs)

### Results

(To be filled after running benchmark)

### Interpretation

(To be filled after running benchmark)

## Practical optimizations
- Use builders (ToBuilder) for bulk updates

## Common mistakes
- Using in high mutation scenarios expecting Dictionary performance

## When I would choose it
- Need immutability + key/value mapping

## When I would avoid it
- Frequent updates → use Dictionary<TKey, TValue>

## Rule of thumb

Use ImmutableDictionary<TKey, TValue> when you need immutability with efficient lookup.
