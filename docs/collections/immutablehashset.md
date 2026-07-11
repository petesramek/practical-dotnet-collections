# ImmutableHashSet<T>

## What it is

ImmutableHashSet<T> is an immutable hash-based set that uses structural sharing to allow efficient updates.

## Typical use cases
- Thread-safe sets without locks
- Functional programming
- Shared read-heavy data

## Sample usage

See:
[samples/immutablehashset-basic.cs](../../samples/immutablehashset-basic.cs)

### How to run the sample

`ash
dotnet run samples/immutablehashset-basic.cs
`

## Internal implementation

Hash-based structure with structural sharing (persistent data structure).

### Lookup flow
- Hash lookup O(1) average

## Memory characteristics
- Shares structure between versions
- Higher overhead than HashSet<T>
- Avoids full copies

## Complexity overview

Lookup: O(1) average  
Add: O(log n) amortized  
Remove: O(log n)

## Benchmark results

### Scenario

Compare building sets:
- ImmutableHashSet<T>.Add
- HashSet<T>.Add

### How to run this benchmark only

`ash
cd benchmarks
dotnet run -c Release -- --filter *ImmutableHashSetBenchmark*
`

### Benchmark code

[benchmarks/Memory/ImmutableHashSetBenchmark.cs](../../benchmarks/Memory/ImmutableHashSetBenchmark.cs)

### Results

(To be filled after running benchmark)

### Interpretation

(To be filled after running benchmark)

## Practical optimizations
- Prefer over ImmutableArray when uniqueness required

## Common mistakes
- Using in high mutation scenarios expecting HashSet performance

## When I would choose it
- Need immutability + fast lookup

## When I would avoid it
- High mutation rate → use HashSet<T>

## Rule of thumb

Use ImmutableHashSet<T> when you need immutability with fast lookup.
