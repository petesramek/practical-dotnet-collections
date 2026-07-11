# FrozenDictionary<TKey, TValue>

## What it is

FrozenDictionary<TKey, TValue> is a read-only, precomputed hash-based dictionary optimized for extremely fast lookups.

## Typical use cases
- Hot-path key/value lookups
- Static configuration data
- High-performance read-only datasets

## Sample usage

See:
[samples/frozendictionary-basic.cs](../../samples/frozendictionary-basic.cs)

### How to run the sample

`ash
dotnet run samples/frozendictionary-basic.cs
`

## Internal implementation

Precomputes optimized hashing structures during creation for fast lookup.

### Lookup flow
- Optimized hash lookup (faster than Dictionary)

## Memory characteristics
- Higher upfront cost to build
- Optimized layout for reads
- No mutation allowed

## Complexity overview

Lookup: O(1) (very fast)  
Add: not supported  
Remove: not supported

## Benchmark results

### Scenario

Compare lookup performance:
- FrozenDictionary<TKey, TValue>.ContainsKey
- Dictionary<TKey, TValue>.ContainsKey

### How to run this benchmark only

`ash
cd benchmarks
dotnet run -c Release -- --filter *FrozenDictionaryLookupBenchmark*
`

### Benchmark code

[benchmarks/Lookup/FrozenDictionaryLookupBenchmark.cs](../../benchmarks/Lookup/FrozenDictionaryLookupBenchmark.cs)

### Results

(To be filled after running benchmark)

### Interpretation

(To be filled after running benchmark)

## Practical optimizations
- Build once, reuse many times
- Use only for read-only scenarios

## Common mistakes
- Rebuilding frequently (loses benefits)

## When I would choose it
- Extremely frequent key-based lookups
- Data does not change

## When I would avoid it
- Data changes → use Dictionary<TKey, TValue>

## Rule of thumb

Use FrozenDictionary<TKey, TValue> when data is static and lookup speed is critical.
