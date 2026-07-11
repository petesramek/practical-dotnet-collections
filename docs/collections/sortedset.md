# SortedSet<T>

## What it is

SortedSet<T> is a tree-based collection that maintains unique elements in sorted order.

## Typical use cases
- Maintaining ordered unique data
- Range queries
- Ordered iteration without sorting step

## Sample usage

See:
[samples/sortedset-ordering.cs](../../samples/sortedset-ordering.cs)

### How to run the sample

`ash
dotnet run samples/sortedset-ordering.cs
`

## Internal implementation

SortedSet<T> is implemented as a balanced binary search tree (red-black tree).

### Lookup flow
- Tree traversal (O(log n))

## Memory characteristics
- Node-based structure
- Higher overhead than array/hash-based collections
- Poorer cache locality than List/HashSet

## Complexity overview

Lookup: O(log n)  
Add: O(log n)  
Remove: O(log n)

## Benchmark results

### Scenario

Compare lookup performance:
- SortedSet<T>.Contains
- HashSet<T>.Contains

### How to run this benchmark only

`ash
cd benchmarks
dotnet run -c Release -- --filter *SortedSetLookupBenchmark*
`

### Benchmark code

[benchmarks/Lookup/SortedSetLookupBenchmark.cs](../../benchmarks/Lookup/SortedSetLookupBenchmark.cs)

### Results

(To be filled after running benchmark)

### Interpretation

(To be filled after running benchmark)

## Practical optimizations
- Use when ordering is required
- Avoid if only lookup speed matters

## Common mistakes
- Using SortedSet instead of HashSet for lookup-heavy workloads

## When I would choose it
- Need sorted unique data
- Need ordered iteration

## When I would avoid it
- Need fastest lookup → use HashSet<T>

## Rule of thumb

Use SortedSet<T> when you need ordering, otherwise prefer HashSet<T>.
