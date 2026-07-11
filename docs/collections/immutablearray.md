# ImmutableArray<T>

## What it is

ImmutableArray<T> is a fixed-size immutable collection. Any modification produces a new instance.

## Typical use cases
- Read-only shared data
- Thread-safe data without locks
- Functional-style programming

## Sample usage

See:
[samples/immutablearray-basic.cs](../../samples/immutablearray-basic.cs)

### How to run the sample

`ash
dotnet run samples/immutablearray-basic.cs
`

## Internal implementation

Backed by an array. Mutations create new arrays (copy-on-write behavior).

### Lookup flow
- Direct index access O(1)

## Memory characteristics
- Each mutation creates a new array
- Higher allocation cost than List<T>
- No internal resizing

## Complexity overview

Lookup: O(1)  
Add: O(n)  
Remove: O(n)

## Benchmark results

### Scenario

Compare building collections:
- ImmutableArray<T>.Add
- List<T>.Add

### How to run this benchmark only

`ash
cd benchmarks
dotnet run -c Release -- --filter *ImmutableArrayBenchmark*
`

### Benchmark code

[benchmarks/Memory/ImmutableArrayBenchmark.cs](../../benchmarks/Memory/ImmutableArrayBenchmark.cs)

### Results

(To be filled after running benchmark)

### Interpretation

(To be filled after running benchmark)

## Practical optimizations
- Use builder (ImmutableArray.CreateBuilder) for bulk operations

## Common mistakes
- Using for frequently changing data

## When I would choose it
- Data is mostly read-only
- Thread safety required without locks

## When I would avoid it
- Frequent mutations → use List<T>

## Rule of thumb

Use ImmutableArray<T> for read-heavy, write-rare scenarios.
