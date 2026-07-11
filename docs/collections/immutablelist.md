# ImmutableList<T>

## What it is

ImmutableList<T> is an immutable, tree-based list that supports efficient updates through structural sharing.

## Typical use cases
- Functional programming
- Shared read-heavy data
- Thread-safe collections without locks

## Sample usage

See:
[samples/immutablelist-basic.cs](../../samples/immutablelist-basic.cs)

### How to run the sample

`ash
dotnet run samples/immutablelist-basic.cs
`

## Internal implementation

Tree-based structure (AVL tree). Updates reuse most of the structure (structural sharing).

### Lookup flow
- Tree traversal (O(log n))

## Memory characteristics
- Shares structure between versions
- Less allocation than ImmutableArray for updates
- Higher overhead than List<T>

## Complexity overview

Lookup: O(log n)  
Add: O(log n)  
Remove: O(log n)

## Benchmark results

### Scenario

Compare building collections:
- ImmutableList<T>.Add
- List<T>.Add

### How to run this benchmark only

`ash
cd benchmarks
dotnet run -c Release -- --filter *ImmutableListBenchmark*
`

### Benchmark code

[benchmarks/Memory/ImmutableListBenchmark.cs](../../benchmarks/Memory/ImmutableListBenchmark.cs)

### Results

(To be filled after running benchmark)

### Interpretation

(To be filled after running benchmark)

## Practical optimizations
- Prefer ImmutableList over ImmutableArray for frequent updates

## Common mistakes
- Assuming it behaves like List<T>

## When I would choose it
- Frequent updates with immutability required

## When I would avoid it
- Performance-critical hot paths → use List<T>

## Rule of thumb

Use ImmutableList<T> when you need immutability with reasonable update performance.
