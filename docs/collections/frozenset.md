# FrozenSet<T>

## What it is

FrozenSet<T> is a read-only, precomputed hash-based set optimized for extremely fast lookups.

## Typical use cases
- Hot-path lookups
- Static configuration data
- High-performance read-only datasets

## Sample usage

See:
[samples/frozenset-basic.cs](../../samples/frozenset-basic.cs)

### How to run the sample

`ash
dotnet run samples/frozenset-basic.cs
`

## Internal implementation

Precomputes optimized hashing structures during creation for fast lookup.

### Lookup flow
- Optimized hash lookup (faster than HashSet)

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
- FrozenSet<T>.Contains
- HashSet<T>.Contains

### How to run this benchmark only

`ash
cd benchmarks
dotnet run -c Release -- --filter *FrozenSetLookupBenchmark*
`

### Benchmark code

[benchmarks/Lookup/FrozenSetLookupBenchmark.cs](../../benchmarks/Lookup/FrozenSetLookupBenchmark.cs)

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
- Extremely frequent lookups
- Data does not change

## When I would avoid it
- Data changes → use HashSet<T>

## Rule of thumb

Use FrozenSet<T> when data is static and lookup speed is critical.
